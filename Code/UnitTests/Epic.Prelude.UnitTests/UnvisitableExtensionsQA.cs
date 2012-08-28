//
//  UnvisitableExtensionsQA.cs
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
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq.Expressions;

namespace Epic
{
    [TestFixture]
    public class UnvisitableExtensionsQA : RhinoMocksFixtureBase
    {
        [Test]
        public void Accept_anArgumentOutOfRangeException_forwardToTheVisitorTheRightType ()
        {
            // arrange:
            VisitContext context = VisitContext.New;
            ArgumentOutOfRangeException exception = new ArgumentOutOfRangeException("param");
            IVisitor<string> visitor = GenerateStrictMock<IVisitor<string>>();
            IVisitor<string, ArgumentOutOfRangeException> specializedVisitor = GenerateStrictMock<IVisitor<string, ArgumentOutOfRangeException>>();
            visitor.Expect(v => v.AsVisitor<ArgumentOutOfRangeException>(exception)).Return(specializedVisitor).Repeat.Once();
            specializedVisitor.Expect(v => v.Visit(exception, context)).Return ("test").Repeat.Once();

            // act:
            string result = exception.Accept(visitor, context);

            // assert:
            Assert.AreEqual("test", result);
        }

        [Test]
        public void Accept_aConstantExpression_forwardToTheVisitorTheRightType()
        {
            // arrange:
            VisitContext context = VisitContext.New;
            ConstantExpression toVisit = Expression.Constant(1);
            IVisitor<string> visitor = GenerateStrictMock<IVisitor<string>>();
            IVisitor<string, ConstantExpression> specializedVisitor = GenerateStrictMock<IVisitor<string, ConstantExpression>>();
            visitor.Expect(v => v.AsVisitor<ConstantExpression>(toVisit)).Return(specializedVisitor).Repeat.Once();
            specializedVisitor.Expect(v => v.Visit(toVisit, context)).Return("test").Repeat.Once();

            // act:
            string result = toVisit.Accept(visitor, context);

            // assert:
            Assert.AreEqual("test", result);
        }


        [Test]
        public void Accept_aAssemblyLoadEventArgs_forwardToTheVisitorTheRightType()
        {
            // arrange:
            VisitContext context = VisitContext.New;
            AssemblyLoadEventArgs toVisit = new AssemblyLoadEventArgs(null);
            IVisitor<string> visitor = GenerateStrictMock<IVisitor<string>>();
            IVisitor<string, AssemblyLoadEventArgs> specializedVisitor = GenerateStrictMock<IVisitor<string, AssemblyLoadEventArgs>>();
            visitor.Expect(v => v.AsVisitor<AssemblyLoadEventArgs>(toVisit)).Return(specializedVisitor).Repeat.Once();
            specializedVisitor.Expect(v => v.Visit(toVisit, context)).Return("test").Repeat.Once();

            // act:
            string result = toVisit.Accept(visitor, context);

            // assert:
            Assert.AreEqual("test", result);
        }

        [Test]
        public void Accept_anArgumentNullExceptionAfterAnArgumentException_forwardToTheVisitorTheRightType ()
        {
            // arrange:
            VisitContext context = VisitContext.New;
            ArgumentException ae = new ArgumentException();
            ArgumentNullException ane = new ArgumentNullException();
            IVisitor<string> visitor = GenerateStrictMock<IVisitor<string>>();
            IVisitor<string, ArgumentException> aeVisitor = GenerateStrictMock<IVisitor<string, ArgumentException>>();
            IVisitor<string, ArgumentNullException> aneVisitor = GenerateStrictMock<IVisitor<string, ArgumentNullException>>();
            visitor.Expect(v => v.AsVisitor<ArgumentException>(ae)).Return(aeVisitor).Repeat.Once();
            visitor.Expect(v => v.AsVisitor<ArgumentNullException>(ane)).Return(aneVisitor).Repeat.Once();
            aeVisitor.Expect(v => v.Visit(ae, context)).Return ("Argument").Repeat.Once();
            aneVisitor.Expect(v => v.Visit(ane, context)).Return ("Null").Repeat.Once();

            // act:
            string resultAE = ae.Accept(visitor, context);
            string resultANE = ane.Accept(visitor, context);

            // assert:
            Assert.AreEqual("Argument", resultAE);
            Assert.AreEqual("Null", resultANE);
        }

        [Test]
        public void Accept_withoutAnArgument_throwsArgumentNullException ()
        {
            // arrange:
            VisitContext context = VisitContext.New;
            ArgumentException ae = new ArgumentException();
            ArgumentException nullAE = null;
            Expression exp = Expression.Constant(1);
            Expression nullExp = null;
            EventArgs args = new AssemblyLoadEventArgs(null);
            EventArgs nullArgs = null;
            IVisitor<string> visitor = GenerateStrictMock<IVisitor<string>>();

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                ae.Accept<string>(null, context);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                ae.Accept(visitor, null);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                nullAE.Accept(visitor, context);
            });
            Assert.Throws<ArgumentNullException>(delegate
            {
                exp.Accept<string>(null, context);
            });
            Assert.Throws<ArgumentNullException>(delegate
            {
                exp.Accept(visitor, null);
            });
            Assert.Throws<ArgumentNullException>(delegate
            {
                nullExp.Accept(visitor, context);
            });
            Assert.Throws<ArgumentNullException>(delegate
            {
                args.Accept<string>(null, context);
            });
            Assert.Throws<ArgumentNullException>(delegate
            {
                args.Accept(visitor, null);
            });
            Assert.Throws<ArgumentNullException>(delegate
            {
                nullArgs.Accept(visitor, context);
            });
        }
    }
}

