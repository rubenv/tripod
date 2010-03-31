// 
// PhotoGridTextCaptionRenderer.cs
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

using Gtk;

using Hyena.Data.Gui;
using Hyena.Gui.Canvas;
using Hyena.Gui;
using Hyena.Gui.Theming;

using Tripod.Model;


namespace Tripod.Model.Gui
{


    public abstract class PhotoGridTextCaptionRenderer : IPhotoGridCaptionRenderer
    {

        protected abstract string GetCaptionString (IPhoto photo);

        public double MeasureHeight (Widget widget)
        {
            var fd = widget.PangoContext.FontDescription;

            return fd.MeasureTextHeight (widget.PangoContext);
        }

        public void Render (CellContext context, Rect allocation, IPhoto photo)
        {
            string caption = GetCaptionString (photo);

            var text_color = context.Theme.Colors.GetWidgetColor (GtkColorClass.Text, context.State);

            var layout = context.Layout;
            layout.Ellipsize = Pango.EllipsizeMode.End;
            layout.Alignment = Pango.Alignment.Center;
            layout.Width = (int)(allocation.Width * Pango.Scale.PangoScale);

            layout.SetText (caption);

            context.Context.Color = text_color;
            context.Context.MoveTo (allocation.X, allocation.Y);
            PangoCairoHelper.ShowLayout (context.Context, layout);
        }
    }
}
