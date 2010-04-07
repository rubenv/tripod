//
// MainWindow.cs
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
using Gtk;
using GLib;
using Hyena;
using Hyena.Jobs;
using Tripod.Base;
using Tripod.Jobs;
using Tripod.Model;
using Tripod.Model.Gui;

namespace FlashUnit.Gui
{
    public class MainWindow : Window
    {

        #region Layout components

        VBox primary_vbox;

        #endregion

        public MainWindow () : base("Flash Unit")
        {
            HeightRequest = 480;
            WidthRequest = 640;
        }

        private bool interface_constructed;

        protected override void OnShown ()
        {
            if (interface_constructed) {
                base.OnShown ();
                return;
            }
            
            interface_constructed = true;
            
            BuildLayout ();
            base.OnShown ();
        }

        #region Interface Construction

        void BuildLayout ()
        {
            primary_vbox = new VBox ();
            
            var photo_view = new PhotoGridView ();
            photo_view.Show ();

            var photo_view_scrolled = new ScrolledWindow ();
            photo_view_scrolled.Add (photo_view);
            photo_view_scrolled.Show ();
            primary_vbox.PackStart (photo_view_scrolled, true, true, 8);

            var photo_model = new Hyena.Data.MemoryListModel<IPhoto> ();
            foreach (IPhoto photo in Core.MainCachePhotoSource.Photos)
                photo_model.Add (photo);

            photo_view.SetModel (photo_model);

            var hbox = new HBox (false, 5);

            hbox.Add (new Label ("Size"));

            var scale = new HScale (50, 800, 10);
            scale.Value = 140;
            scale.ValueChanged += (s, a) => {
                photo_view.ThumbnailSize = (int) scale.Value;
            };
            hbox.Add (scale);
            hbox.ShowAll ();

            primary_vbox.PackEnd (hbox, false, true, 0);

            primary_vbox.Show ();
            Add (primary_vbox);
        }

        #endregion
    }
}

