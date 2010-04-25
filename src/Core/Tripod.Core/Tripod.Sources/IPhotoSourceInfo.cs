// 
// IPhotoSourceInfo.cs
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

namespace Tripod.Sources
{
    /// <summary>
    /// Describes a photo sources, used by the addin manager to figure out what is available.
    /// </summary>
    public interface IPhotoSourceInfo
    {
        /// <summary>
        /// The <see cref="System.Type"/> that is used for the described photo source, this type should implement
        /// <see cref="Tripod.Sources.IPhotoSource"/>.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// A human-readable name for this photo source.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Describes whether you can import from this source.
        /// </summary>
        bool Importable { get; }
    }
}

