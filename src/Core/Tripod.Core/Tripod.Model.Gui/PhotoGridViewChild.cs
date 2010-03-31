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

        private static Stage<PhotoGridViewChild> stage;

        static PhotoGridViewChild ()
        {
            stage = new Stage<PhotoGridViewChild> (80);
            stage.ActorStep += actor => {
                var scale = actor.Target.mouse_over_scale;
                scale += actor.Target.mouse_over_scale_up
                    ? actor.StepDeltaPercent
                    : -actor.StepDeltaPercent;
                actor.Target.mouse_over_scale = scale = Math.Max (0.0, Math.Min (1.0, scale));
                actor.Target.InvalidateThumbnail ();
                return scale > 0 && scale < 1;
            };
        }

#region Public Layout Properties

        public double ThumbnailWidth { get; set; }
        public double ThumbnailHeight { get; set; }
        public double CaptionSpacing { get; set; }
        public double SelectedScale { get; set; }
        public double MouseOverScale { get; set; }

#endregion


#region Private Layout Values

        private bool valid;
        private Rect inner_allocation;
        private Rect thumbnail_allocation;
        private Rect caption_allocation;
        //private string caption;
        //private double appear_opacity = 1.0;

        private bool mouse_over_scale_up;
        private double mouse_over_scale;

#endregion


#region Constructors

        public PhotoGridViewChild ()
        {
            Padding = new Thickness (5);
            ThumbnailWidth = 200;
            ThumbnailHeight = 150;
            CaptionSpacing = 5;
            SelectedScale = 6;
            MouseOverScale = 6;
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
            bool fill, Color fillColor, CairoCorners corners, double scale)
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

        public override void Render (CellContext context)
        {
            if (inner_allocation.IsEmpty || ! valid)
                return;

            context.Context.Translate (inner_allocation.X, inner_allocation.Y);

            IPhoto photo = BoundObject as IPhoto;

            var view_layout = ParentLayout as PhotoGridViewLayout;
            ImageSurface image_surface = view_layout.ThumbnailCache.LookupCachedThumbnail (photo);

            if (image_surface == null)
                view_layout.ThumbnailCache.RequestThumbnail (photo);

            double increase = 0;

            if (context.State == (StateType.Active | StateType.Selected))
                increase = SelectedScale;

            increase = Math.Max (increase, MouseOverScale * mouse_over_scale * mouse_over_scale);

            double max_size = Math.Max (ThumbnailHeight, ThumbnailWidth);
            double scale = (max_size + increase) / max_size;

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

            //caption = String.Format ("{0} {1}", photo.DateTaken.ToShortDateString (), photo.DateTaken.ToLongTimeString ());

            double width = ThumbnailWidth + SelectedScale;
            double height = ThumbnailHeight + SelectedScale;

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

            double width = ThumbnailWidth + SelectedScale + Padding.X;
            double height = ThumbnailHeight + SelectedScale + CaptionSpacing + caption_allocation.Height + Padding.Y;

            return new Size (Math.Round (width), Math.Round (height));
        }

        public override void CursorEnterEvent ()
        {
            mouse_over_scale_up = true;
            stage.AddOrReset (this);
        }

        public override void CursorLeaveEvent ()
        {
            mouse_over_scale_up = false;
            stage.AddOrReset (this);
        }

#endregion

    }
}
