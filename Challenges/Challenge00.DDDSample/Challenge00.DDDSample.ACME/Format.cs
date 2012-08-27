//
//  Format.cs
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
using Epic;
namespace Challenge00.DDDSample.ACME
{
	public sealed class Format<TException> : CompositeVisitor<string>.VisitorBase, IVisitor<string, TException>
	    where TException : Exception
	{
	    private readonly Func<TException, string> _format;
	    private readonly Func<TException, bool> _acceptanceRule;
	    public Format(CompositeVisitor<string> composition, Func<TException, string> format)
	        : this(composition, format, e => true)
	    {
	    }

	    public Format(CompositeVisitor<string> composition, Func<TException, string> format, Func<TException, bool> acceptanceRule)
	        : base(composition)
	    {
	        if (null == format)
	            throw new ArgumentNullException("format");
	        if (null == acceptanceRule)
	            throw new ArgumentNullException("acceptanceRule");
	        _format = format;
	        _acceptanceRule = acceptanceRule;
	    }

	    protected override IVisitor<string, TExpression> AsVisitor<TExpression>(TExpression target)
	    {
	        IVisitor<string, TExpression> visitor = base.AsVisitor(target);

	        if (null == visitor || !_acceptanceRule(target as TException))
	            return null;

	        return visitor;
	    }

	    #region IVisitor implementation
	    public string Visit(TException target, IVisitContext context)
	    {
	        return _format(target);
	    }
	    #endregion
	}
}
