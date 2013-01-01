//  
//  ExpressionTreeTester.cs
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
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework.Constraints;

namespace Epic.Query.Linq.Expressions
{
    public static class Verify
    {
        public static Verifier<TExpression> That<TExpression>(TExpression expression) where TExpression : class
        {
            return new Verifier<TExpression>(expression);
        }
    }

    public sealed class Verifier<TExpression>
        where TExpression : class
    {
        private readonly TExpression _exp;
        public Verifier(TExpression expression)
        {
            _exp = expression;
        }
        
        public void IsNull()
        {
            Assert.IsNull(_exp);
        }
        
        public Verifier<TOther> IsA<TOther>() where TOther : class
        {
            Assert.IsNotNull(_exp);
            Assert.IsInstanceOf<TOther>(_exp);
            return new Verifier<TOther>(_exp as TOther);
        }
  
        public Verifier<TExpression> WithA<TProperty>(Func<TExpression, TProperty> property, Action<TProperty> check)
        {
            check(property(_exp));
            return this;
        }

        public Verifier<TExpression> WithA<TProperty>(Func<TExpression, TProperty> property, Func<TProperty, IResolveConstraint> check)
        {
            TProperty val = property(_exp);
            Assert.That(val, check(val));
            return this;
        }
        
        public Verifier<TExpression> WithEach<TProperty>(Expression<Func<TExpression, IEnumerable<TProperty>>> properties, Action<TProperty, int> check)
        {
            Func<TExpression, IEnumerable<TProperty>> accessor = properties.Compile();
            IEnumerable<TProperty> toCheck = accessor(_exp);
            if(null == toCheck)
                Assert.Fail("Got a null IEnumerable<{0}> while accessing to {1} in the expression:\r\n{2}.", typeof(TProperty), properties.ToString(), _exp.ToString());
            int i = 0;
            foreach(TProperty p in toCheck)
            {
                check(p, i);
                i++;
            }
            return this;
        }

        public Verifier<TExpression> WithEach<TProperty>(Expression<Func<TExpression, IEnumerable<TProperty>>> properties, Func<TProperty, bool> when, Action<TProperty, int> check)
        {
            Func<TExpression, IEnumerable<TProperty>> accessor = properties.Compile();
            IEnumerable<TProperty> toCheck = accessor(_exp);
            if(null == toCheck)
                Assert.Fail("Got a null IEnumerable<{0}> while accessing to {1} in the expression:\r\n{2}.", typeof(TProperty), properties.ToString(), _exp.ToString());
            int i = 0;
            foreach (TProperty p in toCheck)
            {
                if(when(p))
                    check(p, i);
                i++;
            }
            return this;
        }
        
        public Verifier<TExpression> WithEach<TProperty>(Expression<Func<TExpression, IEnumerable<TProperty>>> properties, Func<TProperty, int, IResolveConstraint> check)
        {
            Func<TExpression, IEnumerable<TProperty>> accessor = properties.Compile();
            IEnumerable<TProperty> toCheck = accessor(_exp);
            if(null == toCheck)
                Assert.Fail("Got a null IEnumerable<{0}> while accessing to {1} in the expression:\r\n{2}.", typeof(TProperty), properties.ToString(), _exp.ToString());
            int i = 0;
            foreach(TProperty p in toCheck)
            {
                Assert.That(p, check(p, i));
                i++;
            }
            return this;
        }
    }
    
}

