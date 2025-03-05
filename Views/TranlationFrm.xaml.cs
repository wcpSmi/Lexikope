using Lexikope.Mmodel;
using System.Web;

namespace Lexikope;

public partial class TranlationFrm : ContentPage
{
	private bool isWebViewLoaded = false; // Ezzel tároljuk az állapotot
	private string text;
	private string fromLang;
	private string toLang;

	public TranlationFrm(string txt, string fromLanguage, string toLanguage)
	{
		text = txt;
		fromLang = fromLanguage;
		toLang = toLanguage;
		InitializeComponent();
		CheckInternetAndLoad();
	}

	private void CheckInternetAndLoad()
	{
		if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
		{
			 DisplayAlert("Hálózati hiba", "Nincs aktív internetkapcsolat!", "OK");
			return;
		}
		DoTranslate();
	}

	private void DoTranslate()
	{


		// Az URL megfelelõ formázása (a szöveg URL-kódolása)
		string encodedText = HttpUtility.UrlEncode(text);
		string googleTranslateUrl = $"https://translate.google.com/?sl={fromLang}&tl={toLang}&text={encodedText}&op=translate";

		if (!isWebViewLoaded)
		{
			// Ha még nincs betöltve, akkor betöltjük
			translateWebView.Source = googleTranslateUrl;
			isWebViewLoaded = true;
		}
		else
		{
			// Ha már betöltöttük egyszer, frissítjük az URL-t
			translateWebView.Source = googleTranslateUrl;
		}
	}
	private async void OnBack(object sender, EventArgs e)
	{
		await Navigation.PopModalAsync();//Bezár és visszatér a hívó lapra
	}
}