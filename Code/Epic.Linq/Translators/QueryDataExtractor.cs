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
using Epic.Linq.Expressions.Visit;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;

namespace Epic.Linq.Translators
{
    internal sealed class QueryDataExtractor<TExpression>
        where TExpression : Expression
    {
        private readonly Expression _template;
        
        
        public QueryDataExtractor (TExpression template)
        {
            if(null == template)
                throw new ArgumentNullException("template");
            _template = template;
        }

        public bool TryGetQueryData(TExpression target, out IQuery query)
        {
            QueryData data = new QueryData();
            bool result = Fill(target, _template, data);
            if(result)
            {
                data.Lock();
                query = data;
            }
            else
            {
                query = null;
            }
            return result;
        }
        
        private bool Fill(Expression target, Expression template, QueryData data)
        {
            // both nulls? comparison is OK
            if (target == null && template == null)
                return true;

            if (target == null && template != null)
                return false;

            if (target != null && template == null)
                return false;

            // first thing: check types
            if (target.GetType() != template.GetType())
                return false;

            // then check node types
            if (target.NodeType != template.NodeType)
                return false;

            // finally, check expression types
            if (target.Type != template.Type)
                return false;

            // then for each expression subtype, check members
            if (target is BinaryExpression)
                return Fill2((BinaryExpression)target, (BinaryExpression)template);
            if (target is ConditionalExpression)
                return Fill2((ConditionalExpression)target, (ConditionalExpression)template);
            if (target is ConstantExpression)
                return Fill2((ConstantExpression)target, (ConstantExpression)template);
            if (target is InvocationExpression)
                return Fill2((InvocationExpression)target, (InvocationExpression)template);
            if (target is LambdaExpression)
                return Fill2((LambdaExpression)target, (LambdaExpression)template);
            if (target is ListInitExpression)
                return Fill2((ListInitExpression)target, (ListInitExpression)template);
            if (target is MemberExpression)
                return Fill2((MemberExpression)target, (MemberExpression)template);
            if (target is MemberInitExpression)
                return Fill2((MemberInitExpression)target, (MemberInitExpression)template);
            if (target is MethodCallExpression)
                return Fill2((MethodCallExpression)target, (MethodCallExpression)template);
            if (target is NewArrayExpression)
                return Fill2((NewArrayExpression)target, (NewArrayExpression)template);
            if (target is NewExpression)
                return Fill2((NewExpression)target, (NewExpression)template);
            if (target is ParameterExpression)
                return Fill2((ParameterExpression)target, (ParameterExpression)template);
            if (target is TypeBinaryExpression)
                return Fill2((TypeBinaryExpression)target, (TypeBinaryExpression)template);
            if (target is UnaryExpression)
                return Fill2((UnaryExpression)target, (UnaryExpression)template);
            throw new ArgumentException(string.Format("Unknown Expression type ({0})", target.GetType()));
        }

        private bool Fill2(BinaryExpression target, BinaryExpression template)
        {
            if (target.IsLifted != template.IsLifted)
                return false;

            if (target.IsLiftedToNull != template.IsLiftedToNull)
                return false;

            if (target.Method != template.Method)
                return false;

            if (!Fill(target.Conversion, template.Conversion))
                return false;

            if (!Fill(target.Left, template.Left))
                return false;

            if (!Fill(target.Right, template.Right))
                return false;

            return true;
        }

        private bool Fill2(ConditionalExpression target, ConditionalExpression template)
        {
            if (!Fill(target.Test, template.Test))
                return false;

            if (!Fill(target.IfTrue, template.IfTrue))
                return false;

            if (!Fill(target.IfFalse, template.IfFalse))
                return false;

            return true;
        }

        /// <summary>
        /// Objects comparer, with special hints
        /// </summary>
        /// <param name="target"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        private bool ObjectsEquals(object target, object template)
        {
            if (target == null && template == null)
                return true;

            if (target != null && template == null)
                return false;
            if (target == null && template != null)
                return false;

            if (target.GetType() != template.GetType())
                return false;

            if (target is Expression)
                return Fill((Expression)target, (Expression)template);
   
            // TODO: Compare QueryProviders?
            
            return Fill(target, template);
        }

        private bool Fill2(ConstantExpression target, ConstantExpression template)
        {
            return ObjectsEquals(target.Value, template.Value);
        }

        /// <summary>
        /// Determines if the two lists contain indentical data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xs">The xs.</param>
        /// <param name="ys">The ys.</param>
        /// <param name="equals2">The equals2.</param>
        /// <returns></returns>
        private bool Fill2<T>(IList<T> xs, IList<T> ys, Func<T, T, bool> equals2)
        {
            if (xs == null || ys == null)
                return false;
            if (xs.Count != ys.Count)
                return false;

            for (int index = 0; index < xs.Count; index++)
            {
                if (!equals2(xs[index], ys[index]))
                    return false;
            }
            return true;
        }

        private bool Fill2(InvocationExpression target, InvocationExpression template)
        {
            if (!Fill(target.Expression, template.Expression))
                return false;

            if (!Fill2(target.Arguments, template.Arguments))
                return false;

            return true;
        }

        private bool Fill2(LambdaExpression target, LambdaExpression template)
        {
            if (!Fill(target.Body, template.Body))
                return false;

            if (!Fill2(target.Parameters, template.Parameters, Fill2))
                return false;

            return true;
        }

        private bool Fill2(ListInitExpression target, ListInitExpression template)
        {
            if (!Fill(target.NewExpression, template.NewExpression))
                return false;

            if (!Fill(target.Initializers, template.Initializers))
                return false;

            return true;
        }

        private bool Fill2(ElementInit target, ElementInit template)
        {
            if (target.AddMethod != template.AddMethod)
                return false;

            if (!Fill2(target.Arguments, template.Arguments))
                return false;

            return true;
        }

        private bool Fill2(MemberExpression target, MemberExpression template)
        {
            if (target.Member != template.Member)
                return false;

            if (!Fill(target.Expression, template.Expression))
                return false;

            return true;
        }

        private bool Fill2(MemberInitExpression target, MemberInitExpression template)
        {
            if (!Fill(target.NewExpression, template.NewExpression))
                return false;

            if (!Fill2(target.Bindings, template.Bindings))
                return false;

            return true;
        }

        private bool Fill2(IList<MemberBinding> xs, IList<MemberBinding> ys)
        {
            return Fill2(xs, ys, Fill2);
        }

        private bool Fill2(MemberBinding target, MemberBinding template)
        {
            if (target.BindingType != template.BindingType)
                return false;

            if (target.Member != template.Member)
                return false;

            if (target.GetType() != template.GetType())
                return false;

            if (target is MemberAssignment)
                return Fill2((MemberAssignment)target, (MemberAssignment)template);
            if (target is MemberListBinding)
                return Fill2((MemberListBinding)target, (MemberListBinding)template);
            if (target is MemberMemberBinding)
                return Fill2((MemberMemberBinding)target, (MemberMemberBinding)template);

            throw new ArgumentException(string.Format("Fill2(): unsupported MemberBinding subtype ({0})", target.GetType()));
        }

        private bool Fill2(MemberAssignment target, MemberAssignment template)
        {
            return Fill(target.Expression, template.Expression);
        }

        private bool Fill2(MemberListBinding target, MemberListBinding template)
        {
            if (!Fill2(target.Initializers, template.Initializers, Fill2))
                return false;

            return true;
        }

        private bool Fill2(MemberMemberBinding target, MemberMemberBinding template)
        {
            if (!Fill2(target.Bindings, template.Bindings, Fill2))
                return false;

            return true;
        }

        private bool Fill2(MethodCallExpression target, MethodCallExpression template)
        {
            if (target.Method != template.Method)
                return false;

            if (!Fill(target.Object, template.Object))
                return false;

            if (!Fill2(target.Arguments, template.Arguments))
                return false;

            return true;
        }

        private bool Fill2(NewArrayExpression target, NewArrayExpression template)
        {
            if (!Fill2(target.Expressions, template.Expressions))
                return false;

            return true;
        }

        private bool Fill2(IList<Expression> xs, IList<Expression> ys)
        {
            return Fill2(xs, ys, Fill);
        }

        private bool Fill2(NewExpression target, NewExpression template)
        {
            if (target.Constructor != template.Constructor)
                return false;

            if (!Fill2(target.Arguments, template.Arguments))
                return false;

            if (!Fill2(target.Members, template.Members, (mx, my) => mx == my))
                return false;

            return true;
        }

        private bool Fill2(ParameterExpression target, ParameterExpression template)
        {
            if (target.Name != template.Name)
                return false;

            return true;
        }

        private bool Fill2(TypeBinaryExpression target, TypeBinaryExpression template)
        {
            if (target.TypeOperand != template.TypeOperand)
                return false;

            if (!Fill(target.Expression, template.Expression))
                return false;

            return true;
        }

        private bool Fill2(UnaryExpression target, UnaryExpression template)
        {
            if (target.IsLifted != template.IsLifted)
                return false;

            if (target.IsLiftedToNull != template.IsLiftedToNull)
                return false;

            if (target.Method != template.Method)
                return false;

            if (!Fill(target.Operand, template.Operand))
                return false;

            return true;
        }

        public int GetHashCode(Expression obj)
        {
            return (int)obj.NodeType ^ obj.GetType().GetHashCode();
        }
  
        class QueryData : IQuery    
        {
            private readonly Hashtable _table = new Hashtable();
            private bool _locked = false;
            
            public void Lock()
            {
                _locked = true;
            }
            
            public void Register<TValue> (string name, TValue value)
            {
                if(_locked)
                    throw new InvalidOperationException("Can not register on a locked query.");
                InstanceName<TValue> key = new InstanceName<TValue>(name);
                _table[key] = value;
            }
            
            #region IQuery implementation
            public TValue Get<TValue> (string name)
            {
                // TODO: throw if unlocked?
                InstanceName<TValue> key = new InstanceName<TValue>(name);
                if(!_table.ContainsKey(key))
                    throw new InvalidOperationException("Unknown key");
                return (TValue)_table[key];
            }
            #endregion
        }
    }
}

