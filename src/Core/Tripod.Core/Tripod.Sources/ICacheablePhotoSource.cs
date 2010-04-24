// 
// ICacheablePhotoSource..cs
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

namespace Tripod.Sources
{
    public interface ICacheablePhotoSource : IPhotoSource
    {
        // The id by which this source is stored in the cache.
        int CacheId { get; set; }

        // Called when the source is woken up by the main cache.
        void WakeUp ();

        // Called when the source is added to the main cache.
        void Persist ();

        // Start this source (which means that it should sync with the main cache).
        void Start (ICachingPhotoSource cache);

        // Callback from the main cache, to indicate that the given photo is cached with the given id.
        void RegisterCachedPhoto (IPhoto photo, int cache_id);

        // Request the real instance of a previously cached photo. Used for writing back changes.
        IPhoto LookupCachedPhoto (int cache_id);
    }
}

