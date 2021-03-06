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
using System.Linq;
using Hyena;
using Hyena.Jobs;
using Hyena.Data.Sqlite;
using Mono.Addins;

using Tripod.Sources;
using Tripod.Sources.SqliteCache;
using Tripod.Graphics;

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

        static IPhotoSourceCache main_cache_photo_source = new SqlitePhotoSourceCache ();
        public static IPhotoSourceCache MainPhotoSourceCache {
            get { return main_cache_photo_source; }
        }

        static PhotoLoaderCache photo_loader_cache = new PhotoLoaderCache ();
        public static PhotoLoaderCache PhotoLoaderCache {
            get { return photo_loader_cache; }
        }

        public static void Initialize (string name, ref string[] args)
        {
            Hyena.Log.Debugging = true;

            InitializeToolkit (name, ref args);
            InitializeAddins ();
            InitializeSources ();
            InitializeDefaultSource ();
        }

        static void InitializeToolkit (string name, ref string[] args)
        {
            GLib.Log.SetLogHandler ("Gtk", GLib.LogLevelFlags.Critical, GLib.Log.PrintTraceLogFunction);
            GLib.Log.SetLogHandler ("GdkPixbuf", GLib.LogLevelFlags.Critical, GLib.Log.PrintTraceLogFunction);

            ThreadAssist.InitializeMainThread ();
            ThreadAssist.ProxyToMainHandler = (h) => GLib.Idle.Add (() => { h(); return false; });

            ApplicationContext.TrySetProcessName ("tripod");
            Application.Init (name, ref args);
        }

        static void InitializeAddins ()
        {
            AddinManager.Initialize (ApplicationContext.CommandLine.Contains ("uninstalled")
                ? "." : Paths.ApplicationData);

            IProgressStatus monitor = ApplicationContext.CommandLine.Contains ("debug-addins")
                ? new ConsoleProgressStatus (true)
                : null;

            if (ApplicationContext.Debugging) {
                AddinManager.Registry.Rebuild (monitor);
            } else {
                AddinManager.Registry.Update (monitor);
            }
        }

        static void InitializeSources ()
        {
            PhotoSourceInfoManager.Initialize ();
        }

        /// <summary>
        /// Adds a default photo source if no sources are available (a local folder, ~/Pictures).
        /// </summary>
        static void InitializeDefaultSource ()
        {
            // TODO: We need something smarter than the default hardcoded type.
            if (!MainPhotoSourceCache.PhotoSources.Any ()) {
                var type_name = "Tripod.Sources.LocalFolder.LocalFolderPhotoSource";

                var type = PhotoSourceInfoManager.Instance.PhotoSourceTypes[type_name];
                var instance = Activator.CreateInstance (type) as IPhotoSource;
                instance.SetOption("Root", new Uri ("file://" + Environment.GetFolderPath (Environment.SpecialFolder.MyPictures)));
                instance.SetOption("WatchFileSystem", false);
                MainPhotoSourceCache.RegisterPhotoSource (instance as ICacheablePhotoSource);
            }
        }
    }
}
