using Lexikope.Mmodel;
using System.Diagnostics;

namespace Lexikope;

public partial class EditFrm : ContentPage
{
	private string dicName;
	private SzotarBejegyzes szotarBejegyzes;
	private string otherLang;
	private string hunLang;
	private string category;

	public string OtherLang
	{
		get => otherLang;
		set => otherLang = value?.Trim() ?? throw new ArgumentException("Az idegen nyelv kitöltése kötelezõ!");
	}

	public string HunLang
	{
		get => hunLang;
		set => hunLang = value?.Trim() ?? throw new ArgumentException("A magyar nyelv kitöltése kötelezõ!");
	}

	public string Category
	{
		get => category;
		set => category = value?.Trim() ?? throw new ArgumentException("A kategória kitöltése kötelezõ!");
	}


	public EditFrm(List<string> categoryList, string dictionaryName)
	{
		InitializeComponent();
		CategPickerLoad(categoryList);
		dicName = dictionaryName ?? string.Empty;
	}

	public EditFrm(SzotarBejegyzes bejegyzes, List<string> categoryList, string dictionaryName)
	{
		InitializeComponent();

		szotarBejegyzes = bejegyzes;

		CategPickerLoad(categoryList);

		OtherLangEntry.Text = bejegyzes.OtherLanguage;
		HunLangEntry.Text = bejegyzes.Hungarian;
		CategoryPicker.SelectedItem = bejegyzes.Category;

		dicName = dictionaryName ?? string.Empty;
	}

	private void CategPickerLoad(List<string> categoryList)
	{
		for (int i = 1; i < categoryList.Count; i++)
		{
			CategoryPicker.Items.Add(categoryList[i]);
		}
	}

	private void OnCategoryChanged(object sender, EventArgs e)
	{
		if (CategoryPicker.SelectedIndex > 0)
		{
			CategoryEntry.Text = CategoryPicker.SelectedItem?.ToString() ?? string.Empty;
		}
	}



	private async void OnCancelClicked(object sender, EventArgs e)
	{
		await Navigation.PopModalAsync();//Bezár és visszatér a hívó lapra

	}

	private async void OnNewSaveClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(OtherLang) || string.IsNullOrWhiteSpace(HunLang) || string.IsNullOrWhiteSpace(Category))
		{
			await DisplayAlert("Hiba", "Minden mezõ kitöltése kötelezõ!", "OK");
			return;
		}

		try
		{
			Szotar.Add(new SzotarBejegyzes(HunLang, OtherLang, Category));
			FileHandler.SaveDictionary(dicName);
			await Navigation.PopModalAsync(); // Visszatérés
		}
		catch (Exception ex)
		{
			await DisplayAlert("Hiba", $"Nem sikerült menteni: {ex.Message}", "OK");
		}
	}


	private async void OnDeletClicked(object sender, EventArgs e)
	{
		if (szotarBejegyzes == null)
		{
			await DisplayAlert("Hiba", "Nincs feltöltve bejegyzés amit törölhetnék", "OK");
			return;
		}

		bool confirm = await DisplayAlert("Megerõsítés", "Biztosan törölni szeretnéd ezt a bejegyzést?", "Igen", "Mégse");

		if (confirm)
		{
			try
			{
				Szotar.RemoveEntry(szotarBejegyzes);
				FileHandler.SaveDictionary(dicName);
				await Navigation.PopModalAsync(); // Bezárás és visszatérés
			}
			catch (Exception ex)
			{
				await DisplayAlert("Hiba", $"Nem sikerült törölni: {ex.Message}", "OK");
			}
		}
	}

	private async void OnModifySaveClicked(object sender, EventArgs e)
	{
		if (szotarBejegyzes == null)
		{
			await DisplayAlert("Hiba", "Nincs feltöltve módosítható bejegyzés!", "OK");
			return;
		}

		if (string.IsNullOrWhiteSpace(OtherLangEntry.Text) || string.IsNullOrWhiteSpace(HunLangEntry.Text) || string.IsNullOrWhiteSpace(CategoryEntry.Text))
		{
			await DisplayAlert("Hiba", "Minden mezõ kitöltése kötelezõ!", "OK");
			return;
		}

		try
		{
			szotarBejegyzes.OtherLanguage = OtherLangEntry.Text.Trim();
			szotarBejegyzes.Hungarian = HunLangEntry.Text.Trim();
			szotarBejegyzes.Category = CategoryEntry.Text.Trim();

			FileHandler.SaveDictionary(dicName);
			await Navigation.PopModalAsync(); // Bezárás és visszalépés
		}
		catch (Exception ex)
		{
			Debug.Print(ex.Message, ex);
			await DisplayAlert("Hiba", $"Nem sikerült menteni: {ex.Message}", "OK");
		}
	}

}
