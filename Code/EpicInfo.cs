//  
//  EpicInfo.cs
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

// This file contains the informations shared between the different Epic.NET assemblies.

using System.Reflection;

// Giacomo is the copyright holder of Epic.NET
[assembly: AssemblyCopyright("Copyright (c) 2010-2012 Giacomo Tesio")]

// Epic is neither CLSCompliant nor ComVisible.
[assembly: System.Runtime.InteropServices.ComVisible(false)]
[assembly: System.CLSCompliant(false)]

// All Epic's assemblies share the same version
[assembly: AssemblyVersion("0.3.0.0")]
[assembly: AssemblyFileVersion("0.3.0.0")]
[assembly: AssemblyInformationalVersion("0.3")]


// The following attributes are used to specify the signing key for the assemblies, 
// if desired. See the Mono documentation for more information about signing.

//[assembly: AssemblyDelaySign(false)]
//[assembly: AssemblyKeyFile("")]
