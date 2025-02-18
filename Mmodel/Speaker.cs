

using Microsoft.Maui.Controls;

using System.Diagnostics;
using System.Runtime.Intrinsics.X86;




//Xaomi
//[0:] Nyelv: hu, Néven: Hungarian (Hungary)
//[0:] Nyelv: en, Néven: English (United States)
//[0:] Nyelv: de, Néven: German (Germany)
//[0:] Nyelv: it, Néven: Italian (Italy)

//Samsung A54
//[0:] Nyelv: spa, Néven: spanyol(USA, f00)
//[0:] Nyelv: ita, Néven: olasz(ITA, DEFAULT)
//[0:] Nyelv: deu, Néven: német(DEU, DEFAULT)
//[0:] Nyelv: eng, Néven: angol(GBR, DEFAULT)
//[0:] Nyelv: eng, Néven: angol(USA, l03)

namespace Lexikope
{
	internal static class Speaker
	{
		public async static void CheckDeviceTextToSpeachLangues()
		{
			var locales = await TextToSpeech.GetLocalesAsync();

			foreach (var locale in locales)
			{
				Debug.Print($"Nyelv: {locale.Language}, Néven: {locale.Name}");
			}

		}

		public async static Task Speech(string text, string lang)
		{
			//CheckDeviceTextToSpeachLangues();
			lang = MakeLangText(lang);

			var locales = await TextToSpeech.GetLocalesAsync();
			var selectedLocale = locales.FirstOrDefault(l => l.Language == lang);

			if (selectedLocale != null)
			{
				var settings = new SpeechOptions
				{
					Locale = selectedLocale, // Kiválasztott nyelv beállítása

				};

				await TextToSpeech.SpeakAsync(text, settings);
			}
			else
			{
				await TextToSpeech.SpeakAsync("A kiválasztott nyelv nem érhető el az eszközön.");
			}
			await Task.Delay(700);
		}

		private static string MakeLangText(string lang)
		{
			var device = GetDeviceInfo();
			switch (GetCurrentPlatform())
			{
				case "Android":
					return LanguageMapper.NormalizeLanguage(device,lang);

				case "WinUI":
					if (lang == "en")
					{
						return "en-GB";
					}
					else
					{
						return lang + "-" + lang.ToUpper();
					}
				default:
					return lang;
			}
		}

		private static string GetCurrentPlatform()
		{
			return DeviceInfo.Platform.ToString(); // "Android", "iOS", "MacCatalyst", "WinUI"
		}

		private static string GetDeviceInfo()
		{
			//Console.WriteLine($"Eszköz: {DeviceInfo.Manufacturer} - {DeviceInfo.Model}");
			//Console.WriteLine($"Operációs rendszer: {DeviceInfo.VersionString}");
			//Console.WriteLine($"Architektúra: {DeviceInfo.DeviceType}");

			return DeviceInfo.Manufacturer;
		}
	}
}
