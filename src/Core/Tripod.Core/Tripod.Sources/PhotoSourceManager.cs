// 
// SourceManager.cs
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
using System.Collections.Generic;
using Hyena;
using Mono.Addins;
using Tripod.Base;

namespace Tripod.Sources
{
    public class PhotoSourceManager
    {
        const string EXTENSION_POINT = "/Tripod/Core/PhotoSource";

        public static PhotoSourceManager Instance;

        public Dictionary<string, Type> PhotoSourceTypes { get; set; }

        public static void Initialize ()
        {
            if (Instance != null)
                throw new InvalidOperationException ("Can't initialize twice!");

            Instance = new PhotoSourceManager ();
        }

        private PhotoSourceManager ()
        {
            PhotoSourceTypes = new Dictionary<string, Type> ();
            AddinManager.AddExtensionNodeHandler (EXTENSION_POINT, OnExtensionChanged);

            Core.MainCachePhotoSource.Start ();
        }

        private void OnExtensionChanged (object o, ExtensionNodeEventArgs args)
        {
            TypeExtensionNode node = (TypeExtensionNode)args.ExtensionNode;

            Log.DebugFormat ("Extension: {0} {1}", args.Change == ExtensionChange.Add ? "add" : "remove", node.Type.ToString ());

            if (args.Change == ExtensionChange.Add) {
                PhotoSourceTypes.Add (node.Type.FullName, node.Type);
            } else {
                throw new NotImplementedException ();
            }
        }
    }
}

