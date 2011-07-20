//  
//  RepositoryTranslatorQA.cs
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
using NUnit.Framework;
using System;
using Rhino.Mocks;
using Epic.Linq.Expressions.Visit;
using Epic.Linq.Expressions;
using Epic.Environment;
using System.Linq;
using System.Linq.Expressions;

namespace Epic.Linq.Translators
{
    [TestFixture]
    public class RepositoryTranslatorQA : RhinoMocksFixtureBase
    {
        [SetUp]
        public void ResetApplication()
        {
            TestUtilities.ResetApplication();
        }
        
        [Test]
        public void Visit_withSimpleRepository_works()
        {
            // arrange:
            CompositeVisitorChain chain = new CompositeVisitorChain(new NullCompositeVisitor());
            new RepositoryTranslator<string, int>(chain);
            
            string providerName = "TestProvider";
            EnvironmentBase env = GeneratePartialMock<EnvironmentBase>();
            ApplicationBase app = new Fakes.FakeApplication(env, null);
            Application.Initialize(app);
            IRepository<string,int> repository = new FakeRepository<string,int>(providerName);
            UnvisitableExpressionAdapter expressionAdapter = new UnvisitableExpressionAdapter(repository.Expression);


            // act:
            Expression result = expressionAdapter.Accept(chain, VisitState.New);

            // assert:
            Assert.IsInstanceOf<QueryExpression>(result);
        }
        
        [Test]
        public void Visit_withSimpleWhere_works()
        {
            // arrange:
            CompositeVisitorChain chain = new CompositeVisitorChain(new NullCompositeVisitor());
            new PrintingVisitor(chain);
            
            string providerName = "TestProvider";
            EnvironmentBase env = GeneratePartialMock<EnvironmentBase>();
            IQueryProvider provider = new QueryProvider(providerName);
            env.Expect(e => e.Get<IQueryProvider>(new InstanceName<IQueryProvider>(providerName))).Return(provider).Repeat.Once();
            ApplicationBase app = new Fakes.FakeApplication(env, null);
            Application.Initialize(app);
            IRepository<string,int> repository = new FakeRepository<string,int>(providerName);
            IQueryable<string> queryable = repository.Where(s => s.Length > 2);
            UnvisitableExpressionAdapter expressionAdapter = new UnvisitableExpressionAdapter(queryable.Expression);


            // act:
            Expression result = expressionAdapter.Accept(chain, VisitState.New);

            // assert:
            Assert.AreSame(queryable.Expression, result);
        }
    }
}

