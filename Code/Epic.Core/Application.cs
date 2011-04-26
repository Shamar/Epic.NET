//  
//  Application.cs
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
using System.Configuration;
namespace Epic
{
	public static class Application
	{
		private static ApplicationBase _application;
		
		public static void Initialize(ApplicationBase application)
		{
			if(null == application)
				throw new ArgumentNullException("application");
			if(null != _application)
				throw new InvalidOperationException("Already initialized.");
			_application = application;
		}
		
		public static string Name
		{
			get 
			{
				return _application.Name;
			}
		}

		public static IEnvironment Environment
		{
			get 
			{
				return _application.Environment;
			}
		}


		public static IEnterprise Enterprise
		{
			get 
			{
				return _application.Enterprise;
			}
		}
	}
}

