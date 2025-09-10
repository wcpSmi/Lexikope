
using CommunityToolkit.Mvvm.Messaging.Messages;
using Lexikope;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Lexikope
{

	internal static class Szotar
	{


		private static List<SzotarBejegyzes> bejegyzesek = new List<SzotarBejegyzes>();

		internal static List<SzotarBejegyzes> Bejegyzesek { get => bejegyzesek; private set => bejegyzesek = value; }

		public static void Add(SzotarBejegyzes bejegyzes)
		{
			Bejegyzesek.Add(bejegyzes);
		}
		public static string GetHunWord(string engWord)
		{
			var hunWord = Bejegyzesek.FirstOrDefault(bejegyzes => bejegyzes.OtherLanguage == engWord)?.Hungarian;
			if (hunWord != null)
			{
				return hunWord;
			}
			return "NULL";
		}
		public static string GetEngWord(string hunWord)
		{
			var engWord = Bejegyzesek.FirstOrDefault(bejegyzes => bejegyzes.Hungarian == hunWord)?.OtherLanguage;
			if (engWord != null)
			{
				return engWord;
			}
			return "NULL";
		}

		/// <summary>
		/// Szótárelemek listáját szűri a megadott kategória és kezdő karakterek szerint.
		/// </summary>
		/// <param name="selectedLangueID">0 = Idegen nyelv, 1 = Magyar</param>
		/// <param name="startCharacters">A szó eleje, amely alapján szűrni kell</param>
		/// <param name="kategoria">A kategória, amely alapján szűrni kell (alapértelmezett: "Minden kategória")</param>
		/// <returns>A szűrt szótárelemek listája</returns>
		public static List<SzotarBejegyzes> GetFilteredList(int selectedLangueID, string startCharacters = "", string kategoria = "Minden kategória")
		{
			if (kategoria != null)
			{
				startCharacters = startCharacters == null ? string.Empty : startCharacters;

				return Bejegyzesek
							.Where(x => (kategoria == "Minden kategória" || x.Category == kategoria) &&
										(string.IsNullOrEmpty(startCharacters) ||
										 (selectedLangueID == 0 ? x.OtherLanguage.StartsWith(startCharacters, StringComparison.OrdinalIgnoreCase)
																: x.Hungarian.StartsWith(startCharacters, StringComparison.OrdinalIgnoreCase))))
							.OrderBy(x => selectedLangueID == 0 ? x.OtherLanguage : x.Hungarian)
							.ToList();
			}
			return new List<SzotarBejegyzes>();
		}

		/// <summary>
		/// Oldalakra bontja a szótárt, ahol minden oldal egy meghatározott számú szót tartalmaz.
		/// </summary>
		/// <param name="wordOnPage">Hány szó fér el egy oldalon</param>
		/// <param name="selectedLangueID">0 = Idegen nyelv, 1 = Magyar</param>
		/// <param name="startCharacters">A szó eleje, amely alapján szűrni kell</param>
		/// <param name="kategoria">A kategória, amely alapján szűrni kell (alapértelmezett: "Minden kategória")</param>
		/// <returns>Szótár oldalakra bontva, ahol a kulcs az oldal száma, az érték a szavak listája</returns>
		public static Dictionary<int,List<SzotarBejegyzes>> GetDictionayBook(int wordOnPage,int selectedLangueID, string startCharacters = "", string kategoria = "Minden kategória")
		{
			Dictionary<int, List<SzotarBejegyzes>> book = new Dictionary<int, List<SzotarBejegyzes>>();
			List<SzotarBejegyzes> pageList =new List<SzotarBejegyzes> ();

			var filteredList = GetFilteredList(selectedLangueID, startCharacters, kategoria);
			int page = 1;
			for (int i = 0; i < filteredList.Count; i++)
			{	
				pageList.Add(filteredList[i]);

				if(pageList.Count >= wordOnPage || i==filteredList.Count-1)
				{
					book.Add(page, new List<SzotarBejegyzes>(pageList)); 
					pageList.Clear();
					page++;
				}
			}
		return book;
		}

		public static void LoadDictionary(string dictionaryName)
		{
			Bejegyzesek.Clear();
			using StreamReader reader = FileHandler.GetFileReader($"dic_{dictionaryName}.txt");

			string kategoria = string.Empty;
			string magyarSzo = string.Empty;
			string angolSzo = string.Empty;
			string sor = string.Empty;
			int sorSzám = 0;

			while ((sor = reader.ReadLine()) != null)
			{
				sorSzám++;
				//csak ellenőrzésre
				if(sorSzám==4152)
				{
					Debug.Print(sor);
				}
				Debug.Print(sorSzám.ToString());
				if (!sor.StartsWith('$'))
				{
					if (!string.IsNullOrEmpty(sor) && sor != null)
					{
						if (sor.Substring(0, 1) == "#")
						{
							kategoria = sor.Substring(1, sor.Length - 1);
						}
						else
						{
							var items = sor.Split('\t');
							var bejegyzes = new SzotarBejegyzes(items[1].ToLower().Trim(), items[0].ToLower().Trim(), kategoria);
							Bejegyzesek.Add(bejegyzes);

						}
					}
				}
			}
		}

		public static void RemoveEntry(SzotarBejegyzes entry)
		{
			if (Bejegyzesek.Contains(entry))
			{
				Bejegyzesek.Remove(entry);
			}
		}

		public static void ModifyItem(SzotarBejegyzes item, SzotarBejegyzes newItem)
		{

			item.Hungarian = newItem.Hungarian;
			item.OtherLanguage = newItem.OtherLanguage;			
		}

		public static string ToLexiKopeFormat()
		{
			string outText = string.Empty;

			var kategoriak=bejegyzesek
				.Select(x=> x.Category) // Csak a kategória mezőre van szükség
				.Distinct()// Egyedi értékek kiszűrése
				.OrderBy(x=>x)// Egyedi értékek kiszűrése
				.ToList();

			

			foreach (var kategoria in kategoriak)
			{
				outText += $"\n\n#{kategoria}\n";
				var items = bejegyzesek
					.Where(x=>x.Category == kategoria)
					.ToList();
				foreach(var item in items)
				{
					outText += $"\n{item.OtherLanguage}\t{item.Hungarian}";
				}
			}
			return outText;
		}

	}
}
