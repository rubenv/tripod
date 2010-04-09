//
// MipMappedPhotoLoader.cs
// 
// Author:
//   Ruben Vermeersch <ruben@savanne.be>
// 
// Copyright (c) 2010 Ruben Vermeersch
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

using Gdk;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tripod.Model;
using Tripod.Tasks;

namespace Tripod.Graphics
{
    /// <summary>
    ///     Photo loader that uses locally cached mipmaps.
    /// </summary>
    public class MipMappedPhotoLoader : IPhotoLoader
    {
        IPhoto Photo { get; set; }

        RefCountCancellableTask<MipMapFile> MipMapLoader { get; set; }
        CancellationTokenSource MipMapLoaderTokenSource { get; set; }

        public MipMappedPhotoLoader (IPhoto photo)
        {
            Photo = photo;
        }

        public CancellableTask<Pixbuf> FindBestPreview (int width, int height)
        {
            EnsureMipMap ();
            var source = new CancellationTokenSource ();
            var task = new ChildCancellableTask<MipMapFile, Pixbuf> (MipMapLoader, () => {
                source.Token.ThrowIfCancellationRequested ();

                return MipMapLoader.Result.FindBest (width, height);
            }, source);
            MipMapLoader.Request ();
            task.Start ();
            return task;
        }

        // Fire up a mipmap loader if needed.
        void EnsureMipMap ()
        {
            if (MipMapLoader != null)
                return;
            
            lock (this) {
                if (MipMapLoader == null) {
                    MipMapLoaderTokenSource = new CancellationTokenSource ();
                    MipMapLoader = new RefCountCancellableTask<MipMapFile> (() => {
                            MipMapLoaderTokenSource.Token.ThrowIfCancellationRequested ();
                            return MipMapGenerator.LoadMipMap (Photo);
                    }, MipMapLoaderTokenSource);
                    MipMapLoader.Start ();
                }
            }
        }

        public Task<bool> IsBestPreview (int have_width, int have_height, int desired_width, int desired_height)
        {
            EnsureMipMap ();
            var source = new CancellationTokenSource ();
            var task = new ChildCancellableTask<MipMapFile, bool> (MipMapLoader, () => {
                source.Token.ThrowIfCancellationRequested ();

                return MipMapLoader.Result.IsBestSize (have_width, have_height, desired_width, desired_height);
            }, source);
            task.Start ();
            return task;
        }
    }
}
