//
// LocalFolderPhotoSource.cs
// 
// Author:
//   Ruben Vermeersch <ruben@savanne.be>
// 
// Copyright (c) 2010 Ruben Vermeersch <ruben@savanne.be>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using GLib;
using System;
using System.Linq;
using System.Collections.Generic;
using Hyena.Data.Sqlite;
using Tripod.Base;
using Hyena.Jobs;

namespace Tripod.Model
{
    public class LocalFolderPhotoSource : ICacheablePhotoSource
    {
        static SqliteModelProvider<LocalFolderPhotoSourceParameters> parameter_provider = new SqliteModelProvider<LocalFolderPhotoSourceParameters> (Core.DbConnection, "LocalFolderSourceParameters");
        static SqliteModelProvider<LocalFolderPhotoSourceUris> uri_provider = new SqliteModelProvider<LocalFolderPhotoSourceUris> (Core.DbConnection, "LocalFolderSourceUris");

        public int CacheId { get; set; }


        Uri root;

        public LocalFolderPhotoSource ()
        {
        }

        public LocalFolderPhotoSource (Uri root)
        {
            this.root = root;
        }

        string display_name = String.Empty;
        public string DisplayName {
            get {
                if (display_name == String.Empty) {
                    var segments = root.Segments;
                    display_name = segments[segments.Length - 1].Trim (new char[] { '/' });
                }
                return display_name;
            }
        }

        public bool Available {
            get { return FileFactory.NewForUri (root).Exists; }
        }

        public event EventHandler AvailabilityChanged;

        public IEnumerable<IPhoto> Photos {
            get {
                if (!Available)
                    throw new Exception ("Not available!");
                
                return from f in new RecursiveFileEnumerator (root)
                    where IsPhoto (f)
                    select new LocalFilePhoto (f.Uri) as IPhoto;
            }
        }

        bool IsPhoto (File f)
        {
            // TODO: Generalize and possibly expand to mime-type detection.
            return f.Basename.EndsWith (".jpg", StringComparison.InvariantCultureIgnoreCase);
        }


        public void WakeUp ()
        {
            var parameters = parameter_provider.FetchFirstMatching ("CacheId = ?", CacheId);
            root = new Uri (parameters.RootUri);
        }

        public void Persist ()
        {
            Hyena.Log.Debug ("Storing folder source");
            var parameters = new LocalFolderPhotoSourceParameters { CacheId = CacheId, RootUri = root.ToString () };
            parameter_provider.Save (parameters, true);
        }

        public void Start (ICachingPhotoSource cache)
        {
            Hyena.Log.DebugFormat ("Starting folder source: {0}", root.ToString ());

            Core.Scheduler.Add (new RescanLocalFolderJob () { Source = this, Cache = cache });
            // TODO: Find files that need to be added (the ones that aren't in there already)
            // TODO: Do active monitoring
        }

        public void RegisterCachedPhoto (IPhoto photo, int cache_id)
        {
            // Make sure we can find back the original file if the main cache requests it. Here this is easy: just take
            // the Uri. In sources such as Flickr, this would be the photo id.
            var uri = new LocalFolderPhotoSourceUris { CacheId = cache_id, PhotoUri = photo.Uri.ToString () };
            uri_provider.Save (uri, true);
        }

        public IPhoto LookupCachedPhoto (int cache_id)
        {
            var uri = uri_provider.FetchFirstMatching ("CacheId = ?", cache_id);
            if (uri == null) {
                throw new Exception ("Possibly invalid cache id given, serious bug!");
            }

            return new LocalFilePhoto (new Uri(uri.PhotoUri));
        }


        private class LocalFolderPhotoSourceParameters
        {
            [DatabaseColumn(Constraints = DatabaseColumnConstraints.PrimaryKey)]
            public int CacheId { get; set; }

            [DatabaseColumn]
            public string RootUri { get; set; }
        }

        private class LocalFolderPhotoSourceUris
        {
            [DatabaseColumn(Constraints = DatabaseColumnConstraints.PrimaryKey)]
            public int CacheId { get; set; }

            [DatabaseColumn(Constraints = DatabaseColumnConstraints.Unique)]
            public string PhotoUri { get; set; }
        }

        private class RescanLocalFolderJob : SimpleAsyncJob {
            public LocalFolderPhotoSource Source { get; set; }
            public ICachingPhotoSource Cache { get; set; }

            protected override void Run ()
            {
                // TODO: This can be a ton smarter
                Hyena.Log.Information ("Rescanning database.");
                foreach (var photo in Source.Photos) {
                    if (uri_provider.FetchFirstMatching ("PhotoUri = ?", photo.Uri.ToString ()) == null) {
                        Hyena.Log.DebugFormat ("Registering {0}", photo.Uri.ToString ());
                        Cache.RegisterPhoto (Source, photo);
                    }

                    System.Threading.Thread.Sleep (1); // Sleep for a short while.
                }

                OnFinished ();
            }
        }
    }
}

