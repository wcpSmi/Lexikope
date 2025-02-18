using Lexikope.Mmodel;
using System.Web;

namespace Lexikope;

public partial class TranlationFrm : ContentPage
{
	private bool isWebViewLoaded = false; // Ezzel t�roljuk az �llapotot
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
			 DisplayAlert("H�l�zati hiba", "Nincs akt�v internetkapcsolat!", "OK");
			return;
		}
		DoTranslate();
	}

	private void DoTranslate()
	{


		// Az URL megfelel� form�z�sa (a sz�veg URL-k�dol�sa)
		string encodedText = HttpUtility.UrlEncode(text);
		string googleTranslateUrl = $"https://translate.google.com/?sl={fromLang}&tl={toLang}&text={encodedText}&op=translate";

		if (!isWebViewLoaded)
		{
			// Ha m�g nincs bet�ltve, akkor bet�ltj�k
			translateWebView.Source = googleTranslateUrl;
			isWebViewLoaded = true;
		}
		else
		{
			// Ha m�r bet�lt�tt�k egyszer, friss�tj�k az URL-t
			translateWebView.Source = googleTranslateUrl;
		}
	}
	private async void OnBack(object sender, EventArgs e)
	{
		await Navigation.PopModalAsync();//Bez�r �s visszat�r a h�v� lapra
	}
}