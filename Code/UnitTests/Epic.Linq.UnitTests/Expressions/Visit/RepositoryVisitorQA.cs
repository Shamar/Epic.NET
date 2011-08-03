//  
//  RepositoryVisitorQA.cs
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
using System;
using NUnit.Framework;
using Epic.Environment;
using System.Linq;
using Challenge00.DDDSample.Cargo;
using Challenge00.DDDSample.Location;
using Rhino.Mocks;
using System.Linq.Expressions;
using Epic.Linq.Expressions.Templates;

namespace Epic.Linq.Expressions.Visit
{
    [TestFixture()]
    public class RepositoryVisitorQA : RhinoMocksFixtureBase
    {
        [SetUp]
        public void ResetApplication()
        {
            TestUtilities.ResetApplication();
        }
        
        
        [Test]
        public void Visit_aSimpleWhereQuery_returnASimpleSelection()
        {
            // arrange:
            string providerName = "test";
            EnvironmentBase env = GeneratePartialMock<EnvironmentBase>();
            InstanceName<IQueryProvider> instanceName = new InstanceName<IQueryProvider>(providerName);
            IQueryProvider mockProvider = new QueryProvider(providerName);
            env.Expect(e => e.Get<IQueryProvider>(Arg<InstanceName<IQueryProvider>>.Matches(n => n.Equals(instanceName)))).Return(mockProvider).Repeat.AtLeastOnce();
            ApplicationBase app = new Fakes.FakeApplication(env, null);
            Application.Initialize(app);
            UnLocode location = new UnLocode("USTST");
            IRepository<ICargo, TrackingId> cargos = new FakeRepository<ICargo, TrackingId>(providerName);
            IQueryable<ICargo> selecteds = cargos.Where(c => c.Delivery.LastKnownLocation == location);
            
            IQuery query = null;
            IQueryDataExtractor<Expression<Func<ICargo, bool>>> extractor = TemplateParser<Expression<Func<ICargo, bool>>>.Parse(c => c.Delivery.LastKnownLocation == query.Get<UnLocode>("lastLocation"));
            
            VisitorsComposition chain = new VisitorsComposition("test");
            new UnvisitableExpressionsVisitor(chain);
            new WhereVisitor<ICargo>(chain);
            new RepositoryVisitor<ICargo, TrackingId>(chain, rep => new DomainExpression<TrackingId>("public.cargos"));
            new PredicateVisitor<ICargo>(chain, extractor, (q, r) => new AttributeEqualPredicate(r, new AttributeExpression<string>(r, "lastloc"), new ConstantExpression<string>(q.Get<UnLocode>("lastLocation").ToString())));
            new QueryableConstantVisitor(chain);
            new ClosureVisitor(chain);
            UnvisitableExpressionAdapter adapter = new UnvisitableExpressionAdapter(selecteds.Expression);

            // act:
            VisitableExpression result = adapter.Accept(chain, VisitState.New.Add(mockProvider)) as VisitableExpression;
            SqlExpression sql = result.Accept(new SimpleStringVisitor(), VisitState.New) as SqlExpression;

            // assert:
            Assert.IsNotNull(result);
            Assert.AreEqual((System.Linq.Expressions.ExpressionType)ExpressionType.Selection, result.NodeType);
            Assert.IsNotNull(sql);
            Console.WriteLine(sql.Expression);
        }
    }
    
    class SimpleStringVisitor :
            ICompositeVisitor<SelectionExpression>,
            ICompositeVisitor<AttributeEqualPredicate>,
            ICompositeVisitor<AttributeExpression<string>>,
            ICompositeVisitor<ConstantExpression<string>>,
            ICompositeVisitor<DomainExpression<TrackingId>>
    {
        public SimpleStringVisitor()
        {
        }
        
        

        #region ICompositeVisitor[SelectionExpression] implementation
        public Expression Visit (SelectionExpression target, IVisitState state)
        {
            SqlExpression predicate = target.Predicate.Accept(this, state) as SqlExpression;
            SqlExpression source = target.Source.Accept(this, state) as SqlExpression;
            
            SqlExpression result = new SqlExpression("SELECT * FROM {0} WHERE {1}", source, predicate);
            return result;
        }
        #endregion

        #region ICompositeVisitor[PredicateExpression] implementation
        public Expression Visit (AttributeEqualPredicate target, IVisitState state)
        {
            SqlExpression left = target.Left.Accept(this, state) as SqlExpression;
            SqlExpression right = target.Right.Accept(this, state) as SqlExpression;
            return new SqlExpression("{0} = {1}", left, right);
        }
        #endregion

        #region ICompositeVisitor[AttributeExpression[System.String]] implementation
        public Expression Visit (AttributeExpression<string> target, IVisitState state)
        {
            return new SqlExpression(target.Name);
        }
        #endregion

        #region ICompositeVisitor[ConstantExpression[System.String]] implementation
        public Expression Visit (ConstantExpression<string> target, IVisitState state)
        {
            return new SqlExpression("\"{0}\"", target.Value);
        }
        #endregion

        #region ICompositeVisitor[DomainExpression[TrackingId]] implementation
        public Expression Visit (DomainExpression<TrackingId> target, IVisitState state)
        {
            return new SqlExpression(target.Name);
        }
        #endregion

        #region ICompositeVisitor implementation
        public ICompositeVisitor<TExpression> GetVisitor<TExpression> (TExpression target) where TExpression : Expression
        {
            return this as ICompositeVisitor<TExpression>;
        }
        #endregion
    }
}

