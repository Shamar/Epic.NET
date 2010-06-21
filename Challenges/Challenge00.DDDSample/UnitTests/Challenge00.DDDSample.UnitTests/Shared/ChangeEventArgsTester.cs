using System;
using NUnit.Framework;
namespace Challenge00.DDDSample.UnitTests
{
	[TestFixture]
	public class ChangeEventArgsTester
	{
		[Test]
		public void Test_Ctor_01()
		{
			// arrange:
			object oldV = new object();
			object newV = new object();
		
			// act:
			ChangeEventArgs<object> args = new ChangeEventArgs<object>(oldV, newV);
		
			// assert:
			Assert.AreSame(oldV, args.OldValue);
			Assert.AreSame(newV, args.NewValue);
		}
	}
}

