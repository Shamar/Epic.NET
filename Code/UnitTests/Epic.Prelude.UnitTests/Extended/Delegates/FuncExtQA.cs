//
//  FuncExtQA.cs
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
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;

namespace Epic.Extended.Delegates
{
    [TestFixture]
    public class FuncExtQA : RhinoMocksFixtureBase
    {
        #region ApplyTo Func<T, TResult>

        [Test]
        public void ApplyTo1_withoutAnyArgument_throwsArgumentNullException ()
        {
            // arrange:
            var dummyArray = new object[0];
            Func<int, string> func = MockRepository.GeneratePartialMock<Func<int, string>>();

            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                dummyArray.ApplyTo<int,int>(null);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                FuncExt.ApplyTo(null, func);
            });
        }

        [Test]
        public void ApplyToWithRemainder1_withoutAnyArgument_throwsArgumentNullException ()
        {
            // arrange:
            var dummyArray = new object[0];
            Func<int, string> func = MockRepository.GeneratePartialMock<Func<int, string>>();
            object[] remainders = null;
            
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                dummyArray.ApplyTo<int,int>(null, out remainders);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                FuncExt.ApplyTo(null, func, out remainders);
            });
        }

        [Test]
        public void ApplyTo1_missingArguments_throwsMissingValuesException ()
        {
            // arrange:
            var dummyArray = new object[0];
            Func<int, string> func = MockRepository.GeneratePartialMock<Func<int, string>>();
            
            // assert:
            Assert.Throws<MissingValuesException>(delegate {
                dummyArray.ApplyTo(func);
            });
        }
        
        [Test]
        public void ApplyToWithRemainder1_missingArguments_throwsMissingValuesException ()
        {
            // arrange:
            var dummyArray = new object[0];
            Func<int, string> func = MockRepository.GeneratePartialMock<Func<int, string>>();
            object[] remainders = null;
            
            // assert:
            Assert.Throws<MissingValuesException>(delegate {
                dummyArray.ApplyTo(func, out remainders);
            });
        }

        [Test]
        public void ApplyTo1_withCorrectsArguments_works ()
        {
            // arrange:
            var arguments = new object[] {1, 2, 3, 4, 5, 6, 7};
            var expectedResult = "expectedResult";
            Func<int, string> func = MockRepository.GeneratePartialMock<Func<int, string>>();
            func.Expect(f => f(1)).Return(expectedResult).Repeat.Once();

            // act:
            var result = arguments.ApplyTo(func);
            
            // assert:
            Assert.AreSame(expectedResult, result);
            func.VerifyAllExpectations();
        }
        
        [Test]
        public void ApplyToWithRemainder1_withCorrectsArguments_works ()
        {
            // arrange:
            var arguments = new object[] {1, 2, 3, 4, 5, 6, 7};
            var expectedResult = "expectedResult";
            Func<int, string> func = MockRepository.GeneratePartialMock<Func<int, string>>();
            func.Expect(f => f(1)).Return(expectedResult).Repeat.Once();
            object[] remainders = null;

            // act:
            var result = arguments.ApplyTo(func, out remainders);

            // assert:
            Assert.AreSame(expectedResult, result);
            Assert.AreEqual(arguments.Length - 1, remainders.Length);
            CollectionAssert.AreEqual(arguments.Skip(1), remainders);
            func.VerifyAllExpectations();
        }

        [Test]
        public void ApplyToWithRemainder1_withExactArguments_works ()
        {
            // arrange:
            var arguments = new object[] {1};
            var expectedResult = "expectedResult";
            Func<int, string> func = MockRepository.GeneratePartialMock<Func<int, string>>();
            func.Expect(f => f(1)).Return(expectedResult).Repeat.Once();
            object[] remainders = null;
            
            // act:
            var result = arguments.ApplyTo(func, out remainders);
            
            // assert:
            Assert.AreSame(expectedResult, result);
            Assert.AreEqual(0, remainders.Length);
            func.VerifyAllExpectations();
        }

        #endregion ApplyTo Func<T, TResult>


        #region ApplyTo Func<T1, T2, TResult>

        [Test]
        public void ApplyTo2_withoutAnyArgument_throwsArgumentNullException ()
        {
            // arrange:
            var dummyArray = new object[0];
            Func<int, int, string> func = MockRepository.GeneratePartialMock<Func<int, int, string>>();
            
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                dummyArray.ApplyTo<int,int,string>(null);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                FuncExt.ApplyTo(null, func);
            });
        }
        
        [Test]
        public void ApplyToWithRemainder2_withoutAnyArgument_throwsArgumentNullException ()
        {
            // arrange:
            var dummyArray = new object[0];
            Func<int, int, string> func = MockRepository.GeneratePartialMock<Func<int, int, string>>();
            object[] remainders = null;
            
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                dummyArray.ApplyTo<int, int,string>(null, out remainders);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                FuncExt.ApplyTo(null, func, out remainders);
            });
        }
        
        [Test]
        public void ApplyTo2_missingArguments_throwsMissingValuesException ()
        {
            // arrange:
            var dummyArray = new object[0];
            Func<int, int, string> func = MockRepository.GeneratePartialMock<Func<int, int, string>>();
            
            // assert:
            Assert.Throws<MissingValuesException>(delegate {
                dummyArray.ApplyTo(func);
            });
        }
        
        [Test]
        public void ApplyToWithRemainder2_missingArguments_throwsMissingValuesException ()
        {
            // arrange:
            var dummyArray = new object[0];
            Func<int, int, string> func = MockRepository.GeneratePartialMock<Func<int, int, string>>();
            object[] remainders = null;
            
            // assert:
            Assert.Throws<MissingValuesException>(delegate {
                dummyArray.ApplyTo(func, out remainders);
            });
        }
        
        [Test]
        public void ApplyTo2_withCorrectsArguments_works ()
        {
            // arrange:
            var arguments = new object[] {1, 2, 3, 4, 5, 6, 7};
            var expectedResult = "expectedResult";
            Func<int, int, string> func = MockRepository.GeneratePartialMock<Func<int, int, string>>();
            func.Expect(f => f(1,2)).Return(expectedResult).Repeat.Once();
            
            // act:
            var result = arguments.ApplyTo(func);
            
            // assert:
            Assert.AreSame(expectedResult, result);
            func.VerifyAllExpectations();
        }
        
        [Test]
        public void ApplyToWithRemainder2_withCorrectsArguments_works ()
        {
            // arrange:
            var arguments = new object[] {1, 2, 3, 4, 5, 6, 7};
            var expectedResult = "expectedResult";
            Func<int, int, string> func = MockRepository.GeneratePartialMock<Func<int, int, string>>();
            func.Expect(f => f(1,2)).Return(expectedResult).Repeat.Once();
            object[] remainders = null;
            
            // act:
            var result = arguments.ApplyTo(func, out remainders);
            
            // assert:
            Assert.AreSame(expectedResult, result);
            Assert.AreEqual(arguments.Length - 2, remainders.Length);
            CollectionAssert.AreEqual(arguments.Skip(2), remainders);
            func.VerifyAllExpectations();
        }

        [Test]
        public void ApplyToWithRemainder2_withExactArguments_works ()
        {
            // arrange:
            var arguments = new object[] {1, 2};
            var expectedResult = "expectedResult";
            Func<int, int, string> func = MockRepository.GeneratePartialMock<Func<int, int, string>>();
            func.Expect(f => f(1,2)).Return(expectedResult).Repeat.Once();
            object[] remainders = null;
            
            // act:
            var result = arguments.ApplyTo(func, out remainders);
            
            // assert:
            Assert.AreSame(expectedResult, result);
            Assert.AreEqual(arguments.Length - 2, remainders.Length);
            CollectionAssert.AreEqual(arguments.Skip(2), remainders);
            func.VerifyAllExpectations();
        }

        #endregion ApplyTo Func<T1, T2, TResult>

        #region ApplyTo Func<T1, T2, T3, TResult>
        
        [Test]
        public void ApplyTo3_withoutAnyArgument_throwsArgumentNullException ()
        {
            // arrange:
            var dummyArray = new object[0];
            Func<int, int, int, string> func = MockRepository.GeneratePartialMock<Func<int, int, int, string>>();
            
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                dummyArray.ApplyTo<int,int, int,string>(null);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                FuncExt.ApplyTo(null, func);
            });
        }
        
        [Test]
        public void ApplyToWithRemainder3_withoutAnyArgument_throwsArgumentNullException ()
        {
            // arrange:
            var dummyArray = new object[0];
            Func<int, int, int, string> func = MockRepository.GeneratePartialMock<Func<int, int, int, string>>();
            object[] remainders = null;
            
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                dummyArray.ApplyTo<int, int, int, string>(null, out remainders);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                FuncExt.ApplyTo(null, func, out remainders);
            });
        }
        
        [Test]
        public void ApplyTo3_missingArguments_throwsMissingValuesException ()
        {
            // arrange:
            var dummyArray = new object[0];
            Func<int, int, int, string> func = MockRepository.GeneratePartialMock<Func<int, int, int, string>>();
            
            // assert:
            Assert.Throws<MissingValuesException>(delegate {
                dummyArray.ApplyTo(func);
            });
        }
        
        [Test]
        public void ApplyToWithRemainder3_missingArguments_throwsMissingValuesException ()
        {
            // arrange:
            var dummyArray = new object[0];
            Func<int, int, int, string> func = MockRepository.GeneratePartialMock<Func<int, int, int, string>>();
            object[] remainders = null;
            
            // assert:
            Assert.Throws<MissingValuesException>(delegate {
                dummyArray.ApplyTo(func, out remainders);
            });
        }
        
        [Test]
        public void ApplyTo3_withCorrectsArguments_works ()
        {
            // arrange:
            var arguments = new object[] {1, 2, 3, 4, 5, 6, 7};
            var expectedResult = "expectedResult";
            Func<int, int, int, string> func = MockRepository.GeneratePartialMock<Func<int, int, int, string>>();
            func.Expect(f => f(1,2,3)).Return(expectedResult).Repeat.Once();
            
            // act:
            var result = arguments.ApplyTo(func);
            
            // assert:
            Assert.AreSame(expectedResult, result);
            func.VerifyAllExpectations();
        }
        
        [Test]
        public void ApplyToWithRemainder3_withCorrectsArguments_works ()
        {
            // arrange:
            var arguments = new object[] {1, 2, 3, 4, 5, 6, 7};
            var expectedResult = "expectedResult";
            Func<int, int, int, string> func = MockRepository.GeneratePartialMock<Func<int, int, int, string>>();
            func.Expect(f => f(1,2,3)).Return(expectedResult).Repeat.Once();
            object[] remainders = null;
            
            // act:
            var result = arguments.ApplyTo(func, out remainders);
            
            // assert:
            Assert.AreSame(expectedResult, result);
            Assert.AreEqual(arguments.Length - 3, remainders.Length);
            CollectionAssert.AreEqual(arguments.Skip(3), remainders);
            func.VerifyAllExpectations();
        }
        
        [Test]
        public void ApplyToWithRemainder3_withExactArguments_works ()
        {
            // arrange:
            var arguments = new object[] {1, 2, 3};
            var expectedResult = "expectedResult";
            Func<int, int, int, string> func = MockRepository.GeneratePartialMock<Func<int, int, int, string>>();
            func.Expect(f => f(1,2,3)).Return(expectedResult).Repeat.Once();
            object[] remainders = null;
            
            // act:
            var result = arguments.ApplyTo(func, out remainders);
            
            // assert:
            Assert.AreSame(expectedResult, result);
            Assert.AreEqual(arguments.Length - 3, remainders.Length);
            CollectionAssert.AreEqual(arguments.Skip(3), remainders);
            func.VerifyAllExpectations();
        }
        
        #endregion ApplyTo Func<T1, T2, T3, TResult>

        #region ApplyTo Func<T1, T2, T3, T4, TResult>
        
        [Test]
        public void ApplyTo4_withoutAnyArgument_throwsArgumentNullException ()
        {
            // arrange:
            var dummyArray = new object[0];
            Func<int, int, int, int, string> func = MockRepository.GeneratePartialMock<Func<int, int, int, int, string>>();
            
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                dummyArray.ApplyTo<int, int,int, int,string>(null);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                FuncExt.ApplyTo(null, func);
            });
        }
        
        [Test]
        public void ApplyToWithRemainder4_withoutAnyArgument_throwsArgumentNullException ()
        {
            // arrange:
            var dummyArray = new object[0];
            Func<int, int, int, int, string> func = MockRepository.GeneratePartialMock<Func<int, int, int, int, string>>();
            object[] remainders = null;
            
            // assert:
            Assert.Throws<ArgumentNullException>(delegate {
                dummyArray.ApplyTo<int, int, int, int, string>(null, out remainders);
            });
            Assert.Throws<ArgumentNullException>(delegate {
                FuncExt.ApplyTo(null, func, out remainders);
            });
        }
        
        [Test]
        public void ApplyTo4_missingArguments_throwsMissingValuesException ()
        {
            // arrange:
            var dummyArray = new object[0];
            Func<int, int, int, int, string> func = MockRepository.GeneratePartialMock<Func<int, int, int, int, string>>();
            
            // assert:
            Assert.Throws<MissingValuesException>(delegate {
                dummyArray.ApplyTo(func);
            });
        }
        
        [Test]
        public void ApplyToWithRemainder4_missingArguments_throwsMissingValuesException ()
        {
            // arrange:
            var dummyArray = new object[0];
            Func<int, int, int, int, string> func = MockRepository.GeneratePartialMock<Func<int, int, int, int, string>>();
            object[] remainders = null;
            
            // assert:
            Assert.Throws<MissingValuesException>(delegate {
                dummyArray.ApplyTo(func, out remainders);
            });
        }
        
        [Test]
        public void ApplyTo4_withCorrectsArguments_works ()
        {
            // arrange:
            var arguments = new object[] {1, 2, 3, 4, 5, 6, 7};
            var expectedResult = "expectedResult";
            Func<int, int, int, int, string> func = MockRepository.GeneratePartialMock<Func<int, int, int, int, string>>();
            func.Expect(f => f(1,2,3,4)).Return(expectedResult).Repeat.Once();
            
            // act:
            var result = arguments.ApplyTo(func);
            
            // assert:
            Assert.AreSame(expectedResult, result);
            func.VerifyAllExpectations();
        }
        
        [Test]
        public void ApplyToWithRemainder4_withCorrectsArguments_works ()
        {
            // arrange:
            var arguments = new object[] {1, 2, 3, 4, 5, 6, 7};
            var expectedResult = "expectedResult";
            Func<int, int, int, int, string> func = MockRepository.GeneratePartialMock<Func<int, int, int, int, string>>();
            func.Expect(f => f(1,2,3,4)).Return(expectedResult).Repeat.Once();
            object[] remainders = null;
            
            // act:
            var result = arguments.ApplyTo(func, out remainders);
            
            // assert:
            Assert.AreSame(expectedResult, result);
            Assert.AreEqual(arguments.Length - 4, remainders.Length);
            CollectionAssert.AreEqual(arguments.Skip(4), remainders);
            func.VerifyAllExpectations();
        }
        
        [Test]
        public void ApplyToWithRemainder4_withExactArguments_works ()
        {
            // arrange:
            var arguments = new object[] {1, 2, 3, 4};
            var expectedResult = "expectedResult";
            Func<int, int, int, int, string> func = MockRepository.GeneratePartialMock<Func<int, int, int, int, string>>();
            func.Expect(f => f(1,2,3,4)).Return(expectedResult).Repeat.Once();
            object[] remainders = null;
            
            // act:
            var result = arguments.ApplyTo(func, out remainders);
            
            // assert:
            Assert.AreSame(expectedResult, result);
            Assert.AreEqual(arguments.Length - 4, remainders.Length);
            CollectionAssert.AreEqual(arguments.Skip(4), remainders);
            func.VerifyAllExpectations();
        }
        
        #endregion ApplyTo Func<T1, T2, T3, T4, TResult>
    }
}

