//
// ImportWindow.cs
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
using System;
using Gtk;
using Mono.Unix;
using Hyena;
using Tripod.Sources;

namespace FlashUnit.Gui
{
    // FIXME: Should probably be a dialog
    public class ImportWindow : Window
    {
        Button cancel_button;
        Button import_button;
        ComboBox sources_combo;
        TreeStore sources_model;

        public ImportWindow () : base(Catalog.GetString ("Import"))
        {
            BorderWidth = 12;
            SetSizeRequest (400, 300);
        }

        private bool interface_constructed;

        protected override void OnShown ()
        {
            if (interface_constructed) {
                base.OnShown ();
                return;
            }

            interface_constructed = true;

            PrepareStore ();
            BuildLayout ();
            ConnectEvents ();
            base.OnShown ();
        }

        void PrepareStore ()
        {
            sources_model = new TreeStore (typeof (IPhotoSourceInfo), typeof (string));
            GLib.Idle.Add (() => {
                foreach (var info in PhotoSourceInfoManager.Instance.PhotoSources) {
                    if (!info.Importable)
                        continue;
                    sources_model.AppendValues (info, info.Name);
                }
                return false;
            });
            // TODO: populate
        }

        void BuildLayout ()
        {
            var primary_vbox = new VBox () {
                Spacing = 6
            };

            // Source selector
            var combo_align = new Alignment (0, .5f, 0, 0);
            var combo_hbox = new HBox (false, 6);
            combo_hbox.Add (new Label (Catalog.GetString ("Import from:")));
            sources_combo = new ComboBox (sources_model);
            var render = new CellRendererText ();
            sources_combo.PackStart (render, true);
            sources_combo.SetAttributes (render, "text", 1);
            combo_hbox.Add (sources_combo);
            combo_align.Add (combo_hbox);
            combo_align.ShowAll ();
            primary_vbox.Add (combo_align);

            // Button row near the top
            var align = new Alignment (1, .5f, 0, 0);
            var button_box = new HButtonBox () {
                Spacing = 6
            };
            button_box.Add (cancel_button = new Button (Stock.Cancel));
            button_box.Add (import_button = new Button (Stock.Add));
            align.Add (button_box);
            align.ShowAll ();

            primary_vbox.Add (align);
            primary_vbox.Show ();

            Add (primary_vbox);
        }

        void ConnectEvents ()
        {
            cancel_button.Clicked += (o, args) => {
                Hide ();
                Destroy ();
            };
            import_button.Clicked += (o, args) => {
                Log.Debug ("Importing!");
            };
        }
    }
}

