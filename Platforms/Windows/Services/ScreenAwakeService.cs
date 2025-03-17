

using Windows.System.Display;
using Windows.UI.Popups;

namespace Lexikope.Services
{
	public class ScreenAwakeService
	{
#if WINDOWS
		private DisplayRequest displayRequest;
		private bool isRequestActive = false;
#endif

		public void KeepScreenOn()
		{
#if ANDROID
        // Android specifikus megoldás (nincs szükség DisplayRequest-re)
        MainActivity.Instance.Window.AddFlags(Android.Views.WindowManagerFlags.KeepScreenOn);
#elif WINDOWS
			if (displayRequest == null)
			{
				displayRequest = new DisplayRequest();
			}

			if (!isRequestActive)
			{
				displayRequest.RequestActive();
				isRequestActive = true;
			}
#endif
		}

		public void AllowScreenOff()
		{
#if ANDROID
        // Android specifikus engedélyezés
        MainActivity.Instance.Window.ClearFlags(Android.Views.WindowManagerFlags.KeepScreenOn);
#elif WINDOWS
			if (isRequestActive && displayRequest != null)
			{
				displayRequest.RequestRelease();
				isRequestActive = false;
			}
#endif
		}
	}

}

