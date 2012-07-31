//
//  IdentityMapping.cs
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
using Epic.Query.Relational;
using Epic.Query.Relational.Predicates;
using System.Collections.Generic;

namespace Epic.Query.Object.Relational.Mappings
{
    internal sealed class IdentityMapping<TIdentity, TAttribute> : IIdentityMapping<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        private readonly RelationAttribute _attribute;
        private readonly Func<TAttribute, TIdentity> _fromRelational;
        private readonly Func<TIdentity, TAttribute> _toRelational;
        internal IdentityMapping (RelationAttribute attribute, Func<TAttribute, TIdentity> fromRelational, Func<TIdentity, TAttribute> toRelational)
        {
            if(null == toRelational)
                throw new ArgumentNullException("toRelational");
            if(null == fromRelational)
                throw new ArgumentNullException("fromRelational");

            _attribute = attribute;
            _fromRelational = fromRelational;
            _toRelational = toRelational;
        }

        
        #region IMapping implementation
        TIdentity Epic.Math.IMapping<object[], TIdentity>.ApplyTo (object[] identityData)
        {
            if (null == identityData)
                throw new ArgumentNullException("identityData");
            if (identityData.Length != 1)
            {
                string message = string.Format("Cannot initialize an instance of {0} from an array with {1} objects.", typeof(TIdentity), identityData.Length);
                throw new ArgumentException(message, "identityData");
            }
            TAttribute attribute1 = (TAttribute)identityData[0];
            return _fromRelational(attribute1);
        }
        #endregion
        #region IMapping implementation
        Predicate Epic.Math.IMapping<TIdentity, Predicate>.ApplyTo (TIdentity element)
        {
            return _attribute.Equal(new Constant<TAttribute>(_toRelational(element)));
        }
        #endregion
        #region IIdentityMapping implementation
        IEnumerable<RelationAttribute> IIdentityMapping<TIdentity>.Attributes {
            get {
                yield return _attribute;
            }
        }
        #endregion
    }
}

