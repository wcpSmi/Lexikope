﻿


using Lexikope.Mmodel;
using Microsoft.Maui.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Web;
using System.Windows.Input;

using Lexikope.Services;



namespace Lexikope
{
	public partial class MainPage : ContentPage
	{
		private ObservableCollection<SzotarBejegyzes> DisplayedItems = new ObservableCollection<SzotarBejegyzes>();

		Dictionary<int, List<SzotarBejegyzes>> book = new Dictionary<int, List<SzotarBejegyzes>>();

		private List<SzotarBejegyzes> FilteredList = new List<SzotarBejegyzes>();

		private readonly ScreenAwakeService screenAwakeService = new ScreenAwakeService();

		private const int PAGESIZE = 25; // Egyszerre betöltött elemek száma
		private int currentPage = 1;
		private int row = 0;
		private int lastPage = 1;

		//Vezérlők
		public Picker LanguagePickerPublic => LanguagePicker; //Ez lehetővé teszi az elérést kívülről

		public ICommand EditCommand { get; }

		public MainPage()
		{
			InitializeComponent();

			SetThemeColors();

			//Beágyazott fájl kimásolása elérhető helyre (AppDataDirectory) ha kell
			FileHandler.EnsureWritableFilesExist();


			//Szótárak picker feltöltése
			SetDictionaryChoicePicker();
			//Kategoriák
			LoadCategory();

			ResultsList.ItemsSource = DisplayedItems;

			LanguagePicker.SelectedIndex = 0;


			EditCommand = new Command<SzotarBejegyzes>(async item =>
			{
				if (item != null)
				{

					EditItem(item);
				}
			});

			// 🔹 A BindingContext beállítása (Fontos!)
			BindingContext = this;

		}


		private void SetDictionaryChoicePicker()
		{
			DictionaryPicker.Items.Clear();
			try
			{
				var diclist = FileHandler.GetDictionaryNames();
				DictionaryPicker.ItemsSource = diclist;
				UserDictionarySet();
			}
			catch (Exception)
			{

				throw;
			}

		}

		private void UserDictionarySet()
		{
			var savedUserDic = AppState.DictionaryIndex;
			DictionaryPicker.SelectedIndex = savedUserDic;
		}

		private void SetLanguagesPicker()
		{
			LanguagePicker.Items.Clear();
			if (DictionaryPicker.SelectedItem != null)
			{
				LanguagePicker.Items.Add(DictionaryPicker.SelectedItem.ToString());
			}

			LanguagePicker.Items.Add("hu");
			LanguagePicker.SelectedIndex = 0;
		}


		private void SetThemeColors()
		{
			var currentTheme = Application.Current.RequestedTheme;


			if (currentTheme == AppTheme.Dark)
			{
				BackgroundColor = (Color)Application.Current.Resources["BackgroundDark"];
				SelectedLabel.TextColor = (Color)Application.Current.Resources["TextDark"];
				PlayButton.BackgroundColor = AppState.ReaderOnPause ? Colors.Orange : PauseButton.BackgroundColor;

			}
			else
			{
				BackgroundColor = (Color)Application.Current.Resources["BackgroundLight"];
				SelectedLabel.TextColor = (Color)Application.Current.Resources["TextLight"];
				PlayButton.BackgroundColor = AppState.ReaderOnPause ? Colors.Orange : (Color)Application.Current.Resources["BackgroundLight"];
			}
		}

		private void LoadCategory()
		{
			var list = (from x in Szotar.Bejegyzesek
						group x by x.Category into grouped
						select grouped.Key).ToList();

			// Picker tisztítása
			CategoryPicker.Items.Clear();

			// "Minden kategória" hozzáadása az elejére
			CategoryPicker.Items.Add("Minden kategória");

			// Kategóriák hozzáadása a Pickerhez
			foreach (var item in list)
			{
				CategoryPicker.Items.Add(item);
			}

			// Alapértelmezett kiválasztás
			CategoryPicker.SelectedIndex = 0;
		}

		private void OnPrevClicked(object sender, EventArgs e)
		{
			//Debug.Print("Balra húzva!");
			if (currentPage > 1)
			{
				currentPage--;

				ShowPage(currentPage);
			}
		}

		private void OnNextClicked(object sender, EventArgs e)
		{
			//Debug.Print("Jobbra húzva!");
			if (currentPage < lastPage)
			{
				currentPage++;

				ShowPage(currentPage);
			}
		}

		private void ShowPage(int page = 1)
		{
			ResultsList.ItemsSource = null;
			ResultsList.SelectedItem = null;
			SelectedLabel.Text = "";

			if (FilteredList.Count > 0)
			{
				var startIndex = (page * PAGESIZE) - PAGESIZE;
				var endIndex = (page * PAGESIZE) - 1;
				DisplayedItems.Clear();
				for (int i = startIndex; i < endIndex && i < FilteredList.Count; i++)
				{
					DisplayedItems.Add(FilteredList[i]);
				}

				ListTitle.Text = $"{lastPage} / {page}. oldal (Összes találat: {FilteredList.Count}):";
			}
			else
			{
				SelectedLabel.Text = "???";
				ListTitle.Text = "Nincs találat!";
				DisplayedItems.Clear();
			}
			ResultsList.ItemsSource = DisplayedItems;



		}


		private void ReplaceCollection<T>(ref ObservableCollection<T> target, IEnumerable<T> source)
		{
			var tempList = source.ToList();
			target = new ObservableCollection<T>(tempList);
		}


		private void DictionaryPicker_SelectedIndexChanged(Object sender, EventArgs e)
		{
			//Menti a jelenlegi szótárt alap beállításként
			AppState.DictionaryIndex = DictionaryPicker.SelectedIndex;
			//Szótár beolvasása
			Szotar.LoadDictionary(DictionaryPicker.SelectedItem.ToString());
			//Beálltja a forrásnyelveket
			SetLanguagesPicker();
			LoadCategory();
		}

		private void LanguagePicker_SelectedIndexChanged(object sender, EventArgs e)
		{
			AppState.SourceLanguageIndex = LanguagePicker.SelectedIndex;
			string searchText = SearchEntry.Text ?? "";
			RefreshList();
		}

		private void CategoryPicker_SelectedIndexChanged(object sender, EventArgs e)
		{

			if (CategoryPicker.Items.Count != 0)
			{
				AppState.SelectedCategory = CategoryPicker.SelectedItem.ToString();
			}
			string searchText = SearchEntry.Text ?? "";
			RefreshList();

		}

		private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
		{
			string searchText = e.NewTextValue ?? "";
			RefreshList();
		}

		private void RefreshList()
		{
			FilteredList = Szotar.GetFilteredList(LanguagePicker.SelectedIndex, SearchEntry.Text, AppState.SelectedCategory);
			//currentPage = 1;
			lastPage = (FilteredList.Count / PAGESIZE) + 1;
			ShowPage();
		}

		private void OnItemTapped(object sender, EventArgs e)
		{
			if (sender is StackLayout layout && layout.BindingContext is SzotarBejegyzes tappedItem)
			{
				if (enabelPlay)
				{
					//return;
				}
				int index = DisplayedItems.IndexOf(tappedItem);
				row = index;
				// Az adott elem kiválasztása még akkor is, ha már korábban ki volt választva
				ResultsList.SelectedItem = tappedItem;

				// Kiírjuk a kiválasztott elem szövegét
				SelectedLabel.Text = AppState.SourceLanguageIndex == 0 ? tappedItem.Hungarian : tappedItem.OtherLanguage;

				var speakTxt = AppState.SourceLanguageIndex == 0 ? tappedItem.OtherLanguage : tappedItem.Hungarian;

				if (!string.IsNullOrEmpty(speakTxt))
				{
					Speaker.Speech(speakTxt, LanguagePicker.SelectedItem?.ToString());
				}
			}
		}



		private void OnEditClicked(object sender, EventArgs e)
		{
			var f = ResultsList.SelectedItem as SzotarBejegyzes;
			var existingItem = DisplayedItems.FirstOrDefault(x => x == f);

			EditItem(existingItem);

		}
		private async void EditItem(SzotarBejegyzes item = null)
		{
			string dictionaryName = DictionaryPicker.Items[AppState.DictionaryIndex].ToString();
			if (item != null)
			{
				await Navigation.PushModalAsync(new EditFrm(item, CategoryPicker.Items.ToList(), dictionaryName));
			}
			else
			{
				await Navigation.PushModalAsync(new EditFrm(CategoryPicker.Items.ToList(), dictionaryName));

			}

		}


		private void OnSpeakerClicked(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(SelectedLabel.Text))
			{
				var oppositeLanguageIndex = LanguagePicker.Items[AppState.SourceLanguageIndex ^ 1].ToString();
				Speaker.Speech(SelectedLabel.Text, oppositeLanguageIndex);

			}
		}

		private void OnQuizClicked(object sender, EventArgs e)
		{
			//DisplayAlert("Quiz", "Kvíz funkció indul!", "OK");
			Navigation.PushModalAsync(new QuizFrm(LanguagePicker));
		}


		private void OnTranslateClicked(object sender, EventArgs e)
		{
			var fromLang = LanguagePicker.Items[LanguagePicker.SelectedIndex];
			var toLang = LanguagePicker.Items[LanguagePicker.SelectedIndex ^ 1];
			Navigation.PushModalAsync(new TranlationFrm(SearchEntry.Text, fromLang, toLang));

		}

		private bool enabelPlay = false;

		private async void OnPlayClicked(object sender, EventArgs e)
		{
			//"▶" = &#x25B6;
			//	⏹ = &#x23F9;
			//⏸=&#x23F8;
			//⏺=&#x23FA;
			if (PlayButton.Text == "Play")
			{
				enabelPlay = true;
				PlayButton.Text = "Stop";

				var answer = await ShowDialog("Ébrentartás", "Akarod, hogy a felolvasás közben a képernyőt ébrentartsuk?", "Nem", "Igen");
				if (answer)
				{
					screenAwakeService.KeepScreenOn();
				}

				//beállítja az elmentett szűrőket
				CategoryPicker.SelectedIndex = AppState.ReaderOnPause ? AppState.ReaderCategoryIndex : CategoryPicker.SelectedIndex;

				SearchEntry.Text = AppState.ReaderOnPause ? AppState.ReaderFilterText : SearchEntry.Text;

				row = AppState.ReaderRowIndex ;

				//Elindítja a felolvasást a mentett Kategoriából , mentett oldalról
				currentPage = AppState.ReaderOnPause ? AppState.ReaderPage : currentPage;
				ReadingStart(currentPage);

			}
			else
			{
				ReadingResetSet();
			}
		}


		private void OnPauseClicked(object sender, EventArgs e)
		{
			ReadingPause();
			ShowDialog("Figyelem", "Az jelenlegi oldal rögzítve lett. A felolvasást innen folytathatod a 'Play' gomb megnyomásával.", "ok");
		}

		private async void ReadingStart(int page)
		{
			if (DisplayedItems.Count == 0)
				return;

			string sourceText = "";
			string destText = "";
			string forrasNyelv = LanguagePicker.SelectedItem.ToString();
			string ellentetesNyelv = LanguagePicker.Items[AppState.SourceLanguageIndex ^ 1].ToString();
			CategoryPicker.SelectedIndex = CategoryPicker.SelectedIndex;

			while (page <= lastPage && enabelPlay)
			{

				ShowPage(page);

				while (row < DisplayedItems.Count && enabelPlay)
				{
					sourceText = AppState.SourceLanguageIndex == 0 ? DisplayedItems[row].OtherLanguage : DisplayedItems[row].Hungarian;
					destText = AppState.SourceLanguageIndex == 0 ? DisplayedItems[row].Hungarian : DisplayedItems[row].OtherLanguage;

					//Kijelölés és görgetés az aktuális elemre
					await MainThread.InvokeOnMainThreadAsync(() =>
					{
						ResultsList.SelectedItem = DisplayedItems[row];
						ResultsList.ScrollTo(DisplayedItems[row], position: ScrollToPosition.Center, animate: true);
						SelectedLabel.Text = destText;
					});


					row++;

					await Speaker.Speech(sourceText, forrasNyelv);

					await Speaker.Speech(destText, ellentetesNyelv);

				}

				row = 0;
				page++;
			}
			//Felolvasás végén a gomb és a lista alaphelyzetbe állítás
			ReadingStop();
		}

		private void ReadingStop()
		{
			enabelPlay = false;
			PlayButton.Text = "Play";
			SelectedLabel.Text = "";
			ResultsList.SelectedItem = null;

			//Elengedjük a képernyő ébrentartását ha az be volt kapcsolva
			screenAwakeService.AllowScreenOff();
		}

		private void ReadingPause()
		{
			ReadingStop();

			AppState.ReaderCategoryIndex = CategoryPicker.SelectedIndex;
			AppState.ReaderFilterText = SearchEntry.Text;
			AppState.ReaderPage = currentPage;
			AppState.ReaderOnPause = true;
			AppState.ReaderRowIndex = row;
			SetThemeColors();
		}

		private void ReadingResetSet()
		{
			ReadingStop();

			CategoryPicker.SelectedIndex = 0;
			currentPage = 1;
			row = 0;

			AppState.ReaderCategoryIndex = 0;
			AppState.ReaderFilterText = "";
			AppState.ReaderPage = 1;
			AppState.ReaderOnPause = false;
			AppState.ReaderRowIndex = row;
			SetThemeColors();
			ShowPage();
		}

		public async Task<bool> ShowDialog(string title, string text, string buttonCancel, string buttonOk = "")
		{

			if (string.IsNullOrWhiteSpace(buttonOk))
			{
				await DisplayAlert(title, text, buttonCancel);
				return false;
			}
			else
			{
				return await DisplayAlert(title, text, buttonOk, buttonCancel);
			}
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			ResultsList.SelectedItem = null;
			SelectedLabel.Text = string.Empty;
			RefreshList();
			LoadCategory();
		}


	}
}


