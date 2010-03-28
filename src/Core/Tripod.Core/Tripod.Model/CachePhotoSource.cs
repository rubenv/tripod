//
// CachePhotoSource.cs
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

using System;
using System.Linq;
using System.Collections.Generic;
using Hyena;
using Hyena.Jobs;
using Hyena.Data.Sqlite;
using Tripod.Base;

namespace Tripod.Model
{
    public class CachePhotoSource : IPhotoSource
    {
        public CachePhotoSource ()
        {
        }

        public CachePhotoSource (ICacheablePhotoSource source)
        {
            instance = source;
            instance.AvailabilityChanged += (s, a) => UpdateAvailability ();
            SourceType = instance.GetType ().FullName;
        }

        [DatabaseColumn(Constraints = DatabaseColumnConstraints.PrimaryKey)]
        public int CacheId { get; set; }

        [DatabaseColumn]
        public string SourceType { get; set; }

        public string DisplayName {
            get {
                EnsureInstance ();
                return instance.DisplayName;
            }
        }

        // This is tracked in the database for fast joining with IPhoto, to
        // make sure we can quickly filter on available photos.
        [DatabaseColumn]
        public bool Available {
            get {
                EnsureInstance ();
                return instance.Available;
            }
            set { /* Nothing to do */ }
        }


        public event EventHandler AvailabilityChanged;

        public IEnumerable<IPhoto> Photos {
            get {
                EnsureInstance ();
                return instance.Photos;
            }
        }

        ICacheablePhotoSource instance;
        void EnsureInstance ()
        {
            lock (this) {
                if (instance == null) {
                    var type = Type.GetType (SourceType);
                    instance = Activator.CreateInstance (type) as ICacheablePhotoSource;
                    instance.CacheId = CacheId;
                    instance.WakeUp ();
                    
                    instance.AvailabilityChanged += (s, a) => UpdateAvailability ();
                }
            }
        }

        bool available = false;
        void UpdateAvailability ()
        {
            bool new_available = instance.Available;
            
            if (new_available == available)
                return;
            
            var h = AvailabilityChanged;
            if (h != null)
                h (this, null);
            
        }

        public void Start (ICachingPhotoSource cache)
        {
            Core.Scheduler.Add (new StartPhotoSourceJob (this, cache));
        }


        private sealed class StartPhotoSourceJob : SimpleAsyncJob
        {
            internal StartPhotoSourceJob (CachePhotoSource source, ICachingPhotoSource cache)
            {
                Source = source;
                Cache = cache;
                Title = String.Format ("Starting cached source: {0}/{1}", Source.SourceType, Source.CacheId);
            }

            CachePhotoSource Source { get; set; }
            ICachingPhotoSource Cache { get; set; }

            protected override void Run ()
            {
                Source.EnsureInstance ();
                Source.instance.Start (Cache);
                
                // Make sure we send out the event on start if different from
                // the database value. The value in the database can be stale.
                Source.UpdateAvailability ();
                
                OnFinished ();
            }
        }
    }
}

