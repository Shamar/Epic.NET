//
//  ItalianExceptionsFormatter.cs
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
	public sealed class ItalianExceptionsFormatter : CompositeVisitorBase<string, Exception>
	{
        public ItalianExceptionsFormatter()
            : base("ItalianExceptionsFormatter")
		{
			new ConstantMessage(this, "Si è verificato un errore imprevisto. Contattare l'amministratore del sistema.");
			new InnerDomainExceptionUnwrap(this);
			new Format<Location.WrongLocationException>(this,
			                                            e => string.Format("Non è possibile effettuare l'operazione, perché la località fornita ({0}) non coincide con quella prevista ({1}).", e.ActualLocation, e.ExpectedLocation));
			new Format<Voyage.VoyageCompletedException>(this,
			                                            e => string.Format("Il viaggio '{0}' ha già raggiunto la propria destinazione.", e.Voyage));
			new Format<Cargo.AlreadyClaimedException>(this,
                                                      e => string.Format("Non è possibile effettuare l'operazione perché il cargo '{0}' è già stato ritirato.", e.Cargo));
			new Format<Cargo.RoutingException>(this,
                                               e => string.Format("Non è possibile effettuare l'operazione perché il cargo '{0}' è stato diretto su un percorso sbagliato.", e.Cargo),
			                                   e => e.RoutingStatus == Challenge00.DDDSample.Cargo.RoutingStatus.Misrouted);
			new Format<Cargo.RoutingException>(this,
                                               e => string.Format("Non è possibile effettuare l'operazione perché non è ancora stato assegnato un percorso al cargo '{0}'.", e.Cargo),
			                                   e => e.RoutingStatus == Challenge00.DDDSample.Cargo.RoutingStatus.NotRouted);
			new Format<Cargo.RoutingException>(this,
			                                   e => "Si è verificato un errore imprevisto. Contattare l'amministratore del sistema.",
			                                   e => e.RoutingStatus == Challenge00.DDDSample.Cargo.RoutingStatus.Routed);
		}

		protected override IVisitContext InitializeVisitContext (Exception target, IVisitContext context)
		{
			return context;
		}
	}
}

