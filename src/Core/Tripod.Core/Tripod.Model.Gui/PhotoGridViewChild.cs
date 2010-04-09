// 
// PhotoGridViewChild.cs
// 
// Author:
//   Mike Gemuende <mike@gemuende.de>
//   Ruben Vermeersch <ruben@savanne.be>
// 
// Copyright (c) 2010 Mike Gemuende
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

using System;

using Tripod.Base;
using Tripod.Model;
using Tripod.Tasks;
using Tripod.Graphics;

using Hyena;
using Hyena.Gui;
using Hyena.Data.Gui;
using Hyena.Gui.Theming;
using Hyena.Gui.Theatrics;
using Hyena.Gui.Canvas;

using Gtk;
using Cairo;


namespace Tripod.Model.Gui
{


    public class PhotoGridViewChild : DataViewChild
    {

        static PhotoGridViewChild ()
        {

        }

#region Public Layout Properties

        public int ThumbnailWidth { get { return (ParentLayout as PhotoGridViewLayout).ThumbnailWidth; } }
        public int ThumbnailHeight { get { return (ParentLayout as PhotoGridViewLayout).ThumbnailHeight; }  }
        public double CaptionSpacing { get; set; }

#endregion


#region Private Layout Values

        private bool valid;
        private Rect inner_allocation;
        private Rect thumbnail_allocation;
        private Rect caption_allocation;

#endregion


#region Constructors

        public PhotoGridViewChild ()
        {
            Padding = new Thickness (5);
            CaptionSpacing = 10;
        }

#endregion


#region Public Methods

        public void InvalidateThumbnail ()
        {
            Invalidate (inner_allocation);
        }

        public IPhoto BoundPhoto {
            get { return BoundObject as IPhoto; }
        }

#endregion

        public static void RenderThumbnail (Cairo.Context cr, ImageSurface image, bool dispose,
            double x, double y, double width, double height, double radius,
            bool fill, Cairo.Color fillColor, CairoCorners corners, double scale)
        {
            if (image == null || image.Handle == IntPtr.Zero) {
                image = null;
            }

            double p_x = x;
            double p_y = y;

            if (image != null) {
                double scaled_image_width = scale * image.Width;
                double scaled_image_height = scale * image.Height;

                p_x += (scaled_image_width < width ? (width - scaled_image_width) / 2 : 0) / scale;
                p_y += (scaled_image_height < height ? (height - scaled_image_height) / 2 : 0) / scale;
            }

            cr.Antialias = Cairo.Antialias.Default;

            if (image != null) {
                if (fill) {
                    CairoExtensions.RoundedRectangle (cr, x, y, width, height, radius, corners);
                    cr.Color = fillColor;
                    cr.Fill ();
                }

                cr.Scale (scale, scale);
                CairoExtensions.RoundedRectangle (cr, p_x, p_y, image.Width, image.Height, radius, corners);
                cr.SetSource (image, p_x, p_y);
                cr.Fill ();
                cr.Scale (1.0/scale, 1.0/scale);
            } else {
                CairoExtensions.RoundedRectangle (cr, x, y, width, height, radius, corners);

                if (fill) {
                    var grad = new LinearGradient (x, y, x, y + height);
                    grad.AddColorStop (0, fillColor);
                    grad.AddColorStop (1, CairoExtensions.ColorShade (fillColor, 1.3));
                    cr.Pattern = grad;
                    cr.Fill ();
                    grad.Destroy ();
                }
            }

            cr.Stroke ();

            if (dispose && image != null) {
                ((IDisposable)image).Dispose ();
            }
        }

#region DataViewChild Implementation

        CancellableTask<Gdk.Pixbuf> last_loader_task = null;
        IPhoto last_photo;

        void Reload (IPhoto photo)
        {
            if (last_loader_task != null) {
                // last_loader_task.Cancel (); // FIXME: Re-enable this!
                if (last_loader_task.IsCompleted) {
                    last_loader_task.Result.Dispose ();
                }
            }

            var loader = Core.PhotoLoaderCache.RequestLoader (photo);
            last_loader_task = loader.FindBestPreview (ThumbnailWidth, ThumbnailHeight);
            last_loader_task.ContinueWith ((t) => {
                if (t.IsCanceled)
                    return;

                var new_surface = PixbufImageSurface.Create (last_loader_task.Result);
                var cache = (ParentLayout as PhotoGridViewLayout).SurfaceCache;

                ThreadAssist.ProxyToMain (() => {
                    ImageSurface old_surface = null;
                    if (cache.TryGetValue (photo, out old_surface)) {
                        if (old_surface != null)
                            old_surface.Dispose ();
                    }
                    cache[photo] = new_surface;
                    (ParentLayout.View as PhotoGridView).InvalidateThumbnail (photo);
                });
            });
        }

        bool finding_larger = false;

        void ReloadIfBetterSizeAvailable (IPhoto photo, int have_width, int have_height) {
            if (finding_larger)
                return;
            finding_larger = true;

            var loader = Core.PhotoLoaderCache.RequestLoader (photo);
            var task = loader.IsBestPreview (have_width, have_height, ThumbnailWidth, ThumbnailHeight);
            task.ContinueWith ((t) => {
                if (!t.Result) {
                    Reload (photo);
                }
                finding_larger = false;
            });
        }

        void ReloadIfSuboptimalSize (IPhoto photo) {
            if (photo == null || finding_larger)
                return;

            ImageSurface image_surface = null;
            if (!(ParentLayout as PhotoGridViewLayout).SurfaceCache.TryGetValue (photo, out image_surface))
                return;

            var surface_w = image_surface.Width;
            var surface_h = image_surface.Height;
            var alloc_w = thumbnail_allocation.Width;
            var alloc_h = thumbnail_allocation.Height;

            // Make sure we have the most optimal surface size.
            bool too_small = surface_w < alloc_w && surface_h < alloc_h;
            if (too_small) {
                // Thumbnail never touches the edges.
                ReloadIfBetterSizeAvailable (photo, surface_w, surface_h);
            } else {
                // Downscale if we're twice too large on the longest edge.
                bool wider_than_high = surface_w > surface_h;
                bool too_wide = surface_w > 2 * alloc_w;
                bool too_high = surface_h > 2 * alloc_h;
                if ((wider_than_high && too_wide) || too_high) {
                    ReloadIfBetterSizeAvailable (photo, surface_w, surface_h);
                }
            }
        }

        public override void Render (CellContext context)
        {
            if (inner_allocation.IsEmpty || ! valid)
                return;

            context.Context.Translate (inner_allocation.X, inner_allocation.Y);

            var photo = BoundPhoto;
            var view_layout = ParentLayout as PhotoGridViewLayout;

            if (photo != last_photo) {
                last_photo = photo;

                Reload (photo);
            }

            view_layout.CaptionRender.Render (context, caption_allocation, photo);

            ThreadAssist.BlockingProxyToMain (() => {
                RenderThumbnail (photo, context);
                ReloadIfSuboptimalSize (photo);
            });
        }

        void RenderThumbnail (IPhoto photo, CellContext context)
        {
            ImageSurface image_surface = null;
            if (!(ParentLayout as PhotoGridViewLayout).SurfaceCache.TryGetValue (photo, out image_surface))
                return;

            double scalex = Math.Max (1.0, image_surface.Width / thumbnail_allocation.Width);
            double scaley = Math.Max (1.0, image_surface.Height / thumbnail_allocation.Height);
            double scale = 1 / Math.Max (scalex, scaley);

            RenderThumbnail (context.Context,
                             image_surface,
                             false,
                             0.0,
                             0.0,
                             thumbnail_allocation.Width,
                             thumbnail_allocation.Height,
                             context.Theme.Context.Radius,
                             false,
                             new Color (0.8, 0.0, 0.0),
                             CairoCorners.All, scale);
        }

        public override void Arrange ()
        {
            if (BoundObject == null) {
                valid = false;
                return;
            }

            IPhoto photo = BoundObject as IPhoto;

            if (photo == null)
                throw new InvalidCastException ("PhotoGridViewChild can only bind IPhoto objects");

            valid = true;

            inner_allocation = new Rect () {
                X = Padding.Left,
                Y = Padding.Top,
                Width = Allocation.Width - Padding.X,
                Height = Allocation.Height - Padding.Y
            };

            thumbnail_allocation = new Rect () {
                Width = ThumbnailWidth,
                Height = ThumbnailHeight,
                X = 0,
                Y = 0
            };

            caption_allocation.Y = thumbnail_allocation.Height + CaptionSpacing;
            caption_allocation.Width = inner_allocation.Width;
        }

        public override Size Measure (Size available)
        {
            var layout = ParentLayout as PhotoGridViewLayout;
            caption_allocation.Height = layout.CaptionRender.MeasureHeight (ParentLayout.View);

            double width = ThumbnailWidth + Padding.X;
            double height = ThumbnailHeight + CaptionSpacing + caption_allocation.Height + Padding.Y;

            return new Size (Math.Round (width), Math.Round (height));
        }

#endregion

    }
}
