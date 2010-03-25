// 
// ImportDialog.cs
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

namespace FlashUnit.Gui
{
    public class ImportDialog : Window
    {
        bool interface_constructed = false;

        public ImportDialog () : base ("Import")
        {

        }

        protected override void OnShown ()
        {
            if (interface_constructed) {
                base.OnShown ();
                return;
            }
            interface_constructed = true;

            BuildUI ();
            base.OnShown ();
        }

        #region UI construction

        void BuildUI () {
            var dialog_vbox = new VBox () {
                BorderWidth = 6,
                Spacing = 6
            };
            var dialog_table = new Table (2, 2, false) {
                RowSpacing = 12,
                ColumnSpacing = 6
            };

            dialog_table.Attach (new Label() {
                Text = "Import Source:"
            }, 0, 1, 0, 1);

            dialog_vbox.Add(dialog_table);
            Add (dialog_vbox);
            ShowAll ();
        }

        #endregion
    }
}

