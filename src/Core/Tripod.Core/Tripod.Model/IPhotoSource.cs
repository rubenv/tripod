// 
// IPhotoSource.cs
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

namespace Tripod.Model
{
    public interface IPhotoSource
    {
        // The id by which this source is stored in the cache.
        int CacheId { get; set; }

        // Called when the source is woken up by the main cache.
        void WakeUp ();

        // Called when the source is added to the main cache.
        void Save ();

        // Start this source (which means that it should sync with the main cache).
        void Start (ICachePhotoSource cache);

        string DisplayName { get; }
        bool Available { get; }
        IEnumerable<IPhoto> Photos { get; }

        /// <summary>
        /// Copy the given photo into this storage source, if possible.
        /// </summary>
        /// <param name="photo">
        /// A <see cref="IPhoto"/> that should be copied into the source.
        /// </param>
        //void CopyIntoSource (IPhoto photo);
    }
}

