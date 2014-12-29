using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Addle.Wpf.ViewModel;

namespace Addle.AutoVMTest
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			var app = new App();
			var window = new MainWindow { DataContext = AutoVMFactory.Create<MainWindowVM>() };
			app.Run(window);
		}
	}
}
