

using Lexikope.Mmodel;
using Microsoft.Maui.Controls;
using System;

namespace Lexikope;

public partial class QuizFrm : ContentPage
{
	Random random = new Random();
	Picker langPicker;
	private Quiz quiz;
	private int sourceLangIndex;
	private int destinationLangIndex;
	private int questionCount;
	private int goodResponseCount;
	// Gombokat tartalmazó StackLayout
	private VerticalStackLayout buttonStack = new VerticalStackLayout { Spacing = 5 };

	public QuizFrm(Picker picker)
	{
		InitializeComponent();
		langPicker = picker;
		DisplayShow();

		NewRound();
	}


	private void DisplayShow()
	{
		// Fõ Grid, amely két sorra osztja az elrendezést
		var grid = new Grid();

		// Az elsõ sor kitölti a maradék helyet
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

		// Az Exit gombnak fix méretû sort adunk
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

		// StackLayout a fõ tartalomhoz (Eredmény, Kérdés, Gombok)
		var mainStack = new VerticalStackLayout
		{
			Spacing = 10,
			Padding = new Thickness(20)
		};

		// Eredmény Label létrehozása
		Score = new Label
		{
			Text = "Kérdés : 0 ,	Jó válasz : 0",
			FontSize = 18,
			TextColor = Colors.White,
			VerticalOptions = LayoutOptions.Center
		};

		// Kérdés kerete (Frame)
		var frame = new Frame
		{
			BackgroundColor = Colors.Beige,
			CornerRadius = 10,
			Padding = 10,
			HasShadow = true
		};

		// Kérdés szövege (Label a Frame-ben)
		AskWorld = new Label
		{
			Text = "Kérdés szövege",
			FontSize = 20,
			TextColor = Colors.Blue,
			FontAttributes = FontAttributes.Bold,
			HorizontalOptions = LayoutOptions.Fill,
			HorizontalTextAlignment = TextAlignment.Center,
			VerticalOptions = LayoutOptions.Center
		};

		// Frame tartalmának beállítása
		frame.Content = AskWorld;

		// Spacer Label, hogy nagyobb hely legyen a kérdés után
		var spacer = new Label
		{
			HeightRequest = 20 // 50 pixel szünet a kérdés és a gombok között
		};

		for (int i = 1; i <= 6; i++)
		{
			var buttonFrame = new Frame
			{
				BackgroundColor = Colors.Black,
				CornerRadius = 15,
				Padding = 10,
				HasShadow = true
			};

			var labelButton = new Label
			{
				BackgroundColor = Colors.LightGray,
				TextColor = Colors.Black,
				FontSize = 22,
				Text = $"Gomb {i} - Ez egy hosszabb szöveg, amely kétsoros is lehet",
				LineBreakMode = LineBreakMode.WordWrap, // Többsoros szöveg engedélyezése
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Center,
				Padding = new Thickness(10), // Jobb megjelenés érdekében
				AutomationId = $"choice_{i}",
				HorizontalTextAlignment = TextAlignment.Center, // Vízszintes középre igazítás
				VerticalTextAlignment = TextAlignment.Center // Függõleges középre igazítás
			};

			// Kattintási eseménykezelõ
			var tapGesture = new TapGestureRecognizer();
			tapGesture.Tapped += (s, e) => OnChoice(labelButton, EventArgs.Empty);
			labelButton.GestureRecognizers.Add(tapGesture);

			buttonFrame.Content = labelButton;
			buttonStack.Children.Add(buttonFrame);
		}


		// Exit gomb létrehozása
		var exitButton = new Button
		{
			Text = "Exit",
			TextColor = Colors.Black,
			BackgroundColor = Colors.Beige,
			FontSize = 20,
			Margin = new Thickness(0, 20, 0, 0) // 20 pixel szünet az elõzõ elem és az Exit között
		};

		exitButton.Clicked += OnExitClick;

		// Hozzáadjuk az UI elemeket a fõ StackLayout-hoz
		mainStack.Children.Add(Score);
		mainStack.Children.Add(frame);
		mainStack.Children.Add(spacer); // Szünet a kérdés után
		mainStack.Children.Add(buttonStack);

		// A fõ tartalmat az elsõ Grid sorba tesszük
		grid.Children.Add(mainStack);
		Grid.SetRow(mainStack, 0);

		// Az Exit gombot az alsó Grid sorba tesszük
		grid.Children.Add(exitButton);
		Grid.SetRow(exitButton, 1);

		// Az oldal tartalmát beállítjuk a Grid-re
		Content = grid;
	}


	private async void OnChoice(object sender, EventArgs e)
	{
		string goodAnswer = sourceLangIndex == 0 ?  quiz.Question.Hungarian : quiz.Question.OtherLanguage;

		if (sender is Label button)//(sender is Button button)
		{
			if (quiz.SuccessfulResponse(button.Text))
			{ //jóválasz esetén
				button.Background = Colors.Green;

				await Speaker.Speech(quiz.Question.OtherLanguage, langPicker.Items[0].ToString());
				
				goodResponseCount++;
			}
			else
			{//rossz válasz esetén
				button.Background = Colors.Red;
			
				AskWorld.Background = Colors.LightGreen;
				SetButtonColor(Colors.LightGreen,goodAnswer);

				await Speaker.Speech(quiz.Question.OtherLanguage, langPicker.Items[0].ToString());
			}
			SetButtonColor();
		}
		Score.Text= $"Kérdés : {questionCount} ,	Jó válasz : {goodResponseCount}";
		NewRound();
	}

	private void SetButtonColor()
	{
		int index = 1;

		foreach (var child in buttonStack.Children) // Végigmegyünk a stackben lévõ elemeken
		{
			if (child is Frame frame && frame.Content is Label labelButton) // Ellenõrizzük, hogy a Frame-ben van-e Button
			{
				if (labelButton.AutomationId == $"choice_{index}") // Ha az AutomationId egyezik
				{
					labelButton.Background = Colors.LightGray;
				}
			}
			index++;
		}
		AskWorld.Background = Colors.Beige;
	}

	private void SetButtonColor(Color color,string searchedText)
	{
		int index = 1;

		foreach (var child in buttonStack.Children) // Végigmegyünk a stackben lévõ elemeken
		{
			if (child is Frame frame && frame.Content is Label labelButton) // Ellenõrizzük, hogy a Frame-ben van-e Button
			{
				if (labelButton.Text == searchedText) // Ha a felirat eggyezik a searchedText-el
				{
					labelButton.Background = color;
				}
			}
			index++;
		}
		AskWorld.Background = Colors.Beige;
	}

	private async void OnExitClick(object sender, EventArgs e)
	{
		bool answer = await DisplayAlert("Kilépés", "Biztosan kilépsz?", "Igen", "Nem");
		if (answer)
		{
			await Navigation.PopModalAsync();//Bezár és visszatér a hívó lapra
		}
	}

	private void UpdateButtonText(int index, string newText)
	{
		foreach (var child in buttonStack.Children) // Végigmegyünk a stackben lévõ elemeken
		{
			if (child is Frame frame && frame.Content is Label labelButton) // Ellenõrizzük, hogy a Frame-ben van-e Button
			{
				if (labelButton.AutomationId == $"choice_{index}") // Ha az AutomationId egyezik
				{
					labelButton.Text = newText; // Gomb szövegének frissítése
					break; // Nem kell tovább keresni
				}
			}
		}
	}

	private void NewRound()
	{
		quiz = new Quiz();
		quiz.NewRound();

		//Nyelv kiválasztása/beállítása (Random)
		SelectLanguage();

		//Kérdés beírása a megfelelõ nyelven
		AskWorld.Text =sourceLangIndex==0 ?  quiz.Question.OtherLanguage : quiz.Question.Hungarian;

		//Jó válasz elhelyezése
		int joValaszIndex = random.Next(1, 6);
		var joValasz= destinationLangIndex == 0 ? quiz.Question.OtherLanguage : quiz.Question.Hungarian;
		UpdateButtonText(joValaszIndex, joValasz);

		//Rossz válaszok elhelyezése
		int arrayIndex = 0;
		for (int gombIndex = 1; gombIndex < 7; gombIndex++)
		{
			if (gombIndex != joValaszIndex)
			{
				var rosszValasz = destinationLangIndex == 0 ? quiz.WrongAnswers[arrayIndex].OtherLanguage : quiz.WrongAnswers[arrayIndex].Hungarian;
				UpdateButtonText(gombIndex, rosszValasz);
				arrayIndex++;
			}
		}
		questionCount++;
		Score.Text = $"Kérdés : {questionCount} ,	Jó válasz : {goodResponseCount}";
	}

	private void SelectLanguage()
	{
		sourceLangIndex = random.Next(0, 2);
		destinationLangIndex = sourceLangIndex ^ 1;
	}
}




