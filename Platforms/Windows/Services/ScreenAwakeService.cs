

using Windows.System.Display;
using Windows.UI.Popups;

namespace Lexikope.Services
{
	public class ScreenAwakeService
	{
		private DisplayRequest displayRequest = new();

		public void KeepScreenOn()
		{
			displayRequest.RequestActive();
		}

		public void AllowScreenOff()
		{
			displayRequest.RequestRelease();
		}
	}
}

