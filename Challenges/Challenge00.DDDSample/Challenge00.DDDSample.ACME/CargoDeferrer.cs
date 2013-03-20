//
//  CargoDeferrer.cs
//
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
//
//  Copyright (c) 2010-2013 Giacomo Tesio
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
using Epic.Query.Object;
using Epic.Query.Object.Expressions;
using Epic.Math;
using System.Collections.Generic;
using Challenge00.DDDSample.Cargo;

namespace Challenge00.DDDSample.ACME
{

    public class CargoDeferrer : IDeferrer, IMapping<Expression<IEnumerable<ICargo>>, IEnumerable<ICargo>>
     
        private readonly IVisitor<SqlExpression, IVisitable> _visitor;
        public CargoDeferrer()
        {
        }

        #region IDeferrer implementation

        public TDeferred Defer<TDeferred, TResult>(Expression<TResult> expression) where TDeferred : IDeferred<TResult>
        {
            throw new NotImplementedException();
        }

        public TResult Evaluate<TResult>(Expression<TResult> expression)
        { 
            if (null == expression)
                throw new ArgumentNullException("expression");
            try
            {
                IMapping<Expression<TResult>, TResult> mapping = this as IMapping<Expression<TResult>, TResult>;
                return mapping.ApplyTo(expression);
            }
            catch(Exception e)
            {
                string message = string.Format("An exception occurred during the evaluation of a {0}.", expression.GetType());
                throw new DeferredEvaluationException<TResult>(expression, message, e);
            }

        }

        #endregion

        #region IMapping implementation

        public IEnumerable<ICargo> ApplyTo(Expression<IEnumerable<ICargo>> element)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}

