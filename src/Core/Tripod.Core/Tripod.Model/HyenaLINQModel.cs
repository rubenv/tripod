//
// HyenaLINQModel.cs
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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;
using Hyena.Data;
using Hyena.Data.Sqlite;

namespace Tripod.Model
{
    public class HyenaLINQModel<ImplementationType, VisibleType> : BaseListModel<VisibleType>, ICacheableDatabaseModel
        where ImplementationType : ICacheableItem, VisibleType, new ()
    {
        public string ReloadFragment { get; set; }
        public string SelectAggregates { get; set; }
        public string JoinTable { get; set; }
        public string JoinFragment { get; set; }
        public string JoinPrimaryKey { get; set; }
        public string JoinColumn { get; set; }
        public bool CachesJoinTableEntries { get; set; }
        public bool CachesValues { get; set; }

        // TODO: adapt this
        public int FetchCount {
            get { return 10; }
        }

        public SqliteModelCache<ImplementationType> Cache { get; private set; }

        private static int id;

        public HyenaLINQModel (TripodQuery<ImplementationType> query)
        {
            ReloadFragment = String.Format ("FROM {0} WHERE {1}", query.Provider.TableName, query.ConditionFragment);
            Cache = new SqliteModelCache<ImplementationType> (query.Provider.Connection, (id++).ToString (), this, query.Provider);
        }

        public override void Reload ()
        {
            Cache.Reload ();
            Cache.UpdateAggregates ();
            count = (int) Cache.Count;
            OnReloaded ();
        }

        public override void Clear ()
        {
            count = 0;
            Cache.Clear ();
            OnCleared ();
        }

        public override VisibleType this[int index] {
            get { return Cache.GetValue (index); }
        }

        int count;

        public override int Count {
            get {
                return count;
            }
        }
    }
}

