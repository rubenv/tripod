//
// CachedMipMap.cs
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

using Gdk;
using System;
using System.IO;
using System.Collections.Generic;

namespace Tripod.Graphics
{
    /// <summary>
    /// Optimized mipmap file, does lazy loading of image data, only renders the pieces that are requested.
    /// </summary>
    public sealed class MipMapFile : IDisposable
    {
        Uri uri;
        Stream read_stream;

        // Only used by MipMapGenerator
        internal MipMapFile ()
        {
        }

        /// <summary>
        /// Load mipmap from file.
        /// </summary>
        public MipMapFile (Uri uri)
        {
            this.uri = uri;
            ReadItems ();
        }

        void OpenReadStream ()
        {
            if (uri == null)
                throw new Exception ("Can't open read stream without uri");
            if (read_stream != null)
                return;

            read_stream = new GLib.GioStream (GLib.FileFactory.NewForUri (uri).Read (null));
        }

        void CloseReadStream ()
        {
            read_stream.Close ();
            read_stream = null;
        }

        void ReadItems ()
        {
            OpenReadStream ();

            // Verify header
            byte[] header = new byte[HEADER.Length];
            read_stream.Read (header, 0, HEADER.Length);
            if (header[0] != HEADER[0] || header[1] != HEADER[1] || header[2] != HEADER[2] || header[3] != HEADER[3]) {
                throw new Exception ("Unknown header, wrong file?");
            }

            // Extract length field
            byte[] length_buffer = new byte[4];
            read_stream.Read (length_buffer, 0, 4);
            int length = BitConverter.ToInt32 (length_buffer, 0);

            // Read record data
            byte[] header_buffer = new byte[length * 16];
            read_stream.Read (header_buffer, 0, length * 16);
            for (int i = 0; i < length; i++) {
                Items.Add (new MipMapItem () {
                    MipMap = this,
                    Width = BitConverter.ToInt32 (header_buffer, 16 * i),
                    Height = BitConverter.ToInt32 (header_buffer, 16 * i + 4),
                    LoadOffset = BitConverter.ToInt32 (header_buffer, 16 * i + 8),
                    LoadLength = BitConverter.ToInt32 (header_buffer, 16 * i + 12)
                });
            }
            CloseReadStream ();
        }

        readonly byte [] HEADER = new byte[] { 0x74, 0x72, 0x6d, 0x01 };
        const string MIPMAP_QUALITY = "80";
        const string MIPMAP_TYPE = "jpeg";

        List<MipMapItem> Items = new List<MipMapItem> (7); // Usually not more than 7.

        /// <summary>
        /// Find the best matching mipmap tile. This is the first image that is longer (on one side) than the given
        /// dimensions.
        /// </summary>
        /// <param name="width">
        /// The desired width of the resulting tile.
        /// </param>
        /// <param name="height">
        /// The desired height of the resulting tile.
        /// </param>
        /// <returns>
        /// A <see cref="Pixbuf"/>, which should be disposed by the consumer once done.
        /// </returns>
        /// <para>
        /// This selection algorithm is optimized for photo applications, it selects the smallest available piece that
        /// can be displayed within the given dimensions without upscaling.
        /// </para>
        public Pixbuf FindBest (int width, int height)
        {
            var item = FindBestItem (width, height);
            return new Gdk.Pixbuf (item.Pixbuf, 0, 0, item.Width, item.Height);
        }

        MipMapItem FindBestItem (int width, int height)
        {
            if (Items.Count == 0) {
                throw new Exception ("Can't retrieve from uninitialized mip-map");
            }
            
            int i = 0;
            MipMapItem current = null;
            while (i < Items.Count) {
                current = Items[i++];
                
                if (current.Width > width && current.Height > height)
                    break;
                // Image is larger than requested dimensions.
            }
            
            return current;
        }

        public bool IsBestSize (int have_width, int have_height, int desired_width, int desired_height)
        {
            var item = FindBestItem (desired_width, desired_height);

            return (item.Width == have_width && item.Height == have_height);
        }

        /// <summary>
        /// Output the mipmap to a file for caching.
        /// </summary>
        public void WriteToUri (Uri uri)
        {
            Stream stream = new GLib.GioStream (GLib.FileFactory.NewForUri (uri).Create (GLib.FileCreateFlags.None, null));

            // Data starts after the header which keeps the item data.
            //
            // Header structure:
            //  * 3 bytes to denote the file type (74 72 6d == trm, which stands for Tripod Mipmap)
            //  * 1 bytes version number (currently 01)
            //  * 4 bytes entry count (int)
            //  * N times 16 bytes header info for each item:
            //    * 4 bytes Width (int)
            //    * 4 bytes Height (int)
            //    * 4 bytes Offset (int)
            //    * 4 bytes Length (int)
            var data_offset = Items.Count * 16 + 8;

            List<byte> header = new List<byte> (data_offset);
            List<byte[]> data = new List<byte[]> (Items.Count);

            header.AddRange (HEADER);
            header.AddRange (BitConverter.GetBytes (Items.Count));

            foreach (var item in Items) {
                header.AddRange (BitConverter.GetBytes (item.Width));
                header.AddRange (BitConverter.GetBytes (item.Height));
                header.AddRange (BitConverter.GetBytes (data_offset));

                byte[] item_data = item.Data;
                header.AddRange (BitConverter.GetBytes (item_data.Length));
                data.Add (item_data);

                data_offset += item_data.Length;
            }

            var header_array = header.ToArray ();
            stream.Write (header_array, 0, header_array.Length);
            foreach (var item_data in data) {
                // Append items
                stream.Write (item_data, 0, item_data.Length);
            }

            stream.Close ();
        }

        // Load the data from a given item.
        byte[] LoadData (MipMapItem item)
        {
            OpenReadStream ();
            read_stream.Seek (item.LoadOffset, SeekOrigin.Begin);
            byte[] buffer = new byte[item.LoadLength];
            read_stream.Read (buffer, 0, item.LoadLength);
            CloseReadStream ();
            return buffer;
        }

        internal void Add (Pixbuf pixbuf)
        {
            Items.Add (new MipMapItem () {
                MipMap = this,
                Pixbuf = pixbuf
            });
        }


        public void Dispose ()
        {
            if (read_stream != null) {
                read_stream.Close ();
                read_stream = null;
            }

            foreach (var item in Items) {
                item.Dispose ();
            }
        }

        public IEnumerable<Gdk.Pixbuf> Pixbufs {
            get {
                foreach (var item in Items) {
                    yield return new Gdk.Pixbuf (item.Pixbuf, 0, 0, item.Width, item.Height);
                }
            }
        }

        ~MipMapFile()
        {
            Dispose();
        }

        // Represents the mipmap items as stored in the cache file. Only parses the pixbufs when needed and at the same
        // time makes sure they are only parsed once.
        private sealed class MipMapItem : IDisposable
        {
            public MipMapFile MipMap { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public int LoadOffset { get; set; }
            public int LoadLength { get; set; }

            byte[] data;
            public byte[] Data {
                get {
                    if (data != null)
                        return data;
                    lock (MipMap) {
                        // The lock avoids race conditions on the stream, locking here to avoid double loading.
                        if (pixbuf != null) {
                            data = pixbuf.SaveToBuffer (MIPMAP_TYPE, new string[] { "quality" }, new string[] { MIPMAP_QUALITY });
                        } else {
                            data = MipMap.LoadData (this);
                        }
                    }
                    return data;
                }
                set {
                    data = value;
                    if (pixbuf != null)
                        pixbuf.Dispose ();
                    pixbuf = null;
                }
            }

            Pixbuf pixbuf;
            public Pixbuf Pixbuf {
                get {
                    if (pixbuf != null)
                        return pixbuf;
                    pixbuf = new Pixbuf (Data, Width, Height);
                    data = null; // No need to keep it in memory
                    return pixbuf;
                }
                set {
                    if (pixbuf != null)
                        pixbuf.Dispose ();
                    pixbuf = value;
                    data = null;
                    Width = pixbuf.Width;
                    Height = pixbuf.Height;
                }
            }

            public void Dispose ()
            {
                if (pixbuf != null) {
                    pixbuf.Dispose ();
                    pixbuf = null;
                }
            }

            ~MipMapItem ()
            {
                Dispose ();
            }
        }
    }
}

