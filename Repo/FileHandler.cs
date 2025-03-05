using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Lexikope
{
	internal static class FileHandler
	{
		/// <summary>
		/// A beágyazott fájlok listázására szolgáló metódus.
		/// Ez kiírja az összes elérhető beágyazott fájl nevét a Debug konzolba.
		/// </summary>
		private static void ListEmbaddedFiles()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var resourceNames = assembly.GetManifestResourceNames();
			foreach (var name in resourceNames)
			{
				Debug.Print(name);
			}
		}
			

		/// <summary>
		/// Ellenőrzi, hogy egy adott beágyazott fájl létezik-e az erőforrások között.
		/// </summary>
		/// <param name="resourceName">A keresett beágyazott fájl neve.</param>
		/// <returns>Igaz, ha a fájl létezik, különben hamis.</returns>
		public static bool EmbeddedFileExists(string resourceName)
		{
			var assembly = Assembly.GetExecutingAssembly();
			var resourceNames = assembly.GetManifestResourceNames();

			return resourceNames.Contains(resourceName);
		}

		/// <summary>
		/// Ellenőrzi, hogy az összes `.txt` fájl létezik-e az adott platformon.
		/// Ha nem léteznek, akkor bemásolja a beágyazott erőforrások közül egy írható helyre (documents).
		/// Ha már léteznek, akkor nem frissíti őket.
		/// </summary>
		/// <exception cref="FileNotFoundException">
		/// Dobja ezt a kivételt, ha egy beágyazott `.txt` fájl nem található.
		/// </exception>
		public static async Task EnsureWritableFilesExist()
		{
			string targetFolder = GetDictionaryFolderPath(); // A célmappa meghatározása

			var assembly = Assembly.GetExecutingAssembly();
			var resourceNames = assembly.GetManifestResourceNames()
										.Where(name => name.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
										.ToList();

			if (!resourceNames.Any())
			{
				throw new FileNotFoundException("Nem található beágyazott `.txt` fájl a Resources mappában!");	
			}

			foreach (var resourceName in resourceNames)
			{
				// Fájlnév kinyerése a resource névből (pl. "Lexikope.Resources.dic.txt" → "dic.txt")
				string fileName = resourceName.Split('.').Skip(2).Aggregate((a, b) => $"{a}.{b}");
				string filePath = Path.Combine(targetFolder, fileName);

				// Ha a fájl már létezik, NEM FRISSÍTJÜK, csak kihagyjuk
				if (File.Exists(filePath))
				{
					Debug.Print($"Fájl már létezik, nem másolom: {filePath}");
					continue;
				}

				using Stream stream = assembly.GetManifestResourceStream(resourceName)
					?? throw new FileNotFoundException($"Nem található beágyazott fájl: {resourceName}");

				// Fájl létrehozása és másolás
				using FileStream outputStream = File.Create(filePath);
				await stream.CopyToAsync(outputStream);

				Debug.Print($"Fájl másolva: {filePath}");
			}
		}


		/// <summary>
		/// Ellenőrzi, hogy egy beágyazott fájl frissült-e a meglévő fájlhoz képest.
		/// </summary>
		/// <param name="filePath">A helyi fájl elérési útja.</param>
		/// <param name="embeddedStream">A beágyazott fájl adatfolyama.</param>
		/// <returns>Igaz, ha a fájlt frissíteni kell, különben hamis.</returns>
		private static bool IsEmbeddedFileUpdated(string filePath, Stream embeddedStream)
		{
			FileInfo fileInfo = new FileInfo(filePath);

			// Ha a fájl nem létezik, biztosan frissíteni kell
			if (!fileInfo.Exists) return true;

			// Ha a fájl mérete eltér, akkor frissítjük
			return fileInfo.Length != embeddedStream.Length;
		}


		/// <summary>
		/// Megnyitja a szótár fájlt és visszaad egy StreamReader példányt.
		/// </summary>
		/// <returns>StreamReader, amely az adott fájl tartalmát olvassa.</returns>
		/// <exception cref="FileNotFoundException">
		/// Dobja ezt a kivételt, ha a fájl nem található.
		/// </exception>
		public static StreamReader GetFileReader(string fileName)
		{
			string filePath = Path.Combine(GetDictionaryFolderPath(), $"{fileName}");
			Debug.Print($"A program itt keresi a fájlt: {GetDictionaryFolderPath()}");

			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException($"A fájl nem található! Elérési út: {filePath}");
			}

			return new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
		}

		/// <summary>
		/// Visszaadja a szótár fájl teljes elérési útját.
		/// </summary>
		/// <param name="dictionaryName">A szótár neve.</param>
		/// <returns>A szótár fájl teljes elérési útja.</returns>
		public static String GetCurrentFileFullName(string dictionaryName)
		{
			return Path.Combine(GetDictionaryFolderPath(), $"dic_{dictionaryName}.txt");
		}


		/// <summary>
		/// Visszaadja a szótár fájlok tárolási helyét az aktuális platformon.
		/// </summary>
		/// <returns>A szótár fájlok könyvtára.</returns>
		public static string GetDictionaryFolderPath()
		{
			string folderPath;

#if ANDROID
			// /storage/emulated/0/Documents (felhasználó által elérhető)
			folderPath = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments).AbsolutePath, "Lexikope");
#elif WINDOWS
			// \LexiKope\bin\Debug\net9.0-windows10.0.19041.0\win10-x64\AppX\Dictionaries
			folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dictionaries");
#else
			 folderPath = FileSystem.AppDataDirectory;
#endif

			// Ha nem létezik, akkor létrehozzuk a mappát
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			return folderPath;
		}



		/// <summary>
		/// Lekéri az elérhető szótárak neveit a tárolási helyről.
		/// </summary>
		/// <returns>Szótárnevek listája.</returns>
		public static List<string> GetDictionaryNames()
		{
			List<string> dic = new List<string>();
			string[] files= Directory.GetFiles(GetDictionaryFolderPath());

			foreach (string file in files)
			{
				Debug.Print(file);
				string fileName = Path.GetFileName(file).Substring(0,Path.GetFileName(file).Length-4); // Ez csak a fájlnevet adja vissza, elérési út és kiterjesztés nélkül

				if (fileName.StartsWith("dic_"))
				{
					var e = fileName.Length;
					dic.Add(fileName.Substring(4, fileName.Length-4));
				}
				if(fileName=="dic_")
				{
					File.Delete(file);
				}

			}
			return dic;

		}

		/// <summary>
		/// Elmenti a szótár adatait egy fájlba.
		/// </summary>
		/// <param name="dictionaryName">A szótár neve.</param>
		public static async Task SaveDictionary(string dictionaryName)
		{
			var outtext = Szotar.ToLexiKopeFormat();
			string fileFullName = GetCurrentFileFullName( dictionaryName);
			await AppendOrCreateFile(fileFullName, outtext);

#if WINDOWS
			//Teszt ellenörző fájl
			string appFolder = AppDomain.CurrentDomain.BaseDirectory;
			string filePath = Path.Combine(appFolder, $"Lexikope_{dictionaryName}.txt");
			Debug.Print(filePath);
			await AppendOrCreateFile(filePath, outtext);
#endif
#if ANDROID

			string filePath = Path.Combine("/storage/emulated/0/Download", $"Lexikope_{dictionaryName}.txt");
			Debug.Print(filePath);
			await AppendOrCreateFile(filePath, outtext);
#endif
		}

		/// <summary>
		/// Létrehozza vagy felülírja a megadott fájlt a tartalommal.
		/// </summary>
		/// <param name="filePath">A fájl elérési útja.</param>
		/// <param name="content">A fájlba írandó tartalom.</param>
		public static async Task AppendOrCreateFile(string filePath, string content)
		{
			try
			{
				await File.WriteAllTextAsync(filePath, content);
			}
			catch (Exception ex)
			{
				throw new Exception($"Hiba történt a fájl írása közben: {ex.Message}", ex);
			}
		}
	}
}
