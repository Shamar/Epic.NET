//  
//  RepositoryTranslator.cs
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
using Epic.Linq.Expressions.Visit;
using Epic.Linq.Expressions;

namespace Epic.Linq.Translators
{
    public class RepositoryTranslator<TEntity, TIdentity>  : CompositeVisitorBase, ICompositeVisitor<ConstantExpression>
        where TEntity : class
        where TIdentity : IEquatable<TIdentity>
    {
        public RepositoryTranslator (CompositeVisitorChain chain)
            : base(chain)
        {
        }

        #region ICompositeVisitor[ConstantExpression] implementation
        public Expression Visit (ConstantExpression target, IVisitState state)
        {
            return new QueryExpression(typeof(TEntity), typeof(TEntity).Name, new SourceExpression(typeof(TEntity), typeof(TEntity).Name));
        }
        #endregion
   
        protected override ICompositeVisitor<TExpression> AsVisitor<TExpression> (TExpression target, IVisitState state)
        {
            ConstantExpression exp = target as ConstantExpression;
            if(null != exp)
            {
                IRepository<TEntity, TIdentity> repository = exp.Value as IRepository<TEntity, TIdentity>;
                if(null == repository)
                {
                    return null;
                }
            }
            return base.AsVisitor (target, state);
        }
    }
}

