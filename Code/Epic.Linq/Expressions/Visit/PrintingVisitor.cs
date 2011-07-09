//  
//  PrintingVisitor.cs
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
    public class PrintingVisitor : CompositeVisitorBase, 
        ICompositeVisitor<UnaryExpression>, 
        ICompositeVisitor<BinaryExpression>, 
        ICompositeVisitor<ConditionalExpression>,
        ICompositeVisitor<ConstantExpression>,
        ICompositeVisitor<InvocationExpression>,
        ICompositeVisitor<LambdaExpression>,
        ICompositeVisitor<MemberExpression>,
        ICompositeVisitor<MethodCallExpression>,
        ICompositeVisitor<NewExpression>,
        ICompositeVisitor<NewArrayExpression>,
        ICompositeVisitor<MemberInitExpression>,
        ICompositeVisitor<ListInitExpression>,
        ICompositeVisitor<ParameterExpression>,
        ICompositeVisitor<TypeBinaryExpression>
    {
        private readonly Stack<Expression> _stack;
        
        public PrintingVisitor (CompositeVisitorChain chain)
            : base(chain)
        {
            _stack = new Stack<Expression>();
        }
        
        private void Display(Expression expression)
        {
            for(int i = 0; i < _stack.Count; ++i)
            {
                Console.Write("    ");
            }
            Console.WriteLine("{0} - {1}", expression.NodeType, expression.GetType().FullName);
        }
        
        protected override ICompositeVisitor<TExpression> AsVisitor<TExpression> (TExpression target)
        {
            ICompositeVisitor<TExpression> visitor = base.AsVisitor<TExpression>(target);
            if(null == visitor || (_stack.Count > 0 && object.ReferenceEquals(target, _stack.Peek())))
            {
                return null;
            }
            return visitor;
        }
        
        #region ICompositeVisitor[UnaryExpression] implementation
        System.Linq.Expressions.Expression ICompositeVisitor<UnaryExpression>.Visit (UnaryExpression target)
        {
            Display(target);
            _stack.Push(target);
            var visitor = GetVisitor(target);
            if(null == visitor)
                return target;
            Expression visited = visitor.Visit(target);
            _stack.Pop();
            return visited;
        }
        #endregion
  
        #region ICompositeVisitor[BinaryExpression] implementation
        System.Linq.Expressions.Expression ICompositeVisitor<BinaryExpression>.Visit (BinaryExpression target)
        {
            Display(target);
            _stack.Push(target);
            var visitor = GetVisitor(target);
            if(null == visitor)
                return target;
            Expression visited = visitor.Visit(target);
            _stack.Pop();
            return visited;
        }
        #endregion
        
        #region ICompositeVisitor[ConditionalExpression] implementation
        System.Linq.Expressions.Expression ICompositeVisitor<ConditionalExpression>.Visit (ConditionalExpression target)
        {
            Display(target);
            _stack.Push(target);
            var visitor = GetVisitor(target);
            if(null == visitor)
                return target;
            Expression visited = visitor.Visit(target);
            _stack.Pop();
            return visited;
        }
        #endregion
        
        #region ICompositeVisitor[ConstantExpression] implementation
        System.Linq.Expressions.Expression ICompositeVisitor<ConstantExpression>.Visit (ConstantExpression target)
        {
            Display(target);
            _stack.Push(target);
            var visitor = GetVisitor(target);
            if(null == visitor)
                return target;
            Expression visited = visitor.Visit(target);
            _stack.Pop();
            return visited;
        }
        #endregion
  
        #region ICompositeVisitor[InvocationExpression] implementation
        System.Linq.Expressions.Expression ICompositeVisitor<InvocationExpression>.Visit (InvocationExpression target)
        {
            Display(target);
            _stack.Push(target);
            var visitor = GetVisitor(target);
            if(null == visitor)
                return target;
            Expression visited = visitor.Visit(target);
            _stack.Pop();
            return visited;
        }
        #endregion
        
        #region ICompositeVisitor[LambdaExpression] implementation
        System.Linq.Expressions.Expression ICompositeVisitor<LambdaExpression>.Visit (LambdaExpression target)
        {
            Display(target);
            _stack.Push(target);
            var visitor = GetVisitor(target);
            if(null == visitor)
                return target;
            Expression visited = visitor.Visit(target);
            _stack.Pop();
            return visited;
        }
        #endregion
        
        #region ICompositeVisitor[MemberExpression] implementation
        System.Linq.Expressions.Expression ICompositeVisitor<MemberExpression>.Visit (MemberExpression target)
        {
            Display(target);
            _stack.Push(target);
            var visitor = GetVisitor(target);
            Expression visited = visitor.Visit(target);
            _stack.Pop();
            return visited;
        }
        #endregion
  
        #region ICompositeVisitor[MethodCallExpression] implementation
        System.Linq.Expressions.Expression ICompositeVisitor<MethodCallExpression>.Visit (MethodCallExpression target)
        {
            Display(target);
            _stack.Push(target);
            var visitor = GetVisitor(target);
            if(null == visitor)
                return target;
            Expression visited = visitor.Visit(target);
            _stack.Pop();
            return visited;
        }
        #endregion
        
        #region ICompositeVisitor[NewExpression] implementation
        System.Linq.Expressions.Expression ICompositeVisitor<NewExpression>.Visit (NewExpression target)
        {
            Display(target);
            _stack.Push(target);
            var visitor = GetVisitor(target);
            if(null == visitor)
                return target;
            Expression visited = visitor.Visit(target);
            _stack.Pop();
            return visited;
        }
        #endregion
        
        #region ICompositeVisitor[NewArrayExpression] implementation
        System.Linq.Expressions.Expression ICompositeVisitor<NewArrayExpression>.Visit (NewArrayExpression target)
        {
            Display(target);
            _stack.Push(target);
            var visitor = GetVisitor(target);
            if(null == visitor)
                return target;
            Expression visited = visitor.Visit(target);
            _stack.Pop();
            return visited;
        }
        #endregion
  
        #region ICompositeVisitor[MemberInitExpression] implementation
        System.Linq.Expressions.Expression ICompositeVisitor<MemberInitExpression>.Visit (MemberInitExpression target)
        {
            Display(target);
            _stack.Push(target);
            var visitor = GetVisitor(target);
            if(null == visitor)
                return target;
            Expression visited = visitor.Visit(target);
            _stack.Pop();
            return visited;
        }
        #endregion
        
        #region ICompositeVisitor[ListInitExpression] implementation
        System.Linq.Expressions.Expression ICompositeVisitor<ListInitExpression>.Visit (ListInitExpression target)
        {
            Display(target);
            _stack.Push(target);
            var visitor = GetVisitor(target);
            if(null == visitor)
                return target;
            Expression visited = visitor.Visit(target);
            _stack.Pop();
            return visited;
        }
        #endregion
        
        #region ICompositeVisitor[ParameterExpression] implementation
        System.Linq.Expressions.Expression ICompositeVisitor<ParameterExpression>.Visit (ParameterExpression target)
        {
            Display(target);
            _stack.Push(target);
            var visitor = GetVisitor(target);
            if(null == visitor)
                return target;
            Expression visited = visitor.Visit(target);
            _stack.Pop();
            return visited;
        }
        #endregion
  
        #region ICompositeVisitor[TypeBinaryExpression] implementation
        System.Linq.Expressions.Expression ICompositeVisitor<TypeBinaryExpression>.Visit (TypeBinaryExpression target)
        {
            Display(target);
            _stack.Push(target);
            var visitor = GetVisitor(target);
            if(null == visitor)
                return target;
            Expression visited = visitor.Visit(target);
            _stack.Pop();
            return visited;
        }
        #endregion
    }
}

