//  
//  FakeObserver.cs
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

namespace Epic.Fakes
{
    public class FakeObserver<TEntity> : ObserverBase<TEntity>
         where TEntity : class
    {
        #region templates for tests
        public virtual void CallSubscribe (TEntity entity)
        {
        }
            
        public virtual void CallUnsubscribe (TEntity entity)
        {
        }
        #endregion templates for tests
                
        #region implemented abstract members of Epic.ObserverBase[TEntity,TIdentifier]
        protected override void Subscribe (TEntity entity)
        {
            CallSubscribe (entity);
        }

        protected override void Unsubscribe (TEntity entity)
        {
            CallUnsubscribe (entity);
        }
        #endregion
    }
}

