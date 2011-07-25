//  
//  TemplateVisitor.cs
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

namespace Epic.Linq.Expressions.Templates
{
    public sealed class TemplateVisitor<TTemplateExpression> : VisitorsComposition.VisitorBase, ICompositeVisitor<MethodCallExpression>
        where TTemplateExpression : Expression
    {
        private readonly VisitorWrapper<TTemplateExpression> _initializer;
        
        public TemplateVisitor (VisitorsComposition composition)
            : base(composition)
        {
            _initializer = new VisitorWrapper<TTemplateExpression>(this, InitializeVisit);
        }
        
        internal protected override ICompositeVisitor<TExpression> AsVisitor<TExpression> (TExpression target)
        {
            ICompositeVisitor<TExpression> visitor = base.AsVisitor<TExpression>(target);
            if(null == visitor || !IsQueryAccess(target as MethodCallExpression))
            {
                visitor = _initializer as ICompositeVisitor<TExpression>;
                if(null == visitor)
                {
                    visitor = new VisitorWrapper<TExpression>(this, this.Parse<TExpression>);
                }
            }
            return visitor;
        }
        
        private static bool IsQueryAccess(MethodCallExpression call)
        {
            // the argument "call" can't be null, no need to check
            
            if(null == call.Object)
                return false;
            return typeof(IQuery).Equals(call.Object.Type);
        }
                
        private Expression InitializeVisit(TTemplateExpression expression, IVisitState state)
        {
            state = state.Add<QueryDataExtractor<TTemplateExpression>>(new QueryDataExtractor<TTemplateExpression>());
            state = state.Add<ExpressionPath<TTemplateExpression>>(new ExpressionPath<TTemplateExpression>(e => e));
            ICompositeVisitor<TTemplateExpression> nextVisitor = GetNextVisitor<TTemplateExpression>(expression);
            return nextVisitor.Visit(expression, state);
        }
        
        private Expression Parse<TExpression>(TExpression target, IVisitState state) where TExpression : Expression
        {
            ExpressionPath<TExpression> path = null;
            state.TryGet<ExpressionPath<TExpression>>(out path);
            
            // TODO : visit logic here
            
            ICompositeVisitor<TExpression> nextVisitor = GetNextVisitor<TExpression>(target);
            return nextVisitor.Visit(target, state);
        }

        #region ICompositeVisitor[MethodCallExpression] implementation
        public Expression Visit (MethodCallExpression target, IVisitState state)
        {
            // read argument[0]
            ConstantExpression nameExpression = target.Arguments[0] as ConstantExpression; // TODO : handle closures
            string name = nameExpression.Value as string;
            
            // register the path to here in the extractor
            QueryDataExtractor<TTemplateExpression> extractor = null;
            state.TryGet<QueryDataExtractor<TTemplateExpression>>(out extractor);
            ExpressionPath<MethodCallExpression> path = null;
            state.TryGet<ExpressionPath<MethodCallExpression>>(out path);
            
            path.Register(name, extractor);
           
            return target;
        }
        #endregion
        
        
        class ExpressionPath<TExpression>
            where TExpression : Expression
        {
            private readonly Expression<Func<TTemplateExpression, TExpression>> Path;
            
            public ExpressionPath(Expression<Func<TTemplateExpression, TExpression>> path)
            {
                Path = path;
            }
            
            public ExpressionPath<TNextExpression> Add<TNextExpression>(Expression<Func<TExpression, TNextExpression>> nextStep) where TNextExpression : Expression
            {
                VisitorsComposition composition = new VisitorsComposition();
                new ExpressionReplacingVisitor<ParameterExpression>(composition, nextStep.Parameters[0], Path.Body);
                UnvisitableExpressionAdapter adapter = new UnvisitableExpressionAdapter(nextStep.Body);
            
                Expression lambdaBody = adapter.Accept(composition, VisitState.New);

                Expression<Func<TTemplateExpression, TNextExpression>> nextPath = 
                    Expression.Lambda<Func<TTemplateExpression, TNextExpression>>(lambdaBody, Path.Parameters);
                
                return new ExpressionPath<TNextExpression>(nextPath);    
            }
            
            public void Register(string name, QueryDataExtractor<TTemplateExpression> dataExtractor)
            {
                dataExtractor.Register<TExpression>(name, Path);
            }
        }
    }
    
}

