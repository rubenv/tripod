//
// RecursiveDirectoryEnumerator.cs
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
using System.Collections;
using System.Collections.Generic;
using GLib;

namespace Tripod.Base
{
    public class RecursiveFileEnumerator : IEnumerable<File>
    {
        Uri root;

        public RecursiveFileEnumerator (Uri root)
        {
            this.root = root;
        }

        IEnumerable<File> ScanForFiles (File root)
        {
            var enumerator = root.EnumerateChildren ("standard::name,standard::type", FileQueryInfoFlags.None, null);
            foreach (FileInfo info in enumerator) {
                File file = root.GetChild (info.Name);
                
                if (info.FileType == FileType.Regular) {
                    yield return file;
                } else if (info.FileType == FileType.Directory) {
                    foreach (var child in ScanForFiles (file)) {
                        yield return child;
                    }
                }
            }
        }

        public IEnumerator<File> GetEnumerator ()
        {
            var file = FileFactory.NewForUri (root);
            return ScanForFiles (file).GetEnumerator ();
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator ();
        }
    }
}
