using System;
using System.Windows.Forms;
using NUnit.Gui;

namespace NUnitRunner
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppEntry.Main(new string[0]);
        }
    }
}
