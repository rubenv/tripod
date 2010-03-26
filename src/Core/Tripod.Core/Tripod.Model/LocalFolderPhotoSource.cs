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

namespace Tripod.Model
{
    public class LocalFolderPhotoSource : IPhotoSource
    {
        public int CacheId {
            get; set;
        }


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

        public IEnumerable<IPhoto> Photos {
            get {
                if (!Available)
                    throw new Exception ("Not available!");

                return from f in new RecursiveFileEnumerator (root)
                    where IsPhoto (f)
                    select new LocalFilePhoto (this, f.Uri) as IPhoto;
            }
        }

        bool IsPhoto (File f)
        {
            return f.Basename.EndsWith (".jpg", StringComparison.InvariantCultureIgnoreCase);
        }

        SqliteModelProvider<LocalFolderPhotoSourceParameters> parameter_provider = new SqliteModelProvider<LocalFolderPhotoSourceParameters> (Core.DbConnection, "LocalFolderSourceParameters");

        public void WakeUp ()
        {
            var parameters = parameter_provider.FetchFirstMatching ("CacheId = ?", CacheId);
            root = new Uri (parameters.RootUri);
        }

        public void Save ()
        {
            Hyena.Log.Debug ("Storing folder source");
            var parameters = new LocalFolderPhotoSourceParameters { CacheId = CacheId, RootUri = root.ToString () };
            parameter_provider.Save (parameters, true);
        }

        public void Start (ICachePhotoSource cache)
        {
            Hyena.Log.DebugFormat ("Starting folder source: {0}", root.ToString ());
            // TODO: Find files that need to be added (the ones that aren't in there already)
        }

        private class LocalFolderPhotoSourceParameters
        {
            [DatabaseColumn(Constraints = DatabaseColumnConstraints.PrimaryKey)]
            public int CacheId { get; set; }

            [DatabaseColumn]
            public string RootUri { get; set; }
        }
    }
}

