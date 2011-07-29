//  
//  CompositeVisitorChainQA.cs
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
using Epic.Linq.Expressions.Visit;
using System.Linq.Expressions;
using Rhino.Mocks;
using System.Reflection;
using System.Linq;
using Challenge00.DDDSample.Cargo;
using Epic.Environment;
using Challenge00.DDDSample.Voyage;
using Challenge00.DDDSample.Location;

namespace Epic.Linq.Expressions.Visit
{
    public class DumbVisitor : VisitorsComposition.VisitorBase
    {
        public DumbVisitor(VisitorsComposition next)
            : base(next)
        {
        }
    }
    
    public class GenericDumbVisitor<TExpression> : VisitorsComposition.VisitorBase, ICompositeVisitor<TExpression>
        where TExpression : Expression
    {
        public GenericDumbVisitor(VisitorsComposition next)
            : base(next)
        {
        }

        #region ICompositeVisitor[TExpression] implementation
        public virtual System.Linq.Expressions.Expression Visit (TExpression target, IVisitState state)
        {
            return target;
        }
        #endregion
    }
    
    [TestFixture]
    public class CompositeVisitorChainQA : RhinoMocksFixtureBase
    {
        [SetUp]
        public void ResetApplication()
        {
            TestUtilities.ResetApplication();
        }
        
        [Test]
        public void GetVisitor_withKnownExpression_dontReachEnd()
        {
            // arrange:
            MethodCallExpression expression = Expression.Call(Expression.Constant(this), (MethodInfo)MethodBase.GetCurrentMethod());
            VisitorsComposition chain = new VisitorsComposition("test");
            GenericDumbVisitor<MethodCallExpression> dumb1 = new GenericDumbVisitor<MethodCallExpression> (chain);
            GenericDumbVisitor<MemberInitExpression> dumb2 = new GenericDumbVisitor<MemberInitExpression>(chain);

            // act:
            ICompositeVisitor<MethodCallExpression> recievedVisitor = dumb2.GetVisitor<MethodCallExpression>(expression);

            // assert:
            Assert.AreSame(dumb1, recievedVisitor);
        }
        
        [Test]
        public void GetVisitor_withUnknownExpression_reachEnd()
        {
            // arrange:
            MethodCallExpression expression = Expression.Call(Expression.Constant(this), (MethodInfo)MethodBase.GetCurrentMethod());
            VisitorsComposition chain = new VisitorsComposition("test");
            new DumbVisitor(chain);
            DumbVisitor dumb2 = new DumbVisitor(chain);

            // act:
            ICompositeVisitor<MethodCallExpression> recievedVisitor = dumb2.GetVisitor<MethodCallExpression>(expression);

            // assert:
            Assert.IsInstanceOf<UnvisitableExpressionsVisitor>(recievedVisitor);
        }
        
        [Test]
        public void GetVisitor_withUnknownExpressio_toTheChainItself_reachEnd()
        {
            // arrange:
            MethodCallExpression expression = Expression.Call(Expression.Constant(this), (MethodInfo)MethodBase.GetCurrentMethod());
            VisitorsComposition chain = new VisitorsComposition("test");
            new DumbVisitor(chain);
            new DumbVisitor(chain);

            // act:
            ICompositeVisitor<MethodCallExpression> recievedVisitor = chain.GetVisitor<MethodCallExpression>(expression);

            // assert:
            Assert.IsInstanceOf<UnvisitableExpressionsVisitor>(recievedVisitor);
        }
        
        [Test]
        public void GetVisitor_withKnownExpression_toTheChainItsel_dontReachEnd()
        {
            // arrange:
            MethodCallExpression callExpression = Expression.Call(Expression.Constant(this), (MethodInfo)MethodBase.GetCurrentMethod());
            Expression<Func<int, string>> lambdaExpression = x => x.ToString();
            VisitorsComposition chain = new VisitorsComposition("test");
            GenericDumbVisitor<MethodCallExpression> dumb1 = new GenericDumbVisitor<MethodCallExpression> (chain);
            GenericDumbVisitor<Expression<Func<int, string>>> dumb2 = new GenericDumbVisitor<Expression<Func<int, string>>>(chain);

            // act:
            ICompositeVisitor<MethodCallExpression> recievedMethodCallVisitor = chain.GetVisitor<MethodCallExpression>(callExpression);
            ICompositeVisitor<Expression<Func<int, string>>> recievedLambdaVisitor = chain.GetVisitor<Expression<Func<int, string>>>(lambdaExpression);

            // assert:
            Assert.AreSame(dumb1, recievedMethodCallVisitor);
            Assert.AreSame(dumb2, recievedLambdaVisitor);
        }
        
        [Test]
        public void Visit_withPrintingVisitor_works()
        {
            // arrange:
            Expression<Func<int, string, int>> expression = (i,s)=> (i + s.Length).ToString().Length;
            VisitorsComposition chain = new VisitorsComposition("test");
            new LoggingVisitor(chain, WriteToConsole);
            UnvisitableExpressionAdapter adapter = new UnvisitableExpressionAdapter(expression);

            // act:
            Expression e = adapter.Accept(chain, VisitState.New);
            
            // assert:
            Assert.AreSame(expression, e);
        }
        
        
        [Test]
        public void Visit_TernaryWithPrintingVisitor_works()
        {
            // arrange:
            Expression<Func<int, string, int>> expression = (i,s)=> s == null ? i : s.Length;
            VisitorsComposition chain = new VisitorsComposition("test");
            new LoggingVisitor(chain, WriteToConsole);
            UnvisitableExpressionAdapter adapter = new UnvisitableExpressionAdapter(expression);

            // act:
            Expression e = adapter.Accept(chain, VisitState.New);
            
            // assert:
            Assert.AreSame(expression, e);
        }
        
        public static string GetTypeName(Type type)
        {
            string typeName = type.Name;
            if(type.IsGenericType)
            {
                typeName = typeName + "[[" + string.Join("], [", type.GetGenericArguments().Select(t => GetTypeName(t)).ToArray()) + "]]";
            }
            return typeName;
        }
        
        public static void WriteToConsole(int depth, Expression expression, IVisitState state)
        {
            for(int i = 0; i < depth; ++i)
            {
                Console.Write("|   ");
            }
            Console.Write("{0}", expression.NodeType);
            switch(expression.NodeType)
            {
                case ExpressionType.Constant:
                case ExpressionType.Parameter:
                case ExpressionType.Lambda:
                    Console.Write(" : " + expression.ToString());
                break;
                case ExpressionType.Call:
                    MethodCallExpression call = expression as MethodCallExpression;
                    Console.Write(" : " + call.Method.Name);
                break;
            }
            string typeName = GetTypeName(expression.Type);
            Console.WriteLine(" / " + typeName);
        }
        
        [Test]
        public void Visit_CargoVisitLocationWithPrintingVisitor_works()
        {
            // arrange:
            string providerName = "test";
            EnvironmentBase env = GeneratePartialMock<EnvironmentBase>();
            InstanceName<IQueryProvider> instanceName = new InstanceName<IQueryProvider>(providerName);
            IQueryProvider mockProvider = new QueryProvider(providerName);
            env.Expect(e => e.Get<IQueryProvider>(Arg<InstanceName<IQueryProvider>>.Matches(n => n.Equals(instanceName)))).Return(mockProvider).Repeat.Once();
            ApplicationBase app = new Fakes.FakeApplication(env, null);
            Application.Initialize(app);
            IRepository<ICargo, TrackingId> cargos = new FakeRepository<ICargo, TrackingId>(providerName);
            IQueryable<ICargo> movingCargos = from c in cargos 
                                              where c.Delivery.TransportStatus == TransportStatus.OnboardCarrier 
                                              select c;
            IRepository<ILocation, UnLocode> locations = new FakeRepository<ILocation, UnLocode>(providerName);
            IRepository<IVoyage, VoyageNumber> voyages = new FakeRepository<IVoyage, VoyageNumber>(providerName);
            IQueryable<ILocation> locationsTraversedFromVoyagesEndingToday = 
                    from c in movingCargos
                    from l in locations
                    from v in voyages
                    where   c.Delivery.CurrentVoyage == v.Number 
                        &&  v.WillStopOverAt(l)
                        &&  c.Itinerary.FinalArrivalDate == DateTime.Today
                    select l;
            VisitorsComposition chain = new VisitorsComposition("test");
            new LoggingVisitor(chain, WriteToConsole);
            UnvisitableExpressionAdapter adapter = new UnvisitableExpressionAdapter(locationsTraversedFromVoyagesEndingToday.Expression);
                                  

            // act:
            Expression returnedExpression = adapter.Accept(chain, VisitState.New);
            
            // assert:
            Assert.AreSame(locationsTraversedFromVoyagesEndingToday.Expression, returnedExpression);
        }
        
        [Test]
        public void Visit_nextUsLocationsOfMovingVoyagesWithPrintingVisitor_works()
        {
            // arrange:
            string providerName = "test";
            EnvironmentBase env = GeneratePartialMock<EnvironmentBase>();
            InstanceName<IQueryProvider> instanceName = new InstanceName<IQueryProvider>(providerName);
            IQueryProvider mockProvider = new QueryProvider(providerName);
            env.Expect(e => e.Get<IQueryProvider>(Arg<InstanceName<IQueryProvider>>.Matches(n => n.Equals(instanceName)))).Return(mockProvider).Repeat.AtLeastOnce();
            ApplicationBase app = new Fakes.FakeApplication(env, null);
            Application.Initialize(app);
            IRepository<ICargo, TrackingId> cargos = new FakeRepository<ICargo, TrackingId>(providerName);
            IRepository<ILocation, UnLocode> locations = new FakeRepository<ILocation, UnLocode>(providerName);
            IQueryable<ILocation> usLocations = from l in locations where l.UnLocode.StartsWith("US") select l;
            IRepository<IVoyage, VoyageNumber> voyages = new FakeRepository<IVoyage, VoyageNumber>(providerName);
            IQueryable<IVoyage> movingVoyages = voyages.Where(v => v.IsMoving);
            var nextLocationsOfMovingVoyages = 
                    from v in movingVoyages
                    from l in usLocations
                    where v.WillStopOverAt(l)
                    select l;
            VisitorsComposition chain = new VisitorsComposition("test");
            new LoggingVisitor(chain, WriteToConsole);
            UnvisitableExpressionAdapter adapter = new UnvisitableExpressionAdapter(nextLocationsOfMovingVoyages.Expression);
                                  

            // act:
            Expression returnedExpression = adapter.Accept(chain, VisitState.New);
            
            // assert:
            Assert.AreSame(nextLocationsOfMovingVoyages.Expression, returnedExpression);
        }

    }
    
    
}

