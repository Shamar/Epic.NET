//  
//  RepositoryConstantVisitor.cs
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
using Epic;

namespace Epic.Linq.Expressions.Visit
{
    public sealed class RepositoryVisitor<TEntity, TIdentity> : VisitorsComposition<RelationExpression>.VisitorBase, ICompositeVisitor<RelationExpression, ConstantExpression>
        where TEntity : class
        where TIdentity : IEquatable<TIdentity>
    {
        private readonly Func<IRepository<TEntity, TIdentity>, RelationExpression> _transformation;
            
        public RepositoryVisitor (VisitorsComposition<RelationExpression> chain, Func<IRepository<TEntity, TIdentity>, RelationExpression> transformation)
            : base(chain)
        {
            if(null == transformation)
                throw new ArgumentNullException("transformation");
            _transformation = transformation;
        }

        #region ICompositeVisitor[ConstantExpression] implementation
        public RelationExpression Visit (ConstantExpression target, IVisitState state)
        {
            return _transformation(target.Value as IRepository<TEntity, TIdentity>);
        }
        #endregion
  
        internal protected override ICompositeVisitor<RelationExpression, TExpression> AsVisitor<TExpression> (TExpression target)
        {
            ICompositeVisitor<RelationExpression, TExpression> visitor = base.AsVisitor (target);
            if(null != visitor)
            {
                ConstantExpression expression = target as ConstantExpression;
                IRepository<TEntity, TIdentity> repository = expression.Value as IRepository<TEntity, TIdentity>;
                if (null == repository)
                    return null;
            }
            return visitor;
        }
    }
}

