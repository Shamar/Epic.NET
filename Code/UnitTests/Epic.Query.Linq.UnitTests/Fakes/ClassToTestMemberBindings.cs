//  
//  ClassToTestMemberInits.cs
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
using System.Collections.Generic;

namespace Epic.Query.Linq.Fakes
{
    /// <summary>
    /// Class to test the various type of member bindings.
    /// </summary>
    /// <seealso cref="http://stackoverflow.com/questions/2917448/what-are-some-examples-of-memberbinding-linq-expressions/2917498#2917498"/>
    public class ClassToTestMemberBindings
    {
        public ClassToTestMemberBindings()
        {
        }
        public ClassToTestMemberBindings(string name)
            : this()
        {
            Name = name;
        }
        
        public string Name { get; set; }
        
        private ClassToTestMemberBindings _father = new ClassToTestMemberBindings();
        public ClassToTestMemberBindings Father { get { return _father; } set { _father = value; } }
        
        private IList<ClassToTestMemberBindings> _children = new List<ClassToTestMemberBindings>();
        public IList<ClassToTestMemberBindings> Children { get { return _children; } set { _children = value; } }
    }
}

