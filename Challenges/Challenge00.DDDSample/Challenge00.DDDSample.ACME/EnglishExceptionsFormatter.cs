//
//  EnglishExceptionsFormatter.cs
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
	public sealed class ReturnMessages : CompositeVisitor<string>.VisitorBase, IVisitor<string, Exception>
	{
		public ReturnMessages(CompositeVisitor<string> composition)
			: base(composition)
		{
		}

		public string Visit(Exception target, IVisitContext context)
		{
			return target.Message;
		}
	}

	public sealed class ConstantMessage : CompositeVisitor<string>.VisitorBase, IVisitor<string, Exception>
	{
		private readonly string _msg;
		public ConstantMessage(CompositeVisitor<string> composition, string message)
			: base(composition)
		{
			_msg = message;
		}

		protected override IVisitor<string, TExpression> AsVisitor<TExpression> (TExpression target)
		{
			IVisitor<string, TExpression> visitor = base.AsVisitor (target);
			
			if(null == visitor || typeof(TExpression).Assembly.Equals(typeof(Challenge00.DDDSample.Cargo.ICargo).Assembly))
				return null;
			
			return visitor;
		}

		public string Visit(Exception target, IVisitContext context)
		{
			return _msg;
		}
	}

	public sealed class InnerDomainExceptionUnwrap : CompositeVisitor<string>.VisitorBase, IVisitor<string, Exception>
	{
		public InnerDomainExceptionUnwrap(CompositeVisitor<string> composition)
			: base(composition)
		{
		}
		
		protected override IVisitor<string, TExpression> AsVisitor<TExpression> (TExpression target)
		{
			IVisitor<string, TExpression> visitor = base.AsVisitor (target);
			
			if(null == visitor || typeof(TExpression).Assembly.Equals(typeof(Challenge00.DDDSample.Cargo.ICargo).Assembly))
				return null;
			
			return visitor;
		}
		
		public string Visit(Exception target, IVisitContext context)
		{
			if(null == target.InnerException)
			{
				return ContinueVisit(target, context);
			}
			else
			{
				return VisitInner(target.InnerException, context);
			}
		}
	}

	public sealed class EnglishExceptionsFormatter : CompositeVisitorBase<string, Exception>
	{
		public EnglishExceptionsFormatter ()
			: base("EnglishExceptionsMessages")
		{
			new ReturnMessages(this);
			new ConstantMessage(this, "An unexpected error occurred. Please contact the administrator.");
			new InnerDomainExceptionUnwrap(this);
			new Format<Location.WrongLocationException>(this,
			                                            e => e.Message.Substring(0, e.Message.LastIndexOf("\r\n")));
		}

		protected override IVisitContext InitializeVisitContext (Exception target, IVisitContext context)
		{
			return context;
		}
	}
}

