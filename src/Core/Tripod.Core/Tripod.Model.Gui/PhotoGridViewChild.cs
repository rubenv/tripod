// 
// PhotoGridViewChild.cs
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
            CaptionSpacing = 5;
        }

#endregion


#region Public Methods

        public void InvalidateThumbnail ()
        {
            var invalidate_allocation = thumbnail_allocation;
            invalidate_allocation.Offset (inner_allocation);
            Invalidate (invalidate_allocation);
        }

        public IPhoto BoundPhoto {
            get { return BoundObject as IPhoto; }
        }

#endregion


        enum AlignThumbnail {
            Begin, End
        };

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

                p_x += (scaled_image_width < width ? (width - scaled_image_width) / 2 : 0);
                p_y += (scaled_image_height < height ? (height - scaled_image_height) / 2 : 0);
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

                /*Banshee.CairoGlyphs.BansheeLineLogo.Render (cr,
                    new Rectangle (x + 15, y + 15, width - 30, height - 30),
                    CairoExtensions.RgbaToColor (0x00000044),
                    CairoExtensions.RgbaToColor (0x00000055));*/
            }

            cr.Stroke ();

            if (dispose && image != null) {
                ((IDisposable)image).Dispose ();
            }
        }

#region DataViewChild Implementation

        CancellableTask<Gdk.Pixbuf> last_loader_task = null;
        ImageSurface image_surface = null;
        IPhoto last_photo;

        void Reload (IPhoto photo)
        {
            if (last_loader_task != null) {
                last_loader_task.Cancel ();
                if (last_loader_task.IsCompleted) {
                    last_loader_task.Result.Dispose ();
                }
            }

            var loader = Core.PhotoLoaderCache.RequestLoader (photo);
            last_loader_task = loader.FindBestPreview (ThumbnailWidth, ThumbnailHeight);
            last_loader_task.ContinueWith ((t) => {

                lock (this) {
                    if (image_surface != null)
                        image_surface.Dispose ();
                    image_surface = PixbufImageSurface.Create (last_loader_task.Result);
                }


                ThreadAssist.ProxyToMain (() => {
                    (ParentLayout.View as PhotoGridView).InvalidateThumbnail (photo);
                });
            });
        }

        public override void Render (CellContext context)
        {
            if (inner_allocation.IsEmpty || ! valid)
                return;

            context.Context.Translate (inner_allocation.X, inner_allocation.Y);

            IPhoto photo = BoundObject as IPhoto;
            var view_layout = ParentLayout as PhotoGridViewLayout;

            if (photo != last_photo) {
                last_photo = photo;

                Reload (photo);
            }

            lock (this) {
                if (image_surface != null) {
                    double scalex = Math.Max (1.0, image_surface.Width / thumbnail_allocation.Width);
                    double scaley = Math.Max (1.0, image_surface.Height / thumbnail_allocation.Height);
                    double scale = 1 / Math.Max (scalex, scaley);
    
                    RenderThumbnail (context.Context,
                                     image_surface,
                                     false,
                                     thumbnail_allocation.X,
                                     thumbnail_allocation.Y,
                                     thumbnail_allocation.Width,
                                     thumbnail_allocation.Height,
                                     context.Theme.Context.Radius,
                                     false,
                                     new Color (0.8, 0.8, 0.8),
                                     CairoCorners.All, scale);
                }
            }

            view_layout.CaptionRender.Render (context, caption_allocation, photo);

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

            double width = ThumbnailWidth;
            double height = ThumbnailHeight;

            inner_allocation = new Rect () {
                X = Padding.Left,
                Y = Padding.Top,
                Width = Allocation.Width - Padding.X,
                Height = Allocation.Height - Padding.Y
            };

            thumbnail_allocation = new Rect () {
                Width = Math.Min (inner_allocation.Width, width),
                Height = Math.Min (inner_allocation.Height, height)
            };

            thumbnail_allocation.X = Math.Round ((inner_allocation.Width - width) / 2.0);

            caption_allocation.Y = thumbnail_allocation.Height + CaptionSpacing;
            caption_allocation.Width = inner_allocation.Width;
        }

        public override Size Measure (Size available)
        {
            caption_allocation.Height =
                (ParentLayout as PhotoGridViewLayout).CaptionRender.MeasureHeight (ParentLayout.View);

            double width = ThumbnailWidth + Padding.X;
            double height = ThumbnailHeight + CaptionSpacing + caption_allocation.Height + Padding.Y;

            return new Size (Math.Round (width), Math.Round (height));
        }

#endregion

    }
}
