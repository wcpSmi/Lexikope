using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Lexikope
{

	public static class LanguageMapper
	{
		private static readonly Dictionary<string, Dictionary<string, string>> languageMap = new()
		{
			{
				"samsung", new Dictionary<string, string>
				{
					{ "en", "eng" },
					{ "it", "ita" },
					{ "hu", "hun" },  // ha magyarra is kell speciális kód
					{ "de", "deu" }
				}
			},
			{
				"xiaomi", new Dictionary<string, string>
				{
					{ "en", "en" },
					{ "it", "it" },
					{ "hu", "hu" },
					{ "de", "de" }
				}
			},
			{
				"huawei", new Dictionary<string, string>
				{
					{ "en", "en" },
					{ "it", "it" },
					{ "hu", "hu" },
					{ "de", "de" }
				}
			},
			{
				"default", new Dictionary<string, string>
				{
					{ "en", "en" },
					{ "it", "it" },
					{ "hu", "hu" },
					{ "de", "de" }
				}
			}
		};
	

		public static string NormalizeLanguage(string device, string lang)
		{
			if (languageMap.TryGetValue(device.ToLower(), out var deviceLangs) && deviceLangs.TryGetValue(lang, out var normalizedLang))
			{
				return normalizedLang;
			}

			// Ha a készülék nincs a listában, használja az alapértelmezett beállítást
			return languageMap["default"].TryGetValue(lang, out var defaultLang) ? defaultLang : lang;
		}
	}

}
