//  
//  TemplateParserQA.cs
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
using System.Linq.Expressions;

namespace Epic.Linq.Expressions.Templates
{
    [TestFixture()]
    public class TemplateParserQA
    {
        [Test]
        public void Parse_matchingExpressions_returnQueryData()
        {
            // arrange:
            string lenghtOfString = "lenghtOfString";
            IQuery query = null;
            Expression<Func<string, bool>> template = s => s.Length == query.Get<int>(lenghtOfString);
            Expression<Func<string, bool>> matching = a => a.Length == 10;

            // act:
            IQueryDataExtractor<Expression<Func<string, bool>>> extractor = TemplateParser<Expression<Func<string, bool>>>.Parse(template);
            IQuery matchingQueryData = extractor.Parse(matching);
   
            // assert:
            Assert.IsTrue(extractor.CanParse(matching));
            Assert.IsNotNull(matchingQueryData);
            Assert.AreEqual(10, matchingQueryData.Get<int>(lenghtOfString));
        }

        [Test]
        public void Parse_unmatchingParametricExpressions_returnNull()
        {
            // arrange:
            string lenghtOfString = "lenghtOfString";
            IQuery query = null;
            Expression<Func<string, bool>> template = s => s.Length == query.Get<int>(lenghtOfString);
            Expression<Func<string, bool>> notMatching = a => a.Length == a.Length;

            // act:
            IQueryDataExtractor<Expression<Func<string, bool>>> extractor = TemplateParser<Expression<Func<string, bool>>>.Parse(template);
            IQuery notMatchingQueryData = extractor.Parse(notMatching);

            // assert:
            Assert.IsFalse(extractor.CanParse(notMatching));
            Assert.IsNull(notMatchingQueryData);
        }
        
        [Test]
        public void Parse_matchingCommutatedExpressions_returnQueryData()
        {
            // arrange:
            string lenghtOfString = "lenghtOfString";
            IQuery query = null;
            Expression<Func<string, bool>> template = s => s.Length == query.Get<int>(lenghtOfString);
            Expression<Func<string, bool>> matching = a => 10 == a.Length;

            // act:
            IQueryDataExtractor<Expression<Func<string, bool>>> extractor = TemplateParser<Expression<Func<string, bool>>>.Parse(template);
            IQuery matchingQueryData = extractor.Parse(matching);
   
            // assert:
            Assert.IsTrue(extractor.CanParse(matching));
            Assert.IsNotNull(matchingQueryData);
            Assert.AreEqual(10, matchingQueryData.Get<int>(lenghtOfString));
        }
        
        [Test]
        public void Parse_matchingWithoutQueryVariables_returnQueryData()
        {
            // arrange:
            Expression<Func<string, bool>> template = s => s.Length == s.Length;
            Expression<Func<string, bool>> matching = a => a.Length == a.Length;

            TestingUtilities.PrintExpression(template);
            
            // act:
            IQueryDataExtractor<Expression<Func<string, bool>>> extractor = TemplateParser<Expression<Func<string, bool>>>.Parse(template);
            IQuery matchingQueryData = extractor.Parse(matching);
   
            // assert:
            Assert.IsTrue(extractor.CanParse(matching));
            Assert.IsNotNull(matchingQueryData);
        }
        
        [Test]
        public void Parse_notMatchingWithoutQueryVariables_returnQueryData()
        {
            // arrange:
            Expression<Func<string, bool>> template = s => s.Length == s.Length;
            Expression<Func<string, bool>> notMatching = a => a.Length == a.GetHashCode();
   
            // act:
            IQueryDataExtractor<Expression<Func<string, bool>>> extractor = TemplateParser<Expression<Func<string, bool>>>.Parse(template);
            IQuery matchingQueryData = extractor.Parse(notMatching);
   
            // assert:
            Assert.IsFalse(extractor.CanParse(notMatching));
            Assert.IsNull(matchingQueryData);
        }
        
        [Test]
        public void Parse_notMatchingExpression_returnNull()
        {
            // arrange:
            string lenghtOfString = "lenghtOfString";
            IQuery query = null;
            Expression<Func<string, bool>> template = s => s.Length == query.Get<int>(lenghtOfString);
            Expression<Func<string, bool>> notMatching = b => b.Length < 5;

            // act:
            IQueryDataExtractor<Expression<Func<string, bool>>> extractor = TemplateParser<Expression<Func<string, bool>>>.Parse(template);
            IQuery notMatchingQueryData = extractor.Parse(notMatching);

            // assert:
            Assert.IsFalse(extractor.CanParse(notMatching));
            Assert.IsNull(notMatchingQueryData);
        }

        [Test]
        public void Parse_matchingCommutedAndAlsoExpressions_returnQueryData()
        {
            // arrange:
            int value = 10;
            IQuery query = null;
            Expression<Func<string, bool>> template = s => s.Length == query.Get<int>("lenghtOfString") && s.Contains("test");
            Expression<Func<string, bool>> matching = a => a.Contains("test") && a.Length == value;

            // act:
            IQueryDataExtractor<Expression<Func<string, bool>>> extractor = TemplateParser<Expression<Func<string, bool>>>.Parse(template);
            IQuery matchingQueryData = extractor.Parse(matching);

            // assert:
            Assert.IsTrue(extractor.CanParse(matching));
            Assert.IsNotNull(matchingQueryData);
            Assert.AreEqual(value, matchingQueryData.Get<int>("lenghtOfString"));
        }
        
        [Test]
        public void Parse_matchingExpressionWith2Variable_returnQueryData()
        {
            // arrange:
            int value = 10;
            IQuery query = null;
            Expression<Func<string, bool>> template = s => s.Length == query.Get<int>("lenghtOfString") && s.Contains(query.Get<string>("containedString"));
            Expression<Func<string, bool>> matching = a => a.Contains("test") && value == a.Length;

            // act:
            IQueryDataExtractor<Expression<Func<string, bool>>> extractor = TemplateParser<Expression<Func<string, bool>>>.Parse(template);
            IQuery matchingQueryData = extractor.Parse(matching);

            // assert:
            Assert.IsTrue(extractor.CanParse(matching));
            Assert.IsNotNull(matchingQueryData);
            Assert.AreEqual(value, matchingQueryData.Get<int>("lenghtOfString"));
            Assert.AreEqual("test", matchingQueryData.Get<string>("containedString"));
        }
    }
}

