//  
//  NamedSpecification.cs
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

namespace Epic.Fakes
{
    internal class NamedSpecification<T> : Specifications.SpecificationBase<NamedSpecification<T>, T>, IEquatable<NamedSpecification<T>> where T : class
    {
        public readonly string Name;
        public NamedSpecification(string name)
        {
            Name = name;
        }
        #region implemented abstract members of SpecificationBase
        protected override bool EqualsA(NamedSpecification<T> otherSpecification)
        {
            return Name.Equals(otherSpecification.Name);
        }
        protected override bool IsSatisfiedByA(T candidate)
        {
            throw new NotImplementedException("This is a mock.");
        }
        #endregion

        public override string ToString()
        {
            return Name;
        }
    }

}
