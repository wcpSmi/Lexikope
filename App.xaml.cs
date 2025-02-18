using Lexikope.Mmodel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using Microsoft.Maui.Hosting;
using Application = Microsoft.Maui.Controls.Application;

namespace Lexikope
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();
			MainPage =new AppShell();
		}
		protected override void OnSleep()
		{
			base.OnSleep();

		}


	}
}


