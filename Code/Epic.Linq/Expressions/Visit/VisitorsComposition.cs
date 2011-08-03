//  
//  VisitorComposition.cs
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
using System.Linq.Expressions;
using System.Collections.Generic;
using ExprType = System.Linq.Expressions.ExpressionType;

namespace Epic.Linq.Expressions.Visit
{
    public sealed class VisitorsComposition<TResult> : ICompositeVisitor<TResult, Expression>
    {
        private readonly List<VisitorBase> _chain;
        private readonly string _name;
        public VisitorsComposition (string name)
        {
            if(string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            _name = name;
            _chain = new List<VisitorBase>();
        }

        #region ICompositeVisitor implementation
        public ICompositeVisitor<TResult, TExpression> GetVisitor<TExpression> (TExpression target) where TExpression : System.Linq.Expressions.Expression
        {
            ICompositeVisitor<TResult, TExpression> visitor = this as ICompositeVisitor<TResult, TExpression>;
            if(null != visitor)
                return visitor;
            return GetVisitor<TExpression>(target, _chain.Count);
        }
        #endregion

        #region ICompositeVisitor[TResult, Expression] implementation
        public TResult Visit (Expression target, IVisitState state)
        {
            if (target == null)
                return default(TResult);

            VisitableExpression visitable = target as VisitableExpression;
            if (visitable != null)
                return visitable.Accept(this, state);

            switch (target.NodeType) {
                case ExprType.ArrayLength:
                case ExprType.Convert:
                case ExprType.ConvertChecked:
                case ExprType.Negate:
                case ExprType.NegateChecked:
                case ExprType.Not:
                case ExprType.Quote:
                case ExprType.TypeAs:
                case ExprType.UnaryPlus:
                    return RouteToVisitors ((UnaryExpression)target, state);
                case ExprType.Add:
                case ExprType.AddChecked:
                case ExprType.Divide:
                case ExprType.Modulo:
                case ExprType.Multiply:
                case ExprType.MultiplyChecked:
                case ExprType.Power:
                case ExprType.Subtract:
                case ExprType.SubtractChecked:
                case ExprType.And:
                case ExprType.Or:
                case ExprType.ExclusiveOr:
                case ExprType.LeftShift:
                case ExprType.RightShift:
                case ExprType.AndAlso:
                case ExprType.OrElse:
                case ExprType.Equal:
                case ExprType.NotEqual:
                case ExprType.GreaterThanOrEqual:
                case ExprType.GreaterThan:
                case ExprType.LessThan:
                case ExprType.LessThanOrEqual:
                case ExprType.Coalesce:
                case ExprType.ArrayIndex:
                    return RouteToVisitors ((BinaryExpression)target, state);
                case ExprType.Conditional:
                    return RouteToVisitors ((ConditionalExpression)target, state);
                case ExprType.Constant:
                    return RouteToVisitors ((ConstantExpression)target, state);
                case ExprType.Invoke:
                    return RouteToVisitors ((InvocationExpression)target, state);
                case ExprType.Lambda:
                    return RouteToVisitors ((LambdaExpression)target, state);
                case ExprType.MemberAccess:
                    return RouteToVisitors ((MemberExpression)target, state);
                case ExprType.Call:
                    return RouteToVisitors ((MethodCallExpression)target, state);
                case ExprType.New:
                    return RouteToVisitors ((NewExpression)target, state);
                case ExprType.NewArrayBounds:
                case ExprType.NewArrayInit:
                    return RouteToVisitors ((NewArrayExpression)target, state);
                case ExprType.MemberInit:
                    return RouteToVisitors ((MemberInitExpression)target, state);
                case ExprType.ListInit:
                    return RouteToVisitors ((ListInitExpression)target, state);
                case ExprType.Parameter:
                    return RouteToVisitors ((ParameterExpression)target, state);
                case ExprType.TypeIs:
                    return RouteToVisitors ((TypeBinaryExpression)target, state);
        
                default:
                    string message = string.Format("Unknown expression type {0} (NodeType: {1}).", target.GetType().FullName, target.NodeType.ToString());
                    throw new ArgumentException(message, "expression");
            }

        }
        #endregion
        
        private TResult RouteToVisitors<TExpression>(TExpression expression, IVisitState state) where TExpression : Expression
        {
            ICompositeVisitor<TResult, TExpression> visitor = this.GetVisitor<TExpression>(expression, _chain.Count);
            if(null == visitor)
            {
                string message = string.Format("No visitor for expression of type {0} (NodeType: {1}) has been registered in the composition named '{2}'.", typeof(TExpression).FullName, expression.NodeType.ToString(), _name);
                throw new ArgumentException(message, "expression");
            }
            return visitor.Visit(expression, state);
        }
        
        private void Register(VisitorBase visitor, out int compositionSize)
        {
            compositionSize = _chain.Count;
            _chain.Add(visitor);
        }
        
        private ICompositeVisitor<TResult, TExpression> GetVisitor<TExpression>(TExpression target, int startingPosition) where TExpression : Expression
        {
            ICompositeVisitor<TResult, TExpression> foundVisitor = null;
            
            while(startingPosition > 0)
            {
                --startingPosition;
                VisitorBase visitor = _chain[startingPosition];
                foundVisitor = visitor.AsVisitor<TExpression>(target);
                if(null != foundVisitor)
                    return foundVisitor;
            }
            string message = string.Format("No visitor available for the expression {0}.", target);
            throw new ArgumentException(message);
        }
        
        public abstract class VisitorBase : ICompositeVisitor<TResult>
        {
            private readonly int _nextVisitor;
            private readonly VisitorsComposition<TResult> _composition;
            
            protected ICompositeVisitor<TResult, TExpression> GetNextVisitor<TExpression>(TExpression target) where TExpression : Expression
            {
                return _composition.GetVisitor<TExpression>(target, _nextVisitor);
            }
            
            protected VisitorBase(VisitorsComposition<TResult> composition)
            {
                if(null == composition)
                    throw new ArgumentNullException("composition");
                _composition = composition;
                _composition.Register(this, out _nextVisitor);
            }
            
            protected internal virtual ICompositeVisitor<TResult, TExpression> AsVisitor<TExpression>(TExpression target) where TExpression : Expression
            {
                return this as ICompositeVisitor<TResult, TExpression>;
            }
            
            #region ICompositeVisitor implementation
            public ICompositeVisitor<TResult, TExpression> GetVisitor<TExpression> (TExpression target) where TExpression : System.Linq.Expressions.Expression
            {
                return _composition.GetVisitor<TExpression>(target);
            }
            #endregion
        }
    }
}

