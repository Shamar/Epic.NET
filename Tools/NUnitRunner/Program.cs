//  
//  Program.cs
//  
//  Author:
//       Giacomo Tesio <giacomo@tesio.it>
// 
//  Copyright (c) 2010 Giacomo Tesio
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
using System.Windows.Forms;
using NUnit.Gui;
using System.IO;

namespace NUnitRunner
{
    static class Program
    {
		static string SolutionDirFile = "SolutionDir.txt";
		
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] argv)
        {
			if(argv.Length == 2 && argv[0] == "init")
			{
				File.WriteAllText(SolutionDirFile, argv[1]);
			}
			else if(!File.Exists(SolutionDirFile))
			{
				Console.WriteLine("Missing "+SolutionDirFile+": can not find All.nunit");
				Console.WriteLine("Write a post-build action like this: NUnitRunner.exe init ${SolutionDir}");
			}
			else
			{
				using(StreamReader rdr = File.OpenText(SolutionDirFile))
				{
					string solutionDir = rdr.ReadToEnd();
					AppEntry.Main(new string[2] { solutionDir + "/All.nunit", "-run" });
				}
			}
        }
    }
}
