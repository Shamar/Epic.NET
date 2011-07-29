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

namespace Epic.Linq.Expressions.Visit
{
    public sealed class VisitorsComposition : ICompositeVisitor<Expression>
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
        public ICompositeVisitor<TExpression> GetVisitor<TExpression> (TExpression target) where TExpression : System.Linq.Expressions.Expression
        {
            ICompositeVisitor<TExpression> visitor = this as ICompositeVisitor<TExpression>;
            if(null != visitor)
                return visitor;
            return GetVisitor<TExpression>(target, _chain.Count);
        }
        #endregion

        #region ICompositeVisitor[Expression] implementation
        public Expression Visit (Expression target, IVisitState state)
        {
            if (target == null)
                return null;

            VisitableExpression visitable = target as VisitableExpression;
            if (visitable != null)
                return visitable.Accept(this, state);

            switch (target.NodeType) {
                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                case ExpressionType.UnaryPlus:
                    return RouteToVisitors ((UnaryExpression)target, state);
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
                    return RouteToVisitors ((BinaryExpression)target, state);
                case ExpressionType.Conditional:
                    return RouteToVisitors ((ConditionalExpression)target, state);
                case ExpressionType.Constant:
                    return RouteToVisitors ((ConstantExpression)target, state);
                case ExpressionType.Invoke:
                    return RouteToVisitors ((InvocationExpression)target, state);
                case ExpressionType.Lambda:
                    return RouteToVisitors ((LambdaExpression)target, state);
                case ExpressionType.MemberAccess:
                    return RouteToVisitors ((MemberExpression)target, state);
                case ExpressionType.Call:
                    return RouteToVisitors ((MethodCallExpression)target, state);
                case ExpressionType.New:
                    return RouteToVisitors ((NewExpression)target, state);
                case ExpressionType.NewArrayBounds:
                case ExpressionType.NewArrayInit:
                    return RouteToVisitors ((NewArrayExpression)target, state);
                case ExpressionType.MemberInit:
                    return RouteToVisitors ((MemberInitExpression)target, state);
                case ExpressionType.ListInit:
                    return RouteToVisitors ((ListInitExpression)target, state);
                case ExpressionType.Parameter:
                    return RouteToVisitors ((ParameterExpression)target, state);
                case ExpressionType.TypeIs:
                    return RouteToVisitors ((TypeBinaryExpression)target, state);
        
                default:
                    string message = string.Format("Unknown expression type {0} (NodeType: {1}).", target.GetType().FullName, target.NodeType.ToString());
                    throw new ArgumentException(message, "expression");
            }

        }
        #endregion
        
        private Expression RouteToVisitors<TExpression>(TExpression expression, IVisitState state) where TExpression : Expression
        {
            ICompositeVisitor<TExpression> visitor = this.GetVisitor<TExpression>(expression, _chain.Count);
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
        
        private static Expression IdentityVisit<TExpression>(TExpression expression, IVisitState state) where TExpression : Expression
        {
            return expression;
        }
        
        private ICompositeVisitor<TExpression> GetVisitor<TExpression>(TExpression target, int startingPosition) where TExpression : Expression
        {
            ICompositeVisitor<TExpression> foundVisitor = null;
            
            while(startingPosition > 0)
            {
                --startingPosition;
                VisitorBase visitor = _chain[startingPosition];
                foundVisitor = visitor.AsVisitor<TExpression>(target);
                if(null != foundVisitor)
                    return foundVisitor;
            }
            
            return new VisitorWrapper<TExpression>(this, IdentityVisit<TExpression>); // TODO : should we throw ???
        }
        
        public abstract class VisitorBase : ICompositeVisitor
        {
            private readonly int _nextVisitor;
            private readonly VisitorsComposition _composition;
            
            protected ICompositeVisitor<TExpression> GetNextVisitor<TExpression>(TExpression target) where TExpression : Expression
            {
                return _composition.GetVisitor<TExpression>(target, _nextVisitor);
            }
            
            protected VisitorBase(VisitorsComposition composition)
            {
                if(null == composition)
                    throw new ArgumentNullException("composition");
                _composition = composition;
                _composition.Register(this, out _nextVisitor);
            }
            
            protected internal virtual ICompositeVisitor<TExpression> AsVisitor<TExpression>(TExpression target) where TExpression : Expression
            {
                return this as ICompositeVisitor<TExpression>;
            }
            
            #region ICompositeVisitor implementation
            public ICompositeVisitor<TExpression> GetVisitor<TExpression> (TExpression target) where TExpression : System.Linq.Expressions.Expression
            {
                return _composition.GetVisitor<TExpression>(target);
            }
            #endregion
        }
    }
}

