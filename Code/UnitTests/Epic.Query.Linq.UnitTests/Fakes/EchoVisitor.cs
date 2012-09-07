using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Epic.Query.Linq.Fakes
{
    public class EchoVisitor<TDerived> : CompositeVisitor<Expression>.VisitorBase, IVisitor<Expression, TDerived>
        where TDerived : Expression
    {
        private readonly TDerived _expression;
        public EchoVisitor(CompositeVisitor<Expression> composition, TDerived expression)
            : base(composition)
        {
            _expression = expression;
        }

        protected override IVisitor<Expression, TExpression> AsVisitor<TExpression>(TExpression target)
        {
            IVisitor<Expression, TExpression> visitor = base.AsVisitor<TExpression>(target);
            if (object.ReferenceEquals(target, _expression))
                return visitor;
            return null;
        }

        public Expression Visit(TDerived target, IVisitContext context)
        {
            return _expression;
        }
    }
}
