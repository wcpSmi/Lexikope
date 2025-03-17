using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexikope.Mmodel
{
	internal static class AppState
	{
		//Szótár változás
		public static bool WasChanged = false;
		private static int? dictionaryIndex = null; // Nullable, hogy ne kelljen mindig újra lekérdezni
	
		public static int DictionaryIndex
		{
			get
			{
				if (dictionaryIndex == null) // Csak egyszer olvassuk be a Preferences-ből
				{
					dictionaryIndex = Preferences.Get("DictionaryIndex", 0);
				}
				return dictionaryIndex.Value;
			}
			set
			{
				if (dictionaryIndex != value) // Csak akkor írjuk be, ha változott az érték
				{
					dictionaryIndex = value;
					Preferences.Set("DictionaryIndex", value);
				}
			}
		}

		public static string SelectedCategory { get; set; }
		public static int SourceLanguageIndex { get; set; }


		//Reader
		private static int? readerPage;

		public static int ReaderPage
		{
			get
			{
				if (readerPage == null) // Csak egyszer olvassuk be a Preferences-ből
				{
					readerPage = Preferences.Get("BookPage", 1);
				}
				return readerPage.Value;
			}
			set
			{
				if (readerPage != value) // Csak akkor írjuk be, ha változott az érték
				{
					readerPage = value;
					Preferences.Set("BookPage", value);
				}
			}
		}
		public static bool ReaderOnPause
		{
			get
			{
				return Preferences.Get("OnPause", false);
			}
			set
			{
				Preferences.Set("OnPause", value);

			}
		}
		public static int ReaderRowIndex
		{
			get
			{
				return Preferences.Get("RowIndex", 0);
			}
			set
			{
				Preferences.Set("RowIndex", value);

			}
		}

		public static int ReaderCategoryIndex
		{
			get
			{
				return Preferences.Get("CategoryIndex", 0);
			}
			set
			{
				Preferences.Set("CategoryIndex", value);

			}
		}
		public static string ReaderFilterText
		{
			get
			{
				return Preferences.Get("FilterText", "");
			}
			set
			{
				Preferences.Set("FilterText", value);

			}
		}
	}
}
