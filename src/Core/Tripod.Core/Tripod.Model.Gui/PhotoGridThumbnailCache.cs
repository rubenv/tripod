// 
// PhotoGridThumbnailCache.cs
// 
// Author:
//   Mike Gemuende <mike@gemuende.de>
// 
// Copyright (c) 2010 Mike Gemuende
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

using Hyena.Collections;
using Hyena.Gui;

using Gdk;
using Cairo;

using Tripod.Base;
using Tripod.Model;
using Tripod.Jobs;
using Tripod.Graphics;


namespace Tripod.Model.Gui
{


    public class PhotoGridThumbnailCache {

        private LruCache<string, ImageSurface> cache;
        private PhotoGridViewLayout view_layout;
        private int thumbnail_width;
        private int thumbnail_height;

        public PhotoGridThumbnailCache (PhotoGridViewLayout view_layout, int thumbnail_width, int thumbnail_height)
        {
            this.view_layout = view_layout;
            this.thumbnail_width = thumbnail_width;
            this.thumbnail_height = thumbnail_height;
            cache = new LruCache<string, ImageSurface> (512);
        }


        public ImageSurface LookupCachedThumbnail (IPhoto photo)
        {
            if (photo == null)
                return null;

            string uri_string = photo.Uri.AbsoluteUri;
            ImageSurface image_surface;

            if (cache.TryGetValue (uri_string, out image_surface))
                return image_surface;

            return null;
        }

        private int max_running = 1;
        private LinkedList<IPhoto> requested = new LinkedList<IPhoto> ();
        private Dictionary<IPhoto, LoadThumbnailJob> jobs = new Dictionary<IPhoto, LoadThumbnailJob> ();

        private void UpdateRunning ()
        {
            if (jobs.Count >= max_running)
                return;

            if (requested.Count == 0)
                return;

            IPhoto next = requested.First.Value;
            requested.RemoveFirst ();
            var job = new LoadThumbnailJob (this, next);
            Core.Scheduler.Add (job);
            jobs.Add (next, job);
        }

        public void RequestThumbnail (IPhoto photo)
        {
            lock (jobs) {
                if (! jobs.ContainsKey (photo)) {

                    // Move requested photo to the first position. Therewith,
                    // the visible thumbnails are created first.
                    requested.Remove (photo);
                    requested.AddFirst (photo);

                    UpdateRunning ();
                }
            }
        }

        protected void ThumbnailLoaded (IPhoto photo, ImageSurface thumbnail)
        {
            cache.Add (photo.Uri.AbsoluteUri, thumbnail);
            view_layout.InvalidateThumbnail (photo);

            lock (jobs) {
                jobs.Remove (photo);
                UpdateRunning ();
            }
        }

        private class LoadThumbnailJob : ThreadPoolJob
        {
            private static int running_count = 0;
            private static object sync = new object ();

            PhotoGridThumbnailCache parent;
            IPhoto photo;
            int width;
            int height;

            public LoadThumbnailJob (PhotoGridThumbnailCache parent, IPhoto photo)
            {
                this.parent = parent;
                this.photo = photo;
                this.width = parent.thumbnail_width;
                this.height = parent.thumbnail_height;
            }

            protected override void Run ()
            {
                lock (sync) {
                    running_count ++;
                    Hyena.Log.DebugFormat ("Running LoadThumbnailJob: {0}", running_count);
                }

                ImageSurface image_surface = LookupScaleThumbnail ();

                if (image_surface != null)
                    parent.ThumbnailLoaded (photo, image_surface);

                lock (sync) {
                    running_count --;
                }

                OnFinished ();
            }

            private ImageSurface LookupScaleThumbnail ()
            {
                if (photo == null)
                    return null;

                ImageSurface image_surface;

                Pixbuf pixbuf = null;
                Pixbuf scaled_pixbuf = null;
                try {
                    pixbuf = MipMapGenerator.LoadMipMap (photo).FindBest (width, height);
                    scaled_pixbuf = pixbuf.ShrinkToFit (width, height, InterpType.Bilinear);

                    image_surface =  PixbufImageSurface.Create (scaled_pixbuf);
                } finally {
                    DisposePixbuf (pixbuf);
                    DisposePixbuf (scaled_pixbuf);
                }

                return image_surface;
            }


            //
            //  Copied from Banshee
            //
            private static int dispose_count = 0;
            public static void DisposePixbuf (Pixbuf pixbuf)
            {
                if (pixbuf != null && pixbuf.Handle != IntPtr.Zero) {
                    pixbuf.Dispose ();
                    pixbuf = null;

                    // There is an issue with disposing Pixbufs where we need to explicitly
                    // call the GC otherwise it doesn't get done in a timely way.  But if we
                    // do it every time, it slows things down a lot; so only do it every 100th.
                    if (++dispose_count % 100 == 0) {
                        System.GC.Collect ();
                        dispose_count = 0;
                    }
                }
            }

        }
    }
}
