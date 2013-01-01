//  
//  FakeSpecification.cs
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
using Epic.Specifications;


namespace Epic.Query.Object.UnitTests.Fakes
{
    [Serializable]
    public class FakeSpecification<TEntity> : ISpecification<TEntity>
        where TEntity : class
    {
        public FakeSpecification ()
        {
        }

        #region IEquatable implementation
        public bool Equals (ISpecification<TEntity> other)
        {
            throw new System.NotImplementedException ();
        }
        #endregion

        #region IVisitable implementation
        public TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            throw new System.NotImplementedException ();
        }
        #endregion

        #region ISpecification implementation
        public bool IsSatisfiedBy (TEntity candidate)
        {
            throw new System.NotImplementedException ();
        }

        public Type CandidateType {
            get {
                throw new System.NotImplementedException ();
            }
        }

        public Type SpecificationType {
            get {
                throw new System.NotImplementedException ();
            }
        }

        public ISpecification<TEntity> And (ISpecification<TEntity> other)
        {
            throw new System.NotImplementedException ();
        }

        public ISpecification<TEntity> Or (ISpecification<TEntity> other)
        {
            throw new System.NotImplementedException ();
        }

        public ISpecification<TEntity> Negate ()
        {
            throw new System.NotImplementedException ();
        }

        public ISpecification<TOther> OfType<TOther> () where TOther : class
        {
            throw new System.NotImplementedException ();
        }
        #endregion

        #region IVisitable implementation
        TResult IVisitable.Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
        {
            throw new System.NotImplementedException ();
        }
        #endregion
    }
}

