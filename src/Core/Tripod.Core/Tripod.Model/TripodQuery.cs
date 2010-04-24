// 
// TripodQuery.cs
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
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;

using Hyena.Data;
using Hyena.Data.Sqlite;

namespace Tripod.Model
{
    public class TripodQuery<T> : IEnumerable<T> where T : ICacheableItem, new ()
    {
        public SqliteModelProvider<T> Provider { get; private set; }

        Expression _where;

        public TripodQuery (SqliteModelProvider<T> provider)
        {
            Provider = provider;
        }

        public TripodQuery<T> Clone ()
        {
            var q = new TripodQuery<T> (Provider);
            q._where = _where;
            return q;
        }

        public TripodQuery<T> Where (Expression<Func<T, bool>> predExpr)
        {
            if (predExpr.NodeType == ExpressionType.Lambda) {
                var lambda = (LambdaExpression)predExpr;
                var pred = lambda.Body;
                var q = Clone ();
                q.AddWhere (pred);
                return q;
            } else {
                throw new NotSupportedException ("Must be a predicate");
            }
        }

        private void AddWhere (Expression pred)
        {
            Hyena.Log.InformationFormat ("Adding where, before: {0}", ConditionFragment);
            if (_where == null) {
                _where = pred;
            } else {
                _where = Expression.AndAlso (_where, pred);
            }
            Hyena.Log.InformationFormat ("Adding where, after: {0}", ConditionFragment);
        }

        public string ConditionFragment
        {
            get {
                if (_where == null)
                    return "1=1";

                var c = CompileExpr (_where);
                return c.CommandText;
            }
        }

        class CompileResult
        {
            public string CommandText { get; set; }
            public object Value { get; set; }
        }

        private CompileResult CompileExpr (Expression expr)
        {
            if (expr is BinaryExpression) {
                var bin = (BinaryExpression)expr;
                
                var leftr = CompileExpr (bin.Left);
                var rightr = CompileExpr (bin.Right);
                
                var text = "(" + leftr.CommandText + " " + GetSqlName (bin) + " " + rightr.CommandText + ")";
                return new CompileResult { CommandText = text };
            } else if (expr.NodeType == ExpressionType.Constant) {
                var c = (ConstantExpression)expr;
                var val = c.Value;
                string t;
                if (val is string) {
                    t = "'" + val.ToString ().Replace ("'", "''") + "'";
                } else {
                    t = val.ToString ();
                }
                return new CompileResult { CommandText = t, Value = c.Value };
            } else if (expr.NodeType == ExpressionType.Convert) {
                var u = (UnaryExpression)expr;
                var ty = u.Type;
                var valr = CompileExpr (u.Operand);
                return new CompileResult { CommandText = valr.CommandText, Value = valr.Value != null ? Convert.ChangeType (valr.Value, ty) : null };
            } else if (expr.NodeType == ExpressionType.MemberAccess) {
                var mem = (MemberExpression)expr;

                Hyena.Log.Information (mem.ToString ());
                Hyena.Log.Information ((mem.Expression == null).ToString ());

                if (mem.Expression == null) {
                    if (!mem.ToString ().Equals ("String.Empty"))
                        throw new NotSupportedException ("Unknown expression");
                    return new CompileResult { CommandText = "\"\"" };
                } else if (mem.Expression.NodeType == ExpressionType.Parameter) {
                    //
                    // This is a column of our table, output just the column name
                    //
                    return new CompileResult { CommandText = "\"" + mem.Member.Name + "\"" };
                } else {
                    var r = CompileExpr (mem.Expression);
                    if (r.Value == null) {
                        throw new NotSupportedException ("Member access failed to compile expression");
                    }
                    var obj = r.Value;
                    
                    if (mem.Member.MemberType == MemberTypes.Property) {
                        var m = (PropertyInfo)mem.Member;
                        var val = m.GetValue (obj, null);
                        return new CompileResult { CommandText = "'" + val.ToString ().Replace ("'", "''") + "'", Value = val };
                    } else if (mem.Member.MemberType == MemberTypes.Field) {
                        var m = (FieldInfo)mem.Member;
                        var val = m.GetValue (obj);
                        return new CompileResult { CommandText = "'" + val.ToString ().Replace ("'", "''") + "'", Value = val };
                    } else {
                        throw new NotSupportedException ("MemberExpr: " + mem.Member.MemberType.ToString ());
                    }
                }
            }
            throw new NotSupportedException ("Cannot compile: " + expr.NodeType.ToString ());
        }

        string GetSqlName (Expression expr)
        {
            var n = expr.NodeType;
            if (n == ExpressionType.GreaterThan)
                    return ">"; else if (n == ExpressionType.GreaterThanOrEqual)
                    return ">="; else if (n == ExpressionType.LessThan)
                    return "<"; else if (n == ExpressionType.LessThanOrEqual)
                    return "<="; else if (n == ExpressionType.And)
                    return "and"; else if (n == ExpressionType.AndAlso)
                    return "and"; else if (n == ExpressionType.Or)
                    return "or"; else if (n == ExpressionType.OrElse)
                    return "or"; else if (n == ExpressionType.Equal)
                    return "="; else if (n == ExpressionType.NotEqual)
                    return "!=";
            else
                    throw new System.NotSupportedException ("Cannot get SQL for: " + n.ToString ());
        }


        #region IEnumerable<T> implementation
        public IEnumerator<T> GetEnumerator ()
        {
            Hyena.Log.Information (ConditionFragment);
            return Provider.FetchAllMatching (ConditionFragment).GetEnumerator ();
        }
        #endregion

        #region IEnumerable implementation
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return GetEnumerator ();
        }
        #endregion

    }
}

