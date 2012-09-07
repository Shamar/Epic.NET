//  
//  FakeCriterion.cs
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


namespace Epic.Query.Object.UnitTests.Fakes
{
    [Serializable]
    public class FakeCriterion<TEntity> : OrderCriterionBase<TEntity, FakeCriterion<TEntity>>
        where TEntity : class
    {
        public FakeCriterion ()
            : base()
        {
        }

        public readonly int Identity;
        private readonly Func<TEntity, TEntity, int> _comparison;
        public bool SafeEqualsCalled = false;

        public FakeCriterion (int identity)
            : base()
        {
            Identity = identity;
        }

        public FakeCriterion(int identity, Func<TEntity, TEntity, int> comparison)
            : this(identity)
        {
            _comparison = comparison;
        }

        #region implemented abstract members of Epic.Query.Object.OrderCriterion
        public override int Compare(TEntity x, TEntity y)
        {
            return _comparison(x, y);
        }

        protected override bool EqualsA(FakeCriterion<TEntity> other)
        {
            SafeEqualsCalled = true;
            return Identity.Equals(other.Identity);
        }
        #endregion

        public FakeCriterion(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Identity = info.GetInt32("I");
        }

        protected override void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            info.AddValue("I", Identity);
        }
    }

    [Serializable]
    public sealed class OtherFakeCriterion<TEntity> : OrderCriterionBase<TEntity, OtherFakeCriterion<TEntity>>
        where TEntity : class
    {
        public OtherFakeCriterion ()
            : base()
        {
        }

        #region implemented abstract members of Epic.Query.Object.OrderCriterion
        public override int Compare(TEntity x, TEntity y)
        {
            throw new System.NotImplementedException();
        }

        protected override bool EqualsA(OtherFakeCriterion<TEntity> other)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        public OtherFakeCriterion(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        protected override void GetObjectData (SerializationInfo info, StreamingContext context)
        {
        }
    }

    public class WrongFakeCriterion<TEntity> : FakeCriterion<TEntity>
        where TEntity : class
    {
    }
}

