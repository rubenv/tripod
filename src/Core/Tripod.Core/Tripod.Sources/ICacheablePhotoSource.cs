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
    /// <summary>
    /// A photo source which can be persisted between multiple runs of the application and which should be cached in
    /// the main cache.
    /// </summary>
    public interface ICacheablePhotoSource : IPhotoSource
    {
        /// <summary>
        /// The id by which this source is stored in the cache. Should not be managed manually.
        /// </summary>
        int CacheId { get; set; }

        /// <summary>
        /// Called when the source is woken up by the main cache. This usually happens when starting the program.
        /// Retrieve source parameters with this method and restore state that's used to operate the source.
        /// </summary>
        void WakeUp ();

        /// <summary>
        /// Called when the source is added to the main cache. When this is called, you should make sure that the
        /// source can still be used (and updated) when the application is restarted.
        /// </summary>
        void Persist ();

        /// <summary>
        /// Start this source (which means that it should sync with the main cache).
        /// </summary>
        /// <param name="cache">
        /// An <see cref="ICachingPhotoSource"/>, with which the source should sync. For example, it could mean removing
        /// cached photos that are no longer available in the source.
        /// </param>
        void Start (ICachingPhotoSource cache);

        /// <summary>
        /// Callback from the main cache, to indicate that the given photo is cached with the given id.
        /// </summary>
        /// <param name="photo">
        /// An <see cref="IPhoto"/>, which has just been imported into the cache.
        /// </param>
        /// <param name="cache_id">
        /// A <see cref="System.Int32"/>, used to identify the photo. Based on this number, the source should later on
        /// (which could be after restarting the application) be able to retrieve the photo that belongs to it.
        /// </param>
        void RegisterCachedPhoto (IPhoto photo, int cache_id);

        /// <summary>
        /// Request the real instance of a previously cached photo. Used for writing back changes.
        /// </summary>
        /// <param name="cache_id">
        /// A <see cref="System.Int32"/>, this identifier was previously signalled to the source using
        /// <see cref="ICacheablePhotoSource#RegisterCachedPhoto (IPhoto, int)"/>.
        /// </param>
        /// <returns>
        /// An <see cref="IPhoto"/>.
        /// </returns>
        IPhoto LookupCachedPhoto (int cache_id);
    }
}

