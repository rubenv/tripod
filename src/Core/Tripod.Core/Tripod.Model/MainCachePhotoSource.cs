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
using System.Linq;
using System.Collections.Generic;
using Hyena.Data.Sqlite;
using Tripod.Base;

namespace Tripod.Model
{
    public class MainCachePhotoSource : IPhotoSource
    {
        SqliteModelProvider<CachePhoto> provider = new SqliteModelProvider<CachePhoto>(Core.DbConnection, "CachedPhotos");

        public string DisplayName {
            get {
                return "Main Cache";
            }
        }

        public bool Available {
            get {
                return true;
            }
        }

        public IEnumerable<IPhoto> Photos {
            get {
                return from p in RegisteredPhotos select p as IPhoto;
            }
        }

        public List<CachePhoto> RegisteredPhotos {
            get; private set;
        }

        public void RegisterPhoto (IPhoto photo) {
            if (RegisteredPhotos == null)
                RegisteredPhotos = new List<CachePhoto>();

            var source = CachePhotoSourceFactory.GetForPhotoSource(photo.Source);
            var cached_photo = new CachePhoto () {
                Uri = photo.Uri,
                Source = source,
                SourceId = source.Id
            };

            provider.Save(cached_photo);

            RegisteredPhotos.Add (cached_photo);
        }
    }
}

