using Android.App;
using Android.Content.PM;
using Android.OS;

namespace Lexikope
{
	[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
	public class MainActivity : MauiAppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			RequestStoragePermissions();
		}

		private void RequestStoragePermissions()
		{
			if ((int)Build.VERSION.SdkInt >= 23)
			{
				if (CheckSelfPermission(Android.Manifest.Permission.ReadExternalStorage) != Permission.Granted ||
					CheckSelfPermission(Android.Manifest.Permission.WriteExternalStorage) != Permission.Granted)
				{
					RequestPermissions(new string[]
					{
					Android.Manifest.Permission.ReadExternalStorage,
					Android.Manifest.Permission.WriteExternalStorage
					}, 1);
				}
			}
		}
	}
}
