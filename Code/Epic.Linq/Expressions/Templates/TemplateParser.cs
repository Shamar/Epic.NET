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
    public static class TemplateParser<TTemplateExpression>
        where TTemplateExpression : Expression
    {
        private static bool IsQueryAccess(MethodCallExpression call)
        {
            // the argument "call" can't be null, no need to check
            
            if(null == call.Object)
                return false;
            return typeof(IQuery).Equals(call.Object.Type);
        }
                
        #region tracking path methods
        
        private static bool MatchUnaryExpression(UnaryExpression expression, UnaryExpression template)
        {
            if(null == expression)
                return false;
            if(template.IsLifted != expression.IsLifted || template.IsLiftedToNull != expression.IsLiftedToNull)
                return false;
            return template.Method.Equals(expression.Method);
        }
        
        private static void ParseUnaryExpression(UnaryExpression expression, IVisitState state)
        {
            ExpressionPath<UnaryExpression> path = null;
            state.TryGet<ExpressionPath<UnaryExpression>>(out path);
            ParseExpression(expression.Operand, state.Add(path.Add<Expression>(e => MatchUnaryExpression(e, expression) ? e.Operand : null)));
        }
        
        private static bool MatchBinaryExpression(BinaryExpression expression, BinaryExpression template)
        {
            if(null == expression)
                return false;
            if(template.IsLifted != expression.IsLifted || template.IsLiftedToNull != expression.IsLiftedToNull)
                return false;
            return template.Method.Equals(expression.Method);
        }
        
        private static void ParseBinaryExpression(BinaryExpression expression, IVisitState state)
        {
            ExpressionPath<BinaryExpression> path = null;
            state.TryGet<ExpressionPath<BinaryExpression>>(out path);
            ParseExpression (expression.Left, state.Add(path.Add(e => MatchBinaryExpression(e, expression) ? e.Left : null)));
            ParseExpression (expression.Right, state.Add(path.Add(e => MatchBinaryExpression(e, expression) ? e.Right : null)));
            ParseExpression (expression.Conversion, state.Add(path.Add(e => MatchBinaryExpression(e, expression) ? e.Conversion : null)));
        }
  
        private static void ParseConditionalExpression (ConditionalExpression expression, IVisitState state)
        {
            ExpressionPath<ConditionalExpression> path = null;
            state.TryGet<ExpressionPath<ConditionalExpression>>(out path);
            ParseExpression (expression.Test, state.Add(path.Add(e => null == e ? null : e.Test)));
            ParseExpression (expression.IfFalse, state.Add(path.Add(e => null == e ? null : e.IfFalse)));
            ParseExpression (expression.IfTrue, state.Add(path.Add(e => null == e ? null : e.IfTrue)));
        }
        

        private static void ParseConstantExpression (ConstantExpression expression, IVisitState state)
        {
            // nothing to do ? ? ? what about repositories and IQueryable<T> wrapped in constant expressions?
            /*
            ExpressionPath<ConstantExpression> path = null;
            state.TryGet<ExpressionPath<ConstantExpression>>(out path);
            ParseExpression(expression.Value, state.Add(path.Add(e => null == e ? null : e.Value)));
            */
        }
  
        private static bool MatchInvocationExpression(InvocationExpression expression, InvocationExpression template)
        {
            if(null == expression)
                return false;
            return template.Arguments.Count == expression.Arguments.Count;
        }
        
        private static void ParseInvocationExpression (InvocationExpression expression, IVisitState state)
        {
            ExpressionPath<InvocationExpression> path = null;
            state.TryGet<ExpressionPath<InvocationExpression>>(out path);
            
            ParseExpression (expression.Expression, state.Add(path.Add(e => MatchInvocationExpression(e, expression) ? e.Expression : null)));
   
            int i = 0;
            while(i < expression.Arguments.Count)
            {
                ParseExpression(expression.Arguments[i], state.Add(path.Add(e => MatchInvocationExpression(e, expression) ? expression.Arguments[i] : null)));
                ++i;
            }
        }
        
        private static bool MatchLambdaExpression(LambdaExpression expression, LambdaExpression template)
        {
            if(null == expression)
                return false;
            return template.Parameters.Count == expression.Parameters.Count;
        }

        private static void ParseLambdaExpression (LambdaExpression expression, IVisitState state)
        {
            ExpressionPath<LambdaExpression> path = null;
            state.TryGet<ExpressionPath<LambdaExpression>>(out path);
            
            int i = 0;
            while(i < expression.Parameters.Count)
            {
                ParseParameterExpression(expression.Parameters[i], state.Add(path.Add(e => MatchLambdaExpression(e, expression) ? expression.Parameters[i] : null)));
                ++i;
            }
            ParseExpression(expression.Body, state.Add(path.Add(e => MatchLambdaExpression(e, expression) ? e.Body : null)));
        }
        
        private static bool MatchMemberExpression(MemberExpression expression, MemberExpression template)
        {
            if(null == expression)
                return false;
            return template.Member.Equals(expression.Member);
        }

        private static void ParseMemberExpression (MemberExpression expression, IVisitState state)
        {
            ExpressionPath<MemberExpression> path = null;
            state.TryGet<ExpressionPath<MemberExpression>>(out path);
            
            ParseExpression(expression.Expression, state.Add(path.Add(e => MatchMemberExpression(e, expression) ? e.Expression : null)));
        }
  
        private static bool MatchMethodCall(MethodCallExpression expression, MethodCallExpression template)
        {
            if(null == expression)
                return false;
            if(template.Arguments.Count != expression.Arguments.Count)
                return false;
            return template.Method.Equals(expression.Method);
        }
        
        private static void ParseMethodCallExpression (MethodCallExpression expression, IVisitState state)
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
                ExpressionPath<MethodCallExpression> path = null;
                state.TryGet<ExpressionPath<MethodCallExpression>>(out path);
                
                ParseExpression(expression.Object, state.Add(path.Add(e => MatchMethodCall(e, expression) ? e.Object : null)));
                
                int i = 0;
                while(i < expression.Arguments.Count)
                {
                    ParseExpression(expression.Arguments[i], state.Add(path.Add(e => MatchMethodCall(e, expression) ? expression.Arguments[i] : null)));
                    ++i;
                }
            }
        }

        private static void ParseNewExpression (NewExpression expression, IVisitState state)
        {
            // nothing to do ? ? ?
        }
  
        private static bool MatchMemberInit(MemberInitExpression expression, MemberInitExpression template)
        {
            if(null == expression)
                return false;
            if(template.Bindings.Count != expression.Bindings.Count)
                return false;
            if(!template.NewExpression.Constructor.Equals(expression.NewExpression.Constructor))
                return false;
            
            return true;
        }
        
        private static void ParseMemberInitExpression (MemberInitExpression expression, IVisitState state)
        {
            ExpressionPath<MemberInitExpression> path = null;
            state.TryGet<ExpressionPath<MemberInitExpression>>(out path);
            
            ParseNewExpression(expression.NewExpression, state.Add(path.Add(e => MatchMemberInit(e, expression) ? e.NewExpression : null)));
            
            /* nothing to do ? ? ?
            int i = 0;
            while(i < expression.Bindings.Count)
            {
                ++i;
            }
            */
        }
        
        private static bool MatchNewArrayExpression(NewArrayExpression expression, NewArrayExpression template)
        {
            if(null == expression)
                return false;
            return template.Expressions.Count == expression.Expressions.Count;
        }

        private static void ParseNewArrayExpression (NewArrayExpression expression, IVisitState state)
        {
            ExpressionPath<NewArrayExpression> path = null;
            state.TryGet<ExpressionPath<NewArrayExpression>>(out path);
            
            int i = 0;
            while(i < expression.Expressions.Count)
            {
                ParseExpression(expression.Expressions[i], state.Add(path.Add(e => MatchNewArrayExpression(e, expression) ? e.Expressions[i] : null)));
                ++i;
            }
        }
        
        private static bool MatchListInitExpression(ListInitExpression expression, ListInitExpression template)
        {
            if(null == expression)
                return false;
            if(template.Initializers.Count != expression.Initializers.Count)
                return false;
            
            int i = 0;
            while(i < expression.Initializers.Count)
            {
                ElementInit templateInit = template.Initializers[i];
                ElementInit expressionInit = template.Initializers[i];
                if(templateInit.Arguments.Count != expressionInit.Arguments.Count || !templateInit.AddMethod.Equals(expressionInit.AddMethod))
                    return false;
                ++i;
            }
            
            return true;
        }

        private static void ParseListInitExpression (ListInitExpression expression, IVisitState state)
        {
            ExpressionPath<ListInitExpression> path = null;
            state.TryGet<ExpressionPath<ListInitExpression>>(out path);
            
            ParseNewExpression(expression.NewExpression, state.Add(path.Add(e => MatchListInitExpression(e, expression) ? e.NewExpression : null)));
            
            int i = 0;
            while(i < expression.Initializers.Count)
            {
                int j = 0;
                while(j < expression.Initializers[i].Arguments.Count)
                {
                    ParseExpression(expression.Initializers[i].Arguments[j], state.Add(path.Add(e => MatchListInitExpression(e, expression) ? e.Initializers[i].Arguments[j] : null)));
                }
                ++i;
            }            
        }

        private static void ParseParameterExpression (ParameterExpression expression, IVisitState state)
        {
            // nothing to do ? ? ?
        }
  
        private static bool MatchTypeBinaryExpression(TypeBinaryExpression expression, TypeBinaryExpression template)
        {
            if(null == expression)
                return false;
            return template.TypeOperand.Equals(expression.TypeOperand);
        }
        
        private static void ParseTypeBinaryExpression (TypeBinaryExpression expression, IVisitState state)
        {
            ExpressionPath<TypeBinaryExpression> path = null;
            state.TryGet<ExpressionPath<TypeBinaryExpression>>(out path);
            ParseExpression(expression.Expression, state.Add(path.Add(e => MatchTypeBinaryExpression(e, expression) ? e.Expression : null)));
        }
        
        private static bool MatchExpression(Expression expression, Expression template)
        {
            if(null == expression)
                return false;
            
            return template.NodeType == expression.NodeType && template.Type.Equals(expression.Type);
        }
        
        private static IVisitState CastNodeType<TExpression>(IVisitState state, Expression template) where TExpression : Expression
        {
            ExpressionPath<Expression> path = null;
            state.TryGet<ExpressionPath<Expression>>(out path);
            return state.Add(path.Add<TExpression>(e => MatchExpression(e, template) ? e as TExpression : null));
        }
        
        private static void ParseExpression(Expression expression, IVisitState state)
        {
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
                    ParseUnaryExpression((UnaryExpression)expression, CastNodeType<UnaryExpression>(state, expression));
                break;
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
                    ParseBinaryExpression((BinaryExpression)expression, CastNodeType<BinaryExpression>(state, expression));
                break;
                case ExpressionType.Conditional:
                    ParseConditionalExpression((ConditionalExpression)expression, CastNodeType<ConditionalExpression>(state, expression));
                break;
                case ExpressionType.Constant:
                    ParseConstantExpression((ConstantExpression)expression, CastNodeType<ConstantExpression>(state, expression));
                break;
                case ExpressionType.Invoke:
                    ParseInvocationExpression((InvocationExpression)expression, CastNodeType<InvocationExpression>(state, expression));
                break;
                case ExpressionType.Lambda:
                    ParseLambdaExpression((LambdaExpression)expression, CastNodeType<LambdaExpression>(state, expression));
                break;
                case ExpressionType.MemberAccess:
                    ParseMemberExpression((MemberExpression)expression, CastNodeType<MemberExpression>(state, expression));
                break;
                case ExpressionType.Call:
                    ParseMethodCallExpression((MethodCallExpression)expression, CastNodeType<MethodCallExpression>(state, expression));
                break;
                case ExpressionType.New:
                    ParseNewExpression((NewExpression)expression, CastNodeType<NewExpression>(state, expression));
                break;
                case ExpressionType.NewArrayBounds:
                case ExpressionType.NewArrayInit:
                    ParseNewArrayExpression((NewArrayExpression)expression, CastNodeType<NewArrayExpression>(state, expression));
                break;
                case ExpressionType.MemberInit:
                    ParseMemberInitExpression((MemberInitExpression)expression, CastNodeType<MemberInitExpression>(state, expression));
                break;
                case ExpressionType.ListInit:
                    ParseListInitExpression((ListInitExpression)expression, CastNodeType<ListInitExpression>(state, expression));
                break;
                case ExpressionType.Parameter:
                    ParseParameterExpression((ParameterExpression)expression, CastNodeType<ParameterExpression>(state, expression));
                break;
                case ExpressionType.TypeIs:
                    ParseTypeBinaryExpression((TypeBinaryExpression)expression, CastNodeType<TypeBinaryExpression>(state, expression));
                break;
            }
        }
        
        #endregion tracking path methods

        public static QueryDataExtractor<TTemplateExpression> Parse(TTemplateExpression template)
        {
            if(null == template)
                throw new ArgumentNullException("template");
            
            IVisitState state = VisitState.New;
            QueryDataExtractor<TTemplateExpression> extractor = new QueryDataExtractor<TTemplateExpression>();
            
            state = state.Add<QueryDataExtractor<TTemplateExpression>>(extractor);
            state = state.Add<ExpressionPath<TTemplateExpression>>(new ExpressionPath<TTemplateExpression>(e => e));
            ParseExpression(template, state);
            
            return extractor;
        }
        
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

