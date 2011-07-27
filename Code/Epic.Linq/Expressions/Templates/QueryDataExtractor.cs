//  
//  QueryDataExtractor.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2011 Giacomo Tesio
// 
//  This file is part of Epic.NET.
// 
//  Epic.NET is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Affero General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Epic.NET is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Affero General Public License for more details.
// 
//  You should have received a copy of the GNU Affero General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//  
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Epic.Linq.Expressions.Templates
{
    internal sealed class QueryDataExtractor<TExpression> : IQueryDataExtractor<TExpression>
        where TExpression : Expression
    {
        private readonly Dictionary<string, List<Func<TExpression, bool>>> _knownCheckers = new Dictionary<string, List<Func<TExpression, bool>>>();
        private readonly List<Action<QueryData, TExpression>> _registrations = new List<Action<QueryData, TExpression>>();
        
        public QueryDataExtractor ()
        {
        }
        
        public bool CanParse(TExpression expression)
        {
            foreach(List<Func<TExpression, bool>> knownCheckers in _knownCheckers.Values)
            {
                bool canParse = false;
                for(int i = 0; i < knownCheckers.Count && !canParse; ++i)
                {
                    canParse = knownCheckers[i](expression);
                }
                if(! canParse)
                    return false;
            }
            
            return true;
        }
        
        public IQuery Parse(TExpression expression)
        {
            QueryData data = new QueryData();
            foreach (Action<QueryData, TExpression> register in _registrations)
                register(data, expression);
            if (data.MissValues)
                return null;
            return data;
        }
        
        private static bool CanRead(Expression e)
        {
            if(null == e)
                return false;
            return CanBeCompiled(e);
        }
        
        public void Register<TerminalExpression>(string name, Func<TExpression, TerminalExpression> visit)
            where TerminalExpression : Expression
        {
            List<Func<TExpression, bool>> knownCheckers = null;
            if(!_knownCheckers.TryGetValue(name, out knownCheckers))
            {
                knownCheckers = new List<Func<TExpression, bool>>();
                _knownCheckers[name] = knownCheckers;
            }
            knownCheckers.Add(e => CanRead(visit(e)));
            _registrations.Add((d, e) => Register<TerminalExpression>(name, visit, d, e));
        }

        private static void Register<TerminalExpression>(string name, Func<TExpression, TerminalExpression> visit, QueryData data, TExpression expressionToVisit)
            where TerminalExpression : Expression
        {
            TerminalExpression exp = visit(expressionToVisit);
            if (exp != null)
            {
                if (CanBeCompiled(exp))
                {
                    LambdaExpression expression = Expression.Lambda(typeof(Func<>).MakeGenericType(exp.Type), exp);
                    data.Register(name, expression.Compile());
                }
                else
                {
                    data.Miss(name);
                }
            }
        }

        private static bool CanBeCompiled(Expression expression)
        {
            switch(expression.NodeType)
            {
                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                case ExpressionType.UnaryPlus:
                    return CanBeCompiled((expression as UnaryExpression).Operand);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Power:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.And:
                case ExpressionType.Or:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.LeftShift:
                case ExpressionType.RightShift:
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                    BinaryExpression binaryExp = expression as BinaryExpression;
                    return CanBeCompiled(binaryExp.Left) && CanBeCompiled(binaryExp.Right);
                case ExpressionType.Conditional:
                    ConditionalExpression conditionalExp = expression as ConditionalExpression;
                    return CanBeCompiled(conditionalExp.Test) && CanBeCompiled(conditionalExp.IfTrue) && CanBeCompiled(conditionalExp.IfFalse);
                case ExpressionType.Constant:
                    return true;
                case ExpressionType.Invoke:
                    InvocationExpression invocationExp = expression as InvocationExpression;
                    foreach (Expression arg in invocationExp.Arguments)
                    {
                        if (!CanBeCompiled(arg))
                            return false;
                    }
                    return CanBeCompiled(invocationExp.Expression);
                case ExpressionType.Lambda:
                    return false;
                case ExpressionType.MemberAccess:
                    return CanBeCompiled((expression as MemberExpression).Expression);
                case ExpressionType.Call:
                    MethodCallExpression callExp = expression as MethodCallExpression;
                    foreach (Expression arg in callExp.Arguments)
                    {
                        if (!CanBeCompiled(arg))
                            return false;
                    }
                    return null == callExp.Object || CanBeCompiled(callExp.Object);
                case ExpressionType.New:
                    NewExpression newExp = expression as NewExpression;
                    foreach (Expression arg in newExp.Arguments)
                    {
                        if (!CanBeCompiled(arg))
                            return false;
                    }
                    return true;
                case ExpressionType.NewArrayBounds:
                case ExpressionType.NewArrayInit:
                    NewArrayExpression newArrExp = expression as NewArrayExpression;
                    foreach (Expression arg in newArrExp.Expressions)
                    {
                        if (!CanBeCompiled(arg))
                            return false;
                    }
                    return true;
                case ExpressionType.MemberInit:
                    MemberInitExpression initExp = expression as MemberInitExpression;
                    foreach (MemberBinding binding in initExp.Bindings)
                    {
                        switch (binding.BindingType)
                        {
                            case MemberBindingType.Assignment:
                                MemberAssignment assign = binding as MemberAssignment;
                                if (!CanBeCompiled(assign.Expression))
                                    return false;
                                break;
                            case MemberBindingType.MemberBinding:
                                MemberMemberBinding member = binding as MemberMemberBinding;
                                // TODO : complete
                                return false;
                            default:
                                MemberListBinding list = binding as MemberListBinding;
                                if(!list.Initializers.All(i => i.Arguments.All(e => CanBeCompiled(e))))
                                    return false;
                                break;
                        }
                    }
                    return CanBeCompiled(initExp.NewExpression);
                case ExpressionType.ListInit:
                    ListInitExpression listExp = expression as ListInitExpression;
                    return CanBeCompiled(listExp.NewExpression) && listExp.Initializers.All(i => i.Arguments.All(e => CanBeCompiled(e)));
                case ExpressionType.Parameter:
                    return false;
                case ExpressionType.TypeIs:
                    return CanBeCompiled((expression as TypeBinaryExpression).Expression);
                default:
                    return false;
            }
            
        }
        
        sealed class QueryData : IQuery
        {
            private readonly HashSet<string> _missings = new HashSet<string>();
            private readonly Dictionary<string, Delegate> _dataProviders = new Dictionary<string, Delegate>();
            
            #region IQuery implementation
            TValue IQuery.Get<TValue> (string name)
            {
                if(string.IsNullOrEmpty(name))
                    throw new ArgumentException("name");
                Delegate del = _dataProviders[name];
                Func<TValue> getValue = (Func<TValue>)del;
                return getValue();
            }
            #endregion

            internal bool MissValues
            {
                get
                {
                    return _missings.Count > 0 || _dataProviders.Count == 0;
                }
            }

            internal void Miss(string name)
            {
                if(!_dataProviders.ContainsKey(name))
                    _missings.Add(name);
            }

            internal void Register(string name, Delegate valueProvider)
            {
                if(_dataProviders.ContainsKey(name))
                    throw new InvalidOperationException(); // TODO : better message
                _missings.Remove(name);
                _dataProviders[name] = valueProvider;
            }
        }
    }
}

