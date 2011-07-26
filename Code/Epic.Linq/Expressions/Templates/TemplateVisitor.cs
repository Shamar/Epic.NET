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
using System.Collections.Generic;

namespace Epic.Linq.Expressions.Templates
{
    public sealed class TemplateVisitor<TTemplateExpression> : ICompositeVisitor<TTemplateExpression>
        where TTemplateExpression : Expression
    {
        public TemplateVisitor ()
        {
        }
        
        private static bool IsQueryAccess(MethodCallExpression call)
        {
            // the argument "call" can't be null, no need to check
            
            if(null == call.Object)
                return false;
            return typeof(IQuery).Equals(call.Object.Type);
        }
                
        #region tracking path methods
        
        private static Expression ParseUnaryExpression(UnaryExpression expression, IVisitState state)
        {
            return expression;
        }
        
        private static Expression ParseBinaryExpression(BinaryExpression expression, IVisitState state)
        {
            throw new NotImplementedException();
        }

        private static Expression ParseConditionalExpression (ConditionalExpression expression, IVisitState state)
        {
            throw new NotImplementedException ();
        }

        private static Expression ParseConstantExpression (ConstantExpression expression, IVisitState state)
        {
            throw new NotImplementedException ();
        }

        private static Expression ParseInvocationExpression (InvocationExpression expression, IVisitState state)
        {
            throw new NotImplementedException ();
        }

        private static Expression ParseLambdaExpression (LambdaExpression expression, IVisitState state)
        {
            throw new NotImplementedException ();
        }

        private static Expression ParseMemberExpression (MemberExpression expression, IVisitState state)
        {
            throw new NotImplementedException ();
        }

        private static Expression ParseMethodCallExpression (MethodCallExpression expression, IVisitState state)
        {
            if(IsQueryAccess(expression))
            {
                // read argument[0]
                ConstantExpression nameExpression = expression.Arguments[0] as ConstantExpression; // TODO : handle closures
                string name = nameExpression.Value as string;
                
                // register the path to here in the extractor
                QueryDataExtractor<TTemplateExpression> extractor = null;
                state.TryGet<QueryDataExtractor<TTemplateExpression>>(out extractor);
                ExpressionPath<MethodCallExpression> path = null;
                state.TryGet<ExpressionPath<MethodCallExpression>>(out path);
                
                path.Register(name, extractor);
            }
            else
            {
                // Parse the MethodCallExpression
            }
            return expression;
        }

        private static Expression ParseNewExpression (NewExpression expression, IVisitState state)
        {
            throw new NotImplementedException ();
        }

        private static Expression ParseMemberInitExpression (MemberInitExpression expression, IVisitState state)
        {
            throw new NotImplementedException ();
        }

        private static Expression ParseNewArrayExpression (NewArrayExpression expression, IVisitState state)
        {
            throw new NotImplementedException ();
        }

        private static Expression ParseListInitExpression (ListInitExpression expression, IVisitState state)
        {
            throw new NotImplementedException ();
        }

        private static Expression ParseParameterExpression (ParameterExpression expression, IVisitState state)
        {
            throw new NotImplementedException ();
        }

        private static Expression ParseTypeBinaryExpression (TypeBinaryExpression expression, IVisitState state)
        {
            throw new NotImplementedException ();
        }
        
        private static Expression ParseExpression(Expression expression, IVisitState state)
        {
            ExpressionPath<Expression> path = null;
            state.TryGet<ExpressionPath<Expression>>(out path);
            
            switch (expression.NodeType) {
                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                case ExpressionType.UnaryPlus:
                    state = state.Add<ExpressionPath<UnaryExpression>>(path.Add<UnaryExpression>(e => e.NodeType == expression.NodeType ? e as UnaryExpression : null));
                    return ParseUnaryExpression((UnaryExpression)expression, state);
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
                    return ParseBinaryExpression ((BinaryExpression)expression, state);
                case ExpressionType.Conditional:
                    return ParseConditionalExpression ((ConditionalExpression)expression, state);
                case ExpressionType.Constant:
                    return ParseConstantExpression ((ConstantExpression)expression, state);
                case ExpressionType.Invoke:
                    return ParseInvocationExpression ((InvocationExpression)expression, state);
                case ExpressionType.Lambda:
                    return ParseLambdaExpression ((LambdaExpression)expression, state);
                case ExpressionType.MemberAccess:
                    return ParseMemberExpression ((MemberExpression)expression, state);
                case ExpressionType.Call:
                    return ParseMethodCallExpression ((MethodCallExpression)expression, state);
                case ExpressionType.New:
                    return ParseNewExpression ((NewExpression)expression, state);
                case ExpressionType.NewArrayBounds:
                case ExpressionType.NewArrayInit:
                    return ParseNewArrayExpression ((NewArrayExpression)expression, state);
                case ExpressionType.MemberInit:
                    return ParseMemberInitExpression ((MemberInitExpression)expression, state);
                case ExpressionType.ListInit:
                    return ParseListInitExpression ((ListInitExpression)expression, state);
                case ExpressionType.Parameter:
                    return ParseParameterExpression ((ParameterExpression)expression, state);
                case ExpressionType.TypeIs:
                    return ParseTypeBinaryExpression ((TypeBinaryExpression)expression, state);
            }
            
            return expression;
        }
        
        #endregion tracking path methods

        #region ICompositeVisitor[MethodCallExpression] implementation
        public Expression Visit (TTemplateExpression target, IVisitState state)
        {
            state = state.Add<QueryDataExtractor<TTemplateExpression>>(new QueryDataExtractor<TTemplateExpression>());
            state = state.Add<ExpressionPath<TTemplateExpression>>(new ExpressionPath<TTemplateExpression>(e => e));
            return ParseExpression(target, state);
        }
        #endregion
        
        #region ICompositeVisitor implementation
        public ICompositeVisitor<TRequiredExpression> GetVisitor<TRequiredExpression> (TRequiredExpression target) where TRequiredExpression : Expression
        {
            return this as ICompositeVisitor<TRequiredExpression>;
        }
        #endregion
        
        sealed class ExpressionPath<TExpression>
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

