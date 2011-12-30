//  
//  Predicate.cs
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

namespace Epic.Linq.Expressions.Relational.Predicates
{
    /// <summary>
    /// Base class for Predicates.
    /// </summary>
    [Serializable]
    public abstract class Predicate : VisitableBase, IEquatable<Predicate>
    {
        #region IEquatable[Predicate] implementation
        /// <summary>
        /// Determines whether the specified <see cref="Predicate"/> is equal to the current <see cref="Epic.Linq.Expressions.Relational.Predicate"/>.
        /// </summary>
        /// <param name='other'>
        /// The <see cref="Predicate"/> to compare with the current <see cref="Epic.Linq.Expressions.Relational.Predicate"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Predicate"/> is equal to the current
        /// <see cref="Epic.Linq.Expressions.Relational.Predicate"/>; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool Equals (Predicate other);
        
        #endregion
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Epic.Linq.Expressions.Relational.Predicate"/>.
        /// </summary>
        /// <param name='obj'>
        /// The <see cref="System.Object"/> to compare with the current <see cref="Epic.Linq.Expressions.Relational.Predicate"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="Epic.Linq.Expressions.Relational.Predicate"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals (object obj)
        {
            return Equals (obj as Predicate);
        }
        /// <summary>
        /// Serves as a hash function for a <see cref="Epic.Linq.Expressions.Relational.Predicate"/> object.
        /// </summary>
        /// <returns>
        /// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.
        /// </returns>
        public override int GetHashCode ()
        {
            return GetType().GetHashCode();
        }
    }








//    /// <summary>
//    /// Base class for Predicates having two Scalar as operands.
//    /// E.g. Equals, Greater, LessOrEqual
//    /// </summary>
//    [Serializable]
//    public abstract class ScalarPredicateBase: Predicate, IEquatable<ScalarPredicateBase>
//    {
//        Scalar _left;
//        Scalar _right;
//
//        /// <summary>
//        /// Initializes a new instance of the <see cref="Epic.Linq.Expressions.Relational.ScalarPredicateBase"/> class.
//        /// </summary>
//        /// <param name='leftOperand'>
//        /// Left operand.
//        /// </param>
//        /// <param name='rightOperand'>
//        /// Right operand.
//        /// </param>
//        protected ScalarPredicateBase (Scalar leftOperand, Scalar rightOperand)
//        {
//            this._left = leftOperand;
//            this._right = rightOperand;
//        }
//
//        /// <summary>
//        /// Gets the left operand of the Predicate.
//        /// </summary>
//        /// <value>
//        /// The left operand.
//        /// </value>
//        public Scalar Left { get { return this._left; } }
//
//        /// <summary>
//        /// Gets the right operand of the Predicate.
//        /// </summary>
//        /// <value>
//        /// The right operand.
//        /// </value>
//        public Scalar Right { get { return this._right; } }
//
//        /// <summary>
//        /// Determines whether the specified <see cref="Predicate"/> is equal to the current <see cref="Epic.Linq.Expressions.Relational.ScalarPredicateBase"/>.
//        /// </summary>
//        /// <param name='other'>
//        /// The <see cref="Predicate"/> to compare with the current <see cref="Epic.Linq.Expressions.Relational.ScalarPredicateBase"/>.
//        /// </param>
//        /// <returns>
//        /// <c>true</c> if the specified <see cref="Predicate"/> is equal to the current
//        /// <see cref="Epic.Linq.Expressions.Relational.ScalarPredicateBase"/>; otherwise, <c>false</c>.
//        /// </returns>
//        public override bool Equals (Predicate other)
//        {
//            return Equals(other as ScalarPredicateBase);
//        }
//
//        /// <summary>
//        /// Determines whether the specified <see cref="ScalarPredicateBase"/> is equal to the current <see cref="Epic.Linq.Expressions.Relational.ScalarPredicateBase"/>.
//        /// </summary>
//        /// <param name='other'>
//        /// The <see cref="ScalarPredicateBase"/> to compare with the current <see cref="Epic.Linq.Expressions.Relational.ScalarPredicateBase"/>.
//        /// </param>
//        /// <returns>
//        /// <c>true</c> if the specified <see cref="ScalarPredicateBase"/> is equal to the current
//        /// <see cref="Epic.Linq.Expressions.Relational.ScalarPredicateBase"/>; otherwise, <c>false</c>.
//        /// </returns>
//        public abstract bool Equals (ScalarPredicateBase other);
//
//        /// <summary>
//        /// Serves as a hash function for a <see cref="Epic.Linq.Expressions.Relational.ScalarPredicateBase"/> object.
//        /// </summary>
//        /// <returns>
//        /// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
//        /// hash table.
//        /// </returns>
//        public override int GetHashCode ()
//        {
//            return _left.GetHashCode() ^ _right.GetHashCode () ^ GetType ().GetHashCode ();
//        }
//    }

//    /// <summary>
//    /// Base class for Predicates having two Predicates as operands.
//    /// E.g. AND, OR, XOR
//    /// </summary>
//    [Serializable]
//    public abstract class PredicateBase: Predicate, IEquatable<PredicateBase>
//    {
//        Predicate _left;
//        Predicate _right;
//
//        /// <summary>
//        /// Initializes a new instance of the <see cref="Epic.Linq.Expressions.Relational.PredicateBase"/> class.
//        /// </summary>
//        /// <param name='leftOperand'>
//        /// Left operand.
//        /// </param>
//        /// <param name='rightOperand'>
//        /// Right operand.
//        /// </param>
//        protected PredicateBase (Predicate leftOperand, Predicate rightOperand)
//        {
//            this._left = leftOperand;
//            this._right = rightOperand;
//        }
//
//        /// <summary>
//        /// Gets the left operand of the Predicate.
//        /// </summary>
//        /// <value>
//        /// The left operand.
//        /// </value>
//        public Predicate Left { get { return this._left; } }
//
//        /// <summary>
//        /// Gets the right operand of the Predicate.
//        /// </summary>
//        /// <value>
//        /// The right operand.
//        /// </value>
//        public Predicate Right { get { return this._right; } }
//
//        /// <summary>
//        /// Determines whether the specified <see cref="Predicate"/> is equal to the current <see cref="Epic.Linq.Expressions.Relational.PredicateBase"/>.
//        /// </summary>
//        /// <param name='other'>
//        /// The <see cref="Predicate"/> to compare with the current <see cref="Epic.Linq.Expressions.Relational.PredicateBase"/>.
//        /// </param>
//        /// <returns>
//        /// <c>true</c> if the specified <see cref="Predicate"/> is equal to the current
//        /// <see cref="Epic.Linq.Expressions.Relational.PredicateBase"/>; otherwise, <c>false</c>.
//        /// </returns>
//        public override bool Equals (Predicate other)
//        {
//            return Equals (other as PredicateBase);
//        }
//
//        /// <summary>
//        /// Determines whether the specified <see cref="PredicateBase"/> is equal to the current <see cref="Epic.Linq.Expressions.Relational.PredicateBase"/>.
//        /// </summary>
//        /// <param name='other'>
//        /// The <see cref="PredicateBase"/> to compare with the current <see cref="Epic.Linq.Expressions.Relational.PredicateBase"/>.
//        /// </param>
//        /// <returns>
//        /// <c>true</c> if the specified <see cref="PredicateBase"/> is equal to the current
//        /// <see cref="Epic.Linq.Expressions.Relational.PredicateBase"/>; otherwise, <c>false</c>.
//        /// </returns>
//        public abstract bool Equals(PredicateBase other);
//
//        /// <summary>
//        /// Serves as a hash function for a <see cref="Epic.Linq.Expressions.Relational.PredicateBase"/> object.
//        /// </summary>
//        /// <returns>
//        /// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
//        /// hash table.
//        /// </returns>
//        public override int GetHashCode ()
//        {
//            return _left.GetHashCode() ^ _right.GetHashCode () ^ GetType ().GetHashCode ();
//        }
//    }

//    [Serializable]
//    public sealed class EqualsPredicate: ScalarPredicateBase
//    {
//        #region implemented abstract members of Epic.Linq.Expressions.Relational.ScalarPredicateBase
//        public override bool Equals (ScalarPredicateBase other)
//        {
//            return (this.Left.Equals (other.Left) && this.Right.Equals (other.Right)) ||
//                (this.Left.Equals (other.right) && (this.Right.Equals (other.Left)));
//        }
//        #endregion
//
//        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
//        {
//            return AcceptMe(this, visitor, context);
//        }
//    }
//
//    public sealed class GreaterThanPredicate: ScalarPredicateBase
//    {
//        #region implemented abstract members of Epic.Linq.Expressions.Relational.ScalarPredicateBase
//        public override bool Equals (ScalarPredicateBase other)
//        {
//            return this.Left.Equals (other.Left) && this.Right.Equals (other.Right) ;
//        }
//        #endregion
//
//        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
//        {
//            return AcceptMe (this, visitor, context);
//        }
//    }
//
//    public sealed class LessThanPredicate: ScalarPredicateBase
//    {
//        #region implemented abstract members of Epic.Linq.Expressions.Relational.ScalarPredicateBase
//        public override bool Equals (ScalarPredicateBase other)
//        {
//            return this.Left.Equals (other.Left) && this.Right.Equals (other.Right) ;
//        }
//        #endregion
//
//        public override TResult Accept<TResult> (IVisitor<TResult> visitor, IVisitContext context)
//        {
//            return AcceptMe (this, visitor, context);
//        }
//    }
}

