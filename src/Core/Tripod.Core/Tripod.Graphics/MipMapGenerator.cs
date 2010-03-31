//
// MipMapCache.cs
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
using System.Collections.Generic;
using Tripod.Model;
using Tripod.Base;
using Hyena;
using TagLib.Image;

namespace Tripod.Graphics
{
    enum ScaleMode {
        Width,
        Height
    }

    public static class MipMapGenerator
    {
        public static Uri MipMapUri (IPhoto photo)
        {
            var hash = CryptoUtil.Md5Encode (photo.Uri.ToString ());

            return new Uri (String.Format ("file:///tmp/mipmap/{0}.trimips", hash));
        }

        public static MipMapFile LoadMipMap (IPhoto photo)
        {
            var mipmap_uri = MipMapUri (photo);

            try {
                return new MipMapFile (mipmap_uri);
            } catch {}

            GenerateMipMap (photo);

            return new MipMapFile (mipmap_uri);
        }

        public static void GenerateMipMap (IPhoto photo)
        {
            var mipmap_uri = MipMapUri (photo);
            
            Log.DebugFormat ("Generating mipmap for {0} - {1}", photo.Uri.ToString (), mipmap_uri.AbsoluteUri);
            
            var file = GLib.FileFactory.NewForUri (photo.Uri);
            var pixbuf = new Gdk.Pixbuf (new GLib.GioStream (file.Read (null)));
            
            var imagefile = TagLib.File.Create (new GIOTagLibFileAbstraction () { Uri = photo.Uri }) as TagLib.Image.File;
            var tag = imagefile.ImageTag;
            
            // Correct orientation
            pixbuf = pixbuf.TransformOrientation (tag.Orientation);

            // Determine mode
            var mode = pixbuf.Width > pixbuf.Height ? ScaleMode.Width : ScaleMode.Height;
            var longest = mode == ScaleMode.Width ? pixbuf.Width : pixbuf.Height;

            double scale_factor = Math.Max ((double) longest / 1600, 1.0);

            MipMapFile map = new MipMapFile ();
            List<Gdk.Pixbuf> pixbufs = new List<Gdk.Pixbuf>(7); // Six or seven on average

            if (scale_factor > 1.0) {
                using (var tmp = pixbuf)
                    pixbuf = pixbuf.ScaleSimple ((int) Math.Round (pixbuf.Width / scale_factor), (int) Math.Round (pixbuf.Height / scale_factor), Gdk.InterpType.Bilinear);
            }

            int max;
            do {
                max = Math.Max (pixbuf.Width, pixbuf.Height);
                pixbufs.Add (pixbuf);

                pixbuf = pixbuf.ScaleSimple (pixbuf.Width / 2, pixbuf.Height / 2, Gdk.InterpType.Bilinear);
            } while (max > 64);
            pixbuf.Dispose ();

            // As the mipmap items are built from largest -> smallest, we need to add them in reverse.
            pixbufs.Reverse ();
            foreach (var buf in pixbufs) {
                map.Add (buf);
            }

            map.WriteToUri (mipmap_uri);
        }
    }
}