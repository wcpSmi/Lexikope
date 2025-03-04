using Android.Views;
using Microsoft.Maui.ApplicationModel;
using Android.App;

namespace Lexikope.Services
{
	public class ScreenAwakeService :IScreenAwake
	{
		public void KeepScreenOn()
		{
			var activity = Platform.CurrentActivity;
			activity?.Window?.AddFlags(WindowManagerFlags.KeepScreenOn);
		}

		public void AllowScreenOff()
		{
			var activity = Platform.CurrentActivity;
			activity?.Window?.ClearFlags(WindowManagerFlags.KeepScreenOn);
		}
	}
}

