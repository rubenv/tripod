// 
// PixbufExtensions.cs
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

using Gdk;
using TagLib.Image;

namespace Tripod.Graphics
{
    public static class PixbufExtensions
    {
        /// <summary>
        /// Returns a new <see cref="Pixbuf"/>, with correct orientation.
        /// </summary>
        /// <param name="pixbuf">
        /// A <see cref="Pixbuf"/> to transform.
        /// </param>
        /// <param name="orientation">
        /// A <see cref="ImageOrientation"/> to correct the pixbuf.
        /// </param>
        /// <returns>
        /// A <see cref="Pixbuf"/> that is transformed to the correct orientation. This can be a different pixbuf than
        /// the one that was supplied as an argument (in this case the supplied pixbuf has been disposed).
        /// </returns>
        public static Pixbuf TransformOrientation (this Pixbuf pixbuf, ImageOrientation orientation)
        {
            switch (orientation) {
            case ImageOrientation.TopLeft:
                break;
            case ImageOrientation.TopRight:
                using (var tmp = pixbuf)
                    pixbuf = tmp.Flip (false);
                break;
            case ImageOrientation.BottomRight:
                using (var tmp = pixbuf)
                    pixbuf = tmp.RotateSimple (PixbufRotation.Upsidedown);
                break;
            case ImageOrientation.BottomLeft:
                using (var tmp = pixbuf)
                    pixbuf = tmp.Flip (true);
                break;
            case ImageOrientation.LeftTop:
                using (var tmp = pixbuf)
                    pixbuf = tmp.Flip (true);
                using (var tmp = pixbuf)
                    pixbuf = tmp.RotateSimple (PixbufRotation.Clockwise);
                break;
            case ImageOrientation.RightTop:
                using (var tmp = pixbuf)
                    pixbuf = tmp.RotateSimple (PixbufRotation.Clockwise);
                break;
            case ImageOrientation.RightBottom:
                using (var tmp = pixbuf)
                    pixbuf = tmp.Flip (false);
                using (var tmp = pixbuf)
                    pixbuf = tmp.RotateSimple (PixbufRotation.Clockwise);
                break;
            case ImageOrientation.LeftBottom:
                using (var tmp = pixbuf)
                    pixbuf = tmp.RotateSimple (PixbufRotation.Counterclockwise);
                break;
            }
            return pixbuf;
        }

        /// <summary>
        /// Returns a new <see cref="Pixbuf"/> which is scaled down to fit the given size.
        /// The original aspect ratio is preserved.
        /// </summary>
        /// <param name="pixbuf">
        /// A <see cref="Pixbuf"/> to scale down.
        /// </param>
        /// <param name="max_target_width">
        /// A <see cref="System.Int32"/> (greater than 0) with the width to fit in.
        /// </param>
        /// <param name="max_target_height">
        /// A <see cref="System.Int32"/> (greater than 0) with the height to fit in.
        /// </param>
        /// <param name="interp_type">
        /// A <see cref="InterpType"/> with the interpolation type).
        /// </param>
        /// <returns>
        /// A <see cref="Pixbuf"/> which is scaled down to fit into given size.
        /// </returns>
        public static Pixbuf ShrinkToFit (this Pixbuf pixbuf, int max_target_width, int max_target_height, InterpType interp_type)
        {
            int pixbuf_width = pixbuf.Width;
            int pixbuf_height = pixbuf.Height;

            if (max_target_width <= 0)
                throw new ArgumentOutOfRangeException ("max_target_width > 0 must hold");

            if (max_target_height <= 0)
                throw new ArgumentOutOfRangeException ("max_target_height > 0 must hold");

            if (pixbuf_width <= max_target_width && pixbuf_height <= max_target_height)
                return pixbuf.Copy ();

            double width_scale = (double) max_target_width / (double) pixbuf_width;
            double height_scale = (double) max_target_height / (double) pixbuf_height;

            double scale = Math.Min (width_scale, height_scale);

            return pixbuf.ScaleSimple ((int) (pixbuf_width * scale), (int) (pixbuf_height * scale), interp_type);
        }
    }
}

