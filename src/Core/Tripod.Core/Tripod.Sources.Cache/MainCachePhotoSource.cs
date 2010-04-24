//
// MainCachePhotoSource.cs
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
using System.Collections.Generic;
using Hyena.Data.Sqlite;
using Tripod.Base;
using Tripod.Model;

namespace Tripod.Sources.Cache
{
    public class MainCachePhotoSource : ICachingPhotoSource
    {
        SqliteModelProvider<CachePhoto> provider = new SqliteModelProvider<CachePhoto> (Core.DbConnection, "CachedPhotos");
        SqliteModelProvider<CachePhotoSource> source_provider = new SqliteModelProvider<CachePhotoSource> (Core.DbConnection, "CachedPhotoSources");

        public string DisplayName {
            get { return "Main Cache"; }
        }

        public bool Available {
            get { return true; }
        }

        public event EventHandler AvailabilityChanged;

        public IEnumerable<IPhoto> Photos {
            get { return new TripodQuery<CachePhoto> (provider); }
        }

        IEnumerable<CachePhotoSource> CachedSources {
            get { return source_provider.FetchAll (); }
        }

        public void RegisterPhotoSource (ICacheablePhotoSource source)
        {
            if (source.CacheId != 0) {
                throw new Exception ("Can't register an already registered source!");
            }
            
            var cache = new CachePhotoSource (source);
            cache.AvailabilityChanged += OnCachedSourceAvailabilityChanged;
            source_provider.Save (cache);
            
            source.CacheId = cache.CacheId;
            source.Persist ();
            cache.Start (this);
        }

        public void RegisterPhoto (ICacheablePhotoSource source, IPhoto photo)
        {
            if (source.CacheId == 0) {
                throw new Exception ("The source needs to be registered first using RegisterPhotoSource ()");
            }

            var cache_photo = CachePhoto.CreateFrom (photo);
            cache_photo.SourceId = source.CacheId;

            provider.Save (cache_photo);

            source.RegisterCachedPhoto (photo, cache_photo.CacheId);
        }

        public void Start ()
        {
            foreach (var source in CachedSources) {
                source.AvailabilityChanged += OnCachedSourceAvailabilityChanged;
                source.Start (this);
            }
        }

        void OnCachedSourceAvailabilityChanged (object sender, EventArgs args)
        {
            source_provider.Save (sender as CachePhotoSource);
        }
    }
}