//
// CachePhoto.cs
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

using Hyena.Data.Sqlite;

using TagLib.Image;


namespace Tripod.Model
{
    public class CachePhoto : IPhoto
    {
        [DatabaseColumn(Constraints = DatabaseColumnConstraints.PrimaryKey)]
        public int CacheId { get; set; }

        [DatabaseColumn]
        public int SourceId { get; internal set; }

        [DatabaseColumn]
        public string PhotoUri {
            get { return Uri.ToString (); }
            set { Uri = new Uri (value); }
        }

        public Uri Uri { get; set; }

        [DatabaseColumn]
        public DateTime ImageDataStamp { get; set; }

        [DatabaseColumn]
        public string Comment { get; set; }

        [DatabaseColumn]
        public DateTime DateTaken { get; set; }

        [DatabaseColumn]
        public ImageOrientation Orientation { get; set; }

        [DatabaseColumn]
        public uint? Rating { get; set; }

        [DatabaseColumn]
        public double? ExposureTime { get; private set; }

        [DatabaseColumn]
        public double? FNumber { get; private set; }

        [DatabaseColumn]
        public double? FocalLength { get; private set; }

        [DatabaseColumn]
        public double? FocalLengthIn35mmFilm { get; private set; }

        [DatabaseColumn]
        public string CameraMake { get; private set; }

        [DatabaseColumn]
        public string CameraModel { get; private set; }


        public static CachePhoto CreateFrom (IPhoto source_photo)
        {
            return new CachePhoto () {
                Uri = source_photo.Uri,
                ImageDataStamp = source_photo.ImageDataStamp,
                Comment = source_photo.Comment,
                DateTaken = source_photo.DateTaken,
                Orientation = source_photo.Orientation,
                Rating = source_photo.Rating,
                ExposureTime = source_photo.ExposureTime,
                FNumber = source_photo.FNumber,
                FocalLength = source_photo.FocalLength,
                FocalLengthIn35mmFilm = source_photo.FocalLengthIn35mmFilm,
                CameraMake = source_photo.CameraMake,
                CameraModel = source_photo.CameraModel

            };
        }
    }
}

