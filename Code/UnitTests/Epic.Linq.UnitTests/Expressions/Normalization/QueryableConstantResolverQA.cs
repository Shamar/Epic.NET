using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Epic.Linq.Expressions.Normalization
{
    [TestFixture]
    public class QueryableConstantResolverQA
    {
        [Test]
        public void Initialize_withoutComposition_throwsArgumentNullException()
        {
            // assert:
            Assert.Throws<ArgumentNullException>(delegate
            {
                new QueryableConstantResolver(null);
            });
        }
    }
}
