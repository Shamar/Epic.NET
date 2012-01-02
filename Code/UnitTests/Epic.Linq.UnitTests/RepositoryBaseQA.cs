//  
//  RepositoryBaseQA.cs
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
using NUnit.Framework;
using System;
using System.Linq.Expressions;
using Epic.Environment;
using Epic;
using Rhino.Mocks;
using System.Linq;
using System.Collections.Generic;
using Epic.Fakes;


namespace Epic.Linq
{
    [TestFixture()]
    public class RepositoryBaseQA : RhinoMocksFixtureBase
    {
        [SetUp]
        public void ResetEnterprise()
        {
            TestUtilities.ResetEnterprise();
        }
    
        [Test]
        public void Initialize_withoutProviderName_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
               new Fakes.FakeRepository<string,int>(null);
            });
            Assert.Throws<ArgumentNullException>(delegate {
               new Fakes.FakeRepository<object,string>(string.Empty);
            });
        }
        
        [Test]
        public void Initialize_withProviderName_works()
        {
            // act:
            IRepository<string,int> repository = new Fakes.FakeRepository<string,int>("test");

            // assert:
            Assert.IsNotNull(repository);
            Assert.AreEqual(typeof(string), repository.ElementType);
            Assert.IsInstanceOf<ConstantExpression>(repository.Expression);
            Assert.AreSame(repository, ((ConstantExpression)repository.Expression).Value);
        }
        
        [Test]
        public void Provider_atFirstTime_isTakenFromTheEnvironment()
        {
            // arrange:
            string providerName = "TestProvider";
            EnvironmentBase env = GeneratePartialMock<EnvironmentBase>();
            InstanceName<IQueryProvider> instanceName = new InstanceName<IQueryProvider>(providerName);
            IQueryProvider mockProvider = GenerateStrictMock<IQueryProvider>();
            env.Expect(e => e.Get<IQueryProvider>(Arg<InstanceName<IQueryProvider>>.Matches(n => n.Equals(instanceName)))).Return(mockProvider).Repeat.Once();
            EnterpriseBase app = new FakeEnterprise(env, null);
            Enterprise.Initialize(app);
            IRepository<string,int> repository = new Fakes.FakeRepository<string,int>(providerName);

            // act:
            IQueryProvider provider = repository.Provider;
            IQueryProvider provider2 = repository.Provider;

            // assert:
            Assert.AreSame(mockProvider, provider);
            Assert.AreSame(mockProvider, provider2);
        }
        
        [Test]
        public void GetEnumerator_onGenericEnumerable_callProviderExecute()
        {
            // arrange:
            string providerName = "TestProvider";
            IRepository<string,int> repository = new Fakes.FakeRepository<string,int>(providerName);
            EnvironmentBase env = GeneratePartialMock<EnvironmentBase>();
            IEnumerator<string> mockEnumerator = GenerateStrictMock<IEnumerator<string>>();
            IEnumerable<string> enumerable = GenerateStrictMock<IEnumerable<string>>();
            enumerable.Expect(e => e.GetEnumerator()).Return(mockEnumerator).Repeat.Once();
            InstanceName<IQueryProvider> instanceName = new InstanceName<IQueryProvider>(providerName);
            IQueryProvider mockProvider = GenerateStrictMock<IQueryProvider>();
            mockProvider.Expect(p => p.Execute<IEnumerable<string>>(Arg<Expression>.Matches(e => e is ConstantExpression && object.ReferenceEquals(((ConstantExpression)e).Value, repository))))
                .Return(enumerable).Repeat.Once();
            env.Expect(e => e.Get<IQueryProvider>(Arg<InstanceName<IQueryProvider>>.Matches(n => n.Equals(instanceName)))).Return(mockProvider).Repeat.Once();
            EnterpriseBase app = new FakeEnterprise(env, null);
            Enterprise.Initialize(app);

            // act:
            IEnumerator<string> enumerator = repository.GetEnumerator();

            // assert:
            Assert.AreSame(mockEnumerator, enumerator);
        }
        
        [Test]
        public void GetEnumerator_onEnumerable_callGenericGetEnumerator()
        {
            // arrange:
            string providerName = "TestProvider";
            IRepository<string,int> repository = new Fakes.FakeRepository<string,int>(providerName);
            EnvironmentBase env = GeneratePartialMock<EnvironmentBase>();
            IEnumerator<string> mockEnumerator = GenerateStrictMock<IEnumerator<string>>();
            IEnumerable<string> enumerable = GenerateStrictMock<IEnumerable<string>>();
            enumerable.Expect(e => e.GetEnumerator()).Return(mockEnumerator).Repeat.Once();
            InstanceName<IQueryProvider> instanceName = new InstanceName<IQueryProvider>(providerName);
            IQueryProvider mockProvider = GenerateStrictMock<IQueryProvider>();
            mockProvider.Expect(p => p.Execute<IEnumerable<string>>(Arg<Expression>.Matches(e => e is ConstantExpression && object.ReferenceEquals(((ConstantExpression)e).Value, repository))))
                .Return(enumerable).Repeat.Once();
            env.Expect(e => e.Get<IQueryProvider>(Arg<InstanceName<IQueryProvider>>.Matches(n => n.Equals(instanceName)))).Return(mockProvider).Repeat.Once();
            EnterpriseBase app = new FakeEnterprise(env, null);
            Enterprise.Initialize(app);
            System.Collections.IEnumerable enumerableRepository = repository;

            // act:
            System.Collections.IEnumerator enumerator = enumerableRepository.GetEnumerator();

            // assert:
            Assert.AreSame(mockEnumerator, enumerator);
        }
    }
}

