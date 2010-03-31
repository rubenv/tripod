//
// Core.cs
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

using Gtk;
using System;
using Hyena.Jobs;
using Hyena.Data.Sqlite;

using Tripod.Model;

namespace Tripod.Base
{
    public class Core
    {
        static readonly Scheduler scheduler = new Scheduler ();
        public static Scheduler Scheduler {
            get { return scheduler; }
        }

        static readonly HyenaSqliteConnection db_connection = new TripodSqliteConnection("test.db");
        public static HyenaSqliteConnection DbConnection {
            get { return db_connection; }
        }

        static ICachingPhotoSource main_cache_photo_source;
        public static ICachingPhotoSource MainCachePhotoSource {
            get { return main_cache_photo_source; }
        }

        public static void Initialize (string name, ref string[] args)
        {
            Hyena.Log.Debugging = true;
            GLib.Log.SetLogHandler ("Gtk", GLib.LogLevelFlags.Critical, GLib.Log.PrintTraceLogFunction);
            
            Hyena.Log.Debug ("Initializing Core");

            Application.Init (name, ref args);

            main_cache_photo_source = new MainCachePhotoSource ();
            main_cache_photo_source.Start ();
        }
    }
}