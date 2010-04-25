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
    /// <summary>
    /// This class tracks the available types of photo sources, as registered by the addins.
    /// </summary>
    public class PhotoSourceInfoManager
    {
        const string EXTENSION_POINT = "/Tripod/Core/PhotoSource";

        public static PhotoSourceInfoManager Instance;

        public List<IPhotoSourceInfo> PhotoSources { get; private set; }

        /// <summary>
        /// This dictionary is a quick lookup table that is used for source instantiation.
        /// </summary>
        /// <seealso cref="Tripod.Sources.Cache.MainCachePhotoSource"/>
        public Dictionary<string, Type> PhotoSourceTypes { get; private set; }

        public static void Initialize ()
        {
            if (Instance != null)
                throw new InvalidOperationException ("Can't initialize twice!");

            Instance = new PhotoSourceInfoManager ();
        }

        private PhotoSourceInfoManager ()
        {
            PhotoSources = new List<IPhotoSourceInfo> ();
            PhotoSourceTypes = new Dictionary<string, Type> ();
            AddinManager.AddExtensionNodeHandler (EXTENSION_POINT, OnExtensionChanged);

            Core.MainCachePhotoSource.Start ();
        }

        private void RegisterPhotoSource (IPhotoSourceInfo source_info)
        {
            PhotoSources.Add (source_info);
            PhotoSourceTypes.Add (source_info.Type.FullName, source_info.Type);
        }

        private void OnExtensionChanged (object o, ExtensionNodeEventArgs args)
        {
            InstanceExtensionNode node = (InstanceExtensionNode)args.ExtensionNode;

            if (args.Change == ExtensionChange.Add) {
                RegisterPhotoSource (node.CreateInstance () as IPhotoSourceInfo);
            } else {
                throw new NotImplementedException (); // TODO: Handle this
            }
        }
    }
}

