//
// DataViewLayout.cs
//
// Author:
//   Aaron Bockover <abockover@novell.com>
//
// Copyright 2010 Novell, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;

using Hyena.Data;
using Hyena.Gui.Canvas;

namespace Hyena.Data.Gui
{
    public abstract class DataViewLayout
    {
        private List<DataViewChild> children = new List<DataViewChild> ();
        protected List<DataViewChild> Children {
            get { return children; }
        }

        public IListModel Model { get; set; }
        public ListViewBase View { get; set; }

        public Rect ActualAllocation { get; protected set; }
        public Size VirtualSize { get; protected set; }
        public Size ChildSize { get; protected set; }
        public int XPosition { get; protected set; }
        public int YPosition { get; protected set; }

        public int ChildCount {
            get { return Children.Count; }
        }

        public DataViewChild this[int index] {
            get { return Children[index]; }
        }

        public void UpdatePosition (int x, int y)
        {
            XPosition = x;
            YPosition = y;
            InvalidateChildLayout ();
        }

        public void ModelUpdated ()
        {
            InvalidateVirtualSize ();
            InvalidateChildLayout ();
        }

        public virtual void Allocate (Rect actualAllocation)
        {
            ActualAllocation = actualAllocation;

            InvalidateChildSize ();
            InvalidateChildCollection ();
            InvalidateVirtualSize ();
            InvalidateChildLayout ();
        }

        public virtual DataViewChild FindChildAtPoint (Point point)
        {
            return Children.Find (child => child.Allocation.Contains (
                ActualAllocation.X + point.X, ActualAllocation.Y + point.Y));
        }

        public virtual DataViewChild FindChildAtModelRowIndex (int modelRowIndex)
        {
            return Children.Find (child => child.ModelRowIndex == modelRowIndex);
        }

        protected abstract void InvalidateChildSize ();
        protected abstract void InvalidateVirtualSize ();
        protected abstract void InvalidateChildCollection ();
        protected abstract void InvalidateChildLayout ();

        protected Rect GetChildVirtualAllocation (Rect childAllocation)
        {
            return new Rect () {
                X = childAllocation.X - ActualAllocation.X,
                Y = childAllocation.Y - ActualAllocation.Y,
                Width = childAllocation.Width,
                Height = childAllocation.Height
            };
        }
    }
}
