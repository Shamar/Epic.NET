//
//  FuncExt.cs
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

namespace Epic.Extended.Delegates
{
    /// <summary>
    /// Extension methods for delegate invocation.
    /// </summary>
    public static class FuncExt
    {
        private static readonly object[] _empty = new object[0];

        /// <summary>
        /// Applies an array of objects to a function.
        /// </summary>
        /// <returns>
        /// The result returned by <paramref name="func"/>.
        /// </returns>
        /// <param name='values'>
        /// Values to apply.
        /// </param>
        /// <param name='func'>
        /// Function to apply.
        /// </param>
        /// <typeparam name='T'>
        /// The 1st type parameter.
        /// </typeparam>
        /// <typeparam name='TResult'>
        /// The type of the result.
        /// </typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> or <paramref name="func"/>
        /// is <see langword="null"/>.</exception>
        /// <exception cref="InvalidCastException">The first element of <paramref name="values"/>
        /// can not be casted to <typeparamref name="T"/>.</exception>
        /// <exception cref="MissingValuesException"><paramref name="values"/> contains too few elements
        /// to be used as arguments for <paramref name="func"/>.</exception>
        public static TResult ApplyTo<T, TResult> (this object[] values, Func<T, TResult> func)
        {
            if (null == values)
                throw new ArgumentNullException ("values");
            if (null == func)
                throw new ArgumentNullException ("func");
            if (values.Length < 1)
                throw new MissingValuesException ("values", string.Format ("Cannot apply {0} objects to a {1}.", values.Length, func.GetType ()));
            TResult result = func ((T)values [0]);
            return result;
        }

        /// <summary>
        /// Applies an array of objects to a function.
        /// </summary>
        /// <returns>
        /// The result returned by <paramref name="func"/>.
        /// </returns>
        /// <param name='values'>
        /// Values to apply.
        /// </param>
        /// <param name='func'>
        /// Function to apply.
        /// </param>
        /// <param name='remainders'>
        /// Values ignored by <paramref name="func"/>.
        /// </param>
        /// <typeparam name='T'>
        /// The 1st type parameter.
        /// </typeparam>
        /// <typeparam name='TResult'>
        /// The type of the result.
        /// </typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> or <paramref name="func"/>
        /// is <see langword="null"/>.</exception>
        /// <exception cref="InvalidCastException">The first element of <paramref name="values"/>
        /// can not be casted to <typeparamref name="T"/>.</exception>
        /// <exception cref="MissingValuesException"><paramref name="values"/> contains too few elements
        /// to be used as arguments for <paramref name="func"/>.</exception>
        public static TResult ApplyTo<T, TResult> (this object[] values, Func<T, TResult> func, out object[] remainders)
        {
            if (null == values)
                throw new ArgumentNullException ("values");
            if (null == func)
                throw new ArgumentNullException ("func");
            if (values.Length < 1)
                throw new MissingValuesException ("values", string.Format ("Cannot apply {0} objects to a {1}.", values.Length, func.GetType ()));
            TResult result = func ((T)values [0]);
            if (values.Length == 1) {
                remainders = _empty;
            } else {
                remainders = new object[values.Length - 1];
                Array.Copy (values, 1, remainders, 0, remainders.Length);
            }
            return result;
        }

        /// <summary>
        /// Applies an array of objects to a function.
        /// </summary>
        /// <returns>
        /// The result returned by <paramref name="func"/>.
        /// </returns>
        /// <param name='values'>
        /// Values to apply.
        /// </param>
        /// <param name='func'>
        /// Function to apply.
        /// </param>
        /// <typeparam name='T1'>
        /// The 1st type parameter.
        /// </typeparam>
        /// <typeparam name='T2'>
        /// The 2nd type parameter.
        /// </typeparam>
        /// <typeparam name='TResult'>
        /// The type of the result.
        /// </typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> or <paramref name="func"/>
        /// is <see langword="null"/>.</exception>
        /// <exception cref="InvalidCastException">The first element of <paramref name="values"/>
        /// can not be casted to <typeparamref name="T1"/> or the second element can not be
        /// casted to <typeparamref name="T2"/>.</exception>
        /// <exception cref="MissingValuesException"><paramref name="values"/> contains too few elements
        /// to be used as arguments for <paramref name="func"/>.</exception>
        public static TResult ApplyTo<T1, T2, TResult> (this object[] values, Func<T1, T2, TResult> func)
        {
            if (null == values)
                throw new ArgumentNullException ("values");
            if (null == func)
                throw new ArgumentNullException ("func");
            if (values.Length < 2)
                throw new MissingValuesException ("values", string.Format ("Cannot apply {0} objects to a {1}.", values.Length, func.GetType ()));
            TResult result = func ((T1)values [0], (T2)values [1]);
            return result;
        }    

        /// <summary>
        /// Applies an array of objects to a function.
        /// </summary>
        /// <returns>
        /// The result returned by <paramref name="func"/>.
        /// </returns>
        /// <param name='values'>
        /// Values to apply.
        /// </param>
        /// <param name='func'>
        /// Function to apply.
        /// </param>
        /// <param name='remainders'>
        /// Values ignored by <paramref name="func"/>.
        /// </param>
        /// <typeparam name='T1'>
        /// The 1st type parameter.
        /// </typeparam>
        /// <typeparam name='T2'>
        /// The 2nd type parameter.
        /// </typeparam>
        /// <typeparam name='TResult'>
        /// The type of the result.
        /// </typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> or <paramref name="func"/>
        /// is <see langword="null"/>.</exception>
        /// <exception cref="InvalidCastException">The first element of <paramref name="values"/>
        /// can not be casted to <typeparamref name="T1"/> or the second element can not be
        /// casted to <typeparamref name="T2"/>.</exception>
        /// <exception cref="MissingValuesException"><paramref name="values"/> contains too few elements
        /// to be used as arguments for <paramref name="func"/>.</exception>
        public static TResult ApplyTo<T1, T2, TResult> (this object[] values, Func<T1, T2, TResult> func, out object[] remainders)
        {
            if (null == values)
                throw new ArgumentNullException ("values");
            if (null == func)
                throw new ArgumentNullException ("func");
            if (values.Length < 2)
                throw new MissingValuesException ("values", string.Format ("Cannot apply {0} objects to a {1}.", values.Length, func.GetType ()));
            TResult result = func ((T1)values [0], (T2)values [1]);
            if (values.Length == 2) {
                remainders = _empty;
            } else {
                remainders = new object[values.Length - 2];
                Array.Copy (values, 2, remainders, 0, remainders.Length);
            }
            return result;
        }

        /// <summary>
        /// Applies an array of objects to a function.
        /// </summary>
        /// <returns>
        /// The result returned by <paramref name="func"/>.
        /// </returns>
        /// <param name='values'>
        /// Values to apply.
        /// </param>
        /// <param name='func'>
        /// Function to apply.
        /// </param>
        /// <typeparam name='T1'>
        /// The 1st type parameter.
        /// </typeparam>
        /// <typeparam name='T2'>
        /// The 2nd type parameter.
        /// </typeparam>
        /// <typeparam name='T3'>
        /// The 3rd type parameter.
        /// </typeparam>
        /// <typeparam name='TResult'>
        /// The type of the result.
        /// </typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> or <paramref name="func"/>
        /// is <see langword="null"/>.</exception>
        /// <exception cref="InvalidCastException">The first element of <paramref name="values"/>
        /// can not be casted to <typeparamref name="T1"/>, the second element can not be
        /// casted to <typeparamref name="T2"/> or the third element can not be casted to <typeparamref name="T3"/>.</exception>
        /// <exception cref="MissingValuesException"><paramref name="values"/> contains too few elements
        /// to be used as arguments for <paramref name="func"/>.</exception>
        public static TResult ApplyTo<T1, T2, T3, TResult> (this object[] values, Func<T1, T2, T3, TResult> func)
        {
            if (null == values)
                throw new ArgumentNullException ("values");
            if (null == func)
                throw new ArgumentNullException ("func");
            if (values.Length < 3)
                throw new MissingValuesException ("values", string.Format ("Cannot apply {0} objects to a {1}.", values.Length, func.GetType ()));
            TResult result = func ((T1)values [0], (T2)values [1], (T3)values [2]);
            return result;
        }

        /// <summary>
        /// Applies an array of objects to a function.
        /// </summary>
        /// <returns>
        /// The result returned by <paramref name="func"/>.
        /// </returns>
        /// <param name='values'>
        /// Values to apply.
        /// </param>
        /// <param name='func'>
        /// Function to apply.
        /// </param>
        /// <param name='remainders'>
        /// Values ignored by <paramref name="func"/>.
        /// </param>
        /// <typeparam name='T1'>
        /// The 1st type parameter.
        /// </typeparam>
        /// <typeparam name='T2'>
        /// The 2nd type parameter.
        /// </typeparam>
        /// <typeparam name='T3'>
        /// The 3rd type parameter.
        /// </typeparam>
        /// <typeparam name='TResult'>
        /// The type of the result.
        /// </typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> or <paramref name="func"/>
        /// is <see langword="null"/>.</exception>
        /// <exception cref="InvalidCastException">The first element of <paramref name="values"/>
        /// can not be casted to <typeparamref name="T1"/>, the second element can not be
        /// casted to <typeparamref name="T2"/> or the third element can not be casted to <typeparamref name="T3"/>.</exception>
        /// <exception cref="MissingValuesException"><paramref name="values"/> contains too few elements
        /// to be used as arguments for <paramref name="func"/>.</exception>
        public static TResult ApplyTo<T1, T2, T3, TResult> (this object[] values, Func<T1, T2, T3, TResult> func, out object[] remainders)
        {
            if (null == values)
                throw new ArgumentNullException ("values");
            if (null == func)
                throw new ArgumentNullException ("func");
            if (values.Length < 3)
                throw new MissingValuesException ("values", string.Format ("Cannot apply {0} objects to a {1}.", values.Length, func.GetType ()));
            TResult result = func ((T1)values [0], (T2)values [1], (T3)values [2]);
            if (values.Length == 3) {
                remainders = _empty;
            } else {
                remainders = new object[values.Length - 3];
                Array.Copy (values, 3, remainders, 0, remainders.Length);
            }
            return result;
        }    
        /// <summary>
        /// Applies an array of objects to a function.
        /// </summary>
        /// <returns>
        /// The result returned by <paramref name="func"/>.
        /// </returns>
        /// <param name='values'>
        /// Values to apply.
        /// </param>
        /// <param name='func'>
        /// Function to apply.
        /// </param>
        /// <typeparam name='T1'>
        /// The 1st type parameter.
        /// </typeparam>
        /// <typeparam name='T2'>
        /// The 2nd type parameter.
        /// </typeparam>
        /// <typeparam name='T3'>
        /// The 3rd type parameter.
        /// </typeparam>
        /// <typeparam name='T4'>
        /// The 4th type parameter.
        /// </typeparam>
        /// <typeparam name='TResult'>
        /// The type of the result.
        /// </typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> or <paramref name="func"/>
        /// is <see langword="null"/>.</exception>
        /// <exception cref="InvalidCastException">The first element of <paramref name="values"/>
        /// can not be casted to <typeparamref name="T1"/>, the second element can not be
        /// casted to <typeparamref name="T2"/>, the third element can not be casted to <typeparamref name="T3"/>
        /// or the fourth element can not be casted to <typeparamref name="T4"/>.</exception>
        /// <exception cref="MissingValuesException"><paramref name="values"/> contains too few elements
        /// to be used as arguments for <paramref name="func"/>.</exception>
        public static TResult ApplyTo<T1, T2, T3, T4, TResult> (this object[] values, Func<T1, T2, T3, T4, TResult> func)
        {
            if (null == values)
                throw new ArgumentNullException ("values");
            if (null == func)
                throw new ArgumentNullException ("func");
            if (values.Length < 4)
                throw new MissingValuesException ("values", string.Format ("Cannot apply {0} objects to a {1}.", values.Length, func.GetType ()));
            TResult result = func ((T1)values [0], (T2)values [1], (T3)values [2], (T4)values[3]);
            return result;
        }
    
        /// <summary>
        /// Applies an array of objects to a function.
        /// </summary>
        /// <returns>
        /// The result returned by <paramref name="func"/>.
        /// </returns>
        /// <param name='values'>
        /// Values to apply.
        /// </param>
        /// <param name='func'>
        /// Function to apply.
        /// </param>
        /// <param name='remainders'>
        /// Values ignored by <paramref name="func"/>.
        /// </param>
        /// <typeparam name='T1'>
        /// The 1st type parameter.
        /// </typeparam>
        /// <typeparam name='T2'>
        /// The 2nd type parameter.
        /// </typeparam>
        /// <typeparam name='T3'>
        /// The 3rd type parameter.
        /// </typeparam>
        /// <typeparam name='T4'>
        /// The 4th type parameter.
        /// </typeparam>
        /// <typeparam name='TResult'>
        /// The type of the result.
        /// </typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> or <paramref name="func"/>
        /// is <see langword="null"/>.</exception>
        /// <exception cref="InvalidCastException">The first element of <paramref name="values"/>
        /// can not be casted to <typeparamref name="T1"/>, the second element can not be
        /// casted to <typeparamref name="T2"/>, the third element can not be casted to <typeparamref name="T3"/>
        /// or the fourth element can not be casted to <typeparamref name="T4"/>.</exception>
        /// <exception cref="MissingValuesException"><paramref name="values"/> contains too few elements
        /// to be used as arguments for <paramref name="func"/>.</exception>
        public static TResult ApplyTo<T1, T2, T3, T4, TResult> (this object[] values, Func<T1, T2, T3, T4, TResult> func, out object[] remainders)
        {
            if (null == values)
                throw new ArgumentNullException ("values");
            if (null == func)
                throw new ArgumentNullException ("func");
            if (values.Length < 4)
                throw new MissingValuesException ("values", string.Format ("Cannot apply {0} objects to a {1}.", values.Length, func.GetType ()));
            TResult result = func ((T1)values [0], (T2)values [1], (T3)values [2], (T4)values[3]);
            if (values.Length == 4) {
                remainders = _empty;
            } else {
                remainders = new object[values.Length - 4];
                Array.Copy (values, 4, remainders, 0, remainders.Length);
            }
            return result;
        }
    }

}

