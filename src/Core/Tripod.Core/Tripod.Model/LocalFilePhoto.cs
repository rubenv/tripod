//
// LocalFilePhoto.cs
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
using TagLib;
using Tripod.Base;
using GLib;

namespace Tripod.Model
{
    public class LocalFilePhoto : IPhoto
    {
        bool metadata_parsed = false;

        public LocalFilePhoto (Uri uri)
        {
            this.Uri = uri;
        }

        public Uri Uri { get; private set; }

        string comment;
        public string Comment {
            get {
                EnsureMetadataParsed ();
                return comment;
            }
            set { comment = value; }
        }

        DateTime datetaken;
        public DateTime DateTaken {
            get {
                EnsureMetadataParsed ();
                return datetaken;
            }
            set {
                datetaken = value;
            }
        }

        public DateTime ImageDataStamp {
            get {
                // TODO: This should probably be taken from somewhere else, e.g. the metadata sidecar. Or the local database.
                return UriStamp (Uri, "time::modified");
            }
            set {
                throw new System.NotImplementedException ();
            }
        }

        void EnsureMetadataParsed ()
        {
            if (metadata_parsed)
                return;

            // The lack of thread checking is intentional. Races will probably rarely occur and if the do, they are
            // harmless (just a bit of double computation). Saves us the locking overhead.

            var file = TagLib.File.Create (new GIOTagLibFileAbstraction() { Uri = Uri }) as TagLib.Image.File;

            Comment = file.ImageTag.Comment;
            DateTaken = file.ImageTag.DateTime ?? UriStamp (Uri, "time::changed");

            metadata_parsed = true;
        }

        static DateTime UriStamp (Uri uri, string kind)
        {
            var file = FileFactory.NewForUri (uri);
            var info = file.QueryInfo (kind, FileQueryInfoFlags.None, null);
            var stamp = info.GetAttributeULong (kind);
            return Hyena.DateTimeUtil.FromTimeT ((long) stamp);
        }
    }
}

