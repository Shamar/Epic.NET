//
//  FunctionMapping.cs
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

namespace Epic.Math
{
    [Serializable]
    internal sealed class FunctionMapping<TDomain, TCodomain> : IMapping<TDomain, TCodomain>
    {
        private readonly Func<TDomain, TCodomain> _function;
        internal FunctionMapping (Func<TDomain, TCodomain> function)
        {
            if(null == function)
                throw new ArgumentNullException("function");
            _function = function;
        }

        #region IMapping implementation
        public TCodomain ApplyTo (TDomain element)
        {
            return _function(element);
        }
        #endregion
    }
}

