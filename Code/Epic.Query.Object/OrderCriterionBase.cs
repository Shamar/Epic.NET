//  
//  OrderCriterionBase.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010-2012 Giacomo Tesio
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
using System.Runtime.Serialization;


namespace Epic.Query.Object
{
    [Serializable]
    public abstract class OrderCriterionBase<TEntity, TOrderCriterion> : OrderCriterion<TEntity>
        where TOrderCriterion : OrderCriterion<TEntity>
    {
        protected OrderCriterionBase ()
            : base()
        {
            if(this.GetType() != typeof(TOrderCriterion))
            {
                string message = string.Format("The order criterion {0} must extend OrderCriterionBase<{1}, {0}> (it extends OrderCriterionBase<{1}, {2}> instead).", this.GetType(), typeof(TEntity), typeof(TOrderCriterion));
                throw new EpicException(message);
            }
        }

        protected override sealed bool SafeEquals (OrderCriterion<TEntity> other)
        {
            TOrderCriterion otherCriterion = other as TOrderCriterion;
            // we don't need to check the cast again, since it's granted from the OrderCriterionBase 
            // constructor and the last OrderCriterion.Equals() check.

            return SafeEquals(otherCriterion);
        }

        protected abstract bool SafeEquals (TOrderCriterion other);

        public override sealed OrderCriterion<TEntity> Chain(OrderCriterion<TEntity> other)
        {
            return new OrderCriteria<TEntity>(this, other);
        }

        public override sealed OrderCriterion<TEntity> Reverse()
        {
            return new ReverseOrder<TEntity>(this);
        }

        protected OrderCriterionBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

