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
using TagLib.Image;
using Tripod.Base;
using GLib;

namespace Tripod.Model
{
    public class LocalFilePhoto : IPhoto
    {

#region Constructors

        public LocalFilePhoto (Uri uri)
        {
            this.Uri = uri;
        }

#endregion


#region IPhoto File Properties

        public Uri Uri { get; private set; }


        public DateTime ImageDataStamp {
            get {
                // TODO: This should probably be taken from somewhere else, e.g. the metadata sidecar. Or the local database.
                return UriStamp (Uri, "time::modified");
            }
            set {
                throw new System.NotImplementedException ();
            }
        }

#endregion


#region IPhoto Metadata Properties

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

        ImageOrientation orientation;
        public ImageOrientation Orientation {
            get {
                EnsureMetadataParsed ();
                return orientation;
            }
            set {
                orientation = value;
            }
        }

        uint? rating;
        public uint? Rating {
            get {
                EnsureMetadataParsed ();
                return rating;
            }
            set {
                rating = value;
            }
        }

        double? exposure_time;
        public double? ExposureTime {
            get {
                EnsureMetadataParsed ();
                return exposure_time;
            }
        }

        double? f_number;
        public double? FNumber {
            get {
                EnsureMetadataParsed ();
                return f_number;
            }
        }

        double? focal_length;
        public double? FocalLength {
            get {
                EnsureMetadataParsed ();
                return focal_length;
            }
        }

        double? focal_length_35mm;
        public double? FocalLengthIn35mmFilm {
            get {
                EnsureMetadataParsed ();
                return focal_length_35mm;
            }
        }

        string camera_make;
        public string CameraMake {
            get {
                EnsureMetadataParsed ();
                return camera_make;
            }
        }

        string camera_model;
        public string CameraModel {
            get {
                EnsureMetadataParsed ();
                return camera_model;
            }
        }

        int width;
        public int Width {
            get {
                EnsureMetadataParsed ();
                return width;
            }
        }

        int height;
        public int Height {
            get {
                EnsureMetadataParsed ();
                return height;
            }
        }

#endregion


#region Private Methods

        bool metadata_parsed = false;

        void EnsureMetadataParsed ()
        {
            if (metadata_parsed)
                return;

            // The lack of thread checking is intentional. Races will probably rarely occur and if the do, they are
            // harmless (just a bit of double computation). Saves us the locking overhead.

            var file = TagLib.File.Create (new GIOTagLibFileAbstraction() { Uri = Uri }) as TagLib.Image.File;

            var image_tag = file.ImageTag;

            Comment = image_tag.Comment;
            DateTaken = image_tag.DateTime ?? UriStamp (Uri, "time::changed");
            Orientation = image_tag.Orientation;
            Rating = image_tag.Rating;
            exposure_time = image_tag.ExposureTime;
            f_number = image_tag.FNumber;
            focal_length = image_tag.FocalLength;
            focal_length_35mm = image_tag.FocalLengthIn35mmFilm;
            camera_make = image_tag.Make;
            camera_model = image_tag.Model;

            var properties = file.Properties;
            width = properties.PhotoWidth;
            height = properties.PhotoHeight;

            metadata_parsed = true;
        }

#endregion


        static DateTime UriStamp (Uri uri, string kind)
        {
            var file = FileFactory.NewForUri (uri);
            var info = file.QueryInfo (kind, FileQueryInfoFlags.None, null);
            var stamp = info.GetAttributeULong (kind);
            return Hyena.DateTimeUtil.FromTimeT ((long) stamp);
        }
    }
}

