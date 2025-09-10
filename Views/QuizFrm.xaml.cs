

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
	// Gombokat tartalmaz� StackLayout
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
		// F� Grid, amely k�t sorra osztja az elrendez�st
		var grid = new Grid();

		// Az els� sor kit�lti a marad�k helyet
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

		// Az Exit gombnak fix m�ret� sort adunk
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

		// StackLayout a f� tartalomhoz (Eredm�ny, K�rd�s, Gombok)
		var mainStack = new VerticalStackLayout
		{
			Spacing = 10,
			Padding = new Thickness(20)
		};

		// Eredm�ny Label l�trehoz�sa
		Score = new Label
		{
			Text = "K�rd�s : 0 ,	J� v�lasz : 0",
			FontSize = 18,
			TextColor = Colors.White,
			VerticalOptions = LayoutOptions.Center
		};

		// K�rd�s kerete (Frame)
		var frame = new Frame
		{
			BackgroundColor = Colors.Beige,
			CornerRadius = 10,
			Padding = 10,
			HasShadow = true
		};

		// K�rd�s sz�vege (Label a Frame-ben)
		AskWorld = new Label
		{
			Text = "K�rd�s sz�vege",
			FontSize = 20,
			TextColor = Colors.Blue,
			FontAttributes = FontAttributes.Bold,
			HorizontalOptions = LayoutOptions.Fill,
			HorizontalTextAlignment = TextAlignment.Center,
			VerticalOptions = LayoutOptions.Center
		};

		// Frame tartalm�nak be�ll�t�sa
		frame.Content = AskWorld;

		// Spacer Label, hogy nagyobb hely legyen a k�rd�s ut�n
		var spacer = new Label
		{
			HeightRequest = 20 // 50 pixel sz�net a k�rd�s �s a gombok k�z�tt
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
				Text = $"Gomb {i} - Ez egy hosszabb sz�veg, amely k�tsoros is lehet",
				LineBreakMode = LineBreakMode.WordWrap, // T�bbsoros sz�veg enged�lyez�se
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Center,
				Padding = new Thickness(10), // Jobb megjelen�s �rdek�ben
				AutomationId = $"choice_{i}",
				HorizontalTextAlignment = TextAlignment.Center, // V�zszintes k�z�pre igaz�t�s
				VerticalTextAlignment = TextAlignment.Center // F�gg�leges k�z�pre igaz�t�s
			};

			// Kattint�si esem�nykezel�
			var tapGesture = new TapGestureRecognizer();
			tapGesture.Tapped += (s, e) => OnChoice(labelButton, EventArgs.Empty);
			labelButton.GestureRecognizers.Add(tapGesture);

			buttonFrame.Content = labelButton;
			buttonStack.Children.Add(buttonFrame);
		}


		// Exit gomb l�trehoz�sa
		var exitButton = new Button
		{
			Text = "Exit",
			TextColor = Colors.Black,
			BackgroundColor = Colors.Beige,
			FontSize = 20,
			Margin = new Thickness(0, 20, 0, 0) // 20 pixel sz�net az el�z� elem �s az Exit k�z�tt
		};

		exitButton.Clicked += OnExitClick;

		// Hozz�adjuk az UI elemeket a f� StackLayout-hoz
		mainStack.Children.Add(Score);
		mainStack.Children.Add(frame);
		mainStack.Children.Add(spacer); // Sz�net a k�rd�s ut�n
		mainStack.Children.Add(buttonStack);

		// A f� tartalmat az els� Grid sorba tessz�k
		grid.Children.Add(mainStack);
		Grid.SetRow(mainStack, 0);

		// Az Exit gombot az als� Grid sorba tessz�k
		grid.Children.Add(exitButton);
		Grid.SetRow(exitButton, 1);

		// Az oldal tartalm�t be�ll�tjuk a Grid-re
		Content = grid;
	}


	private async void OnChoice(object sender, EventArgs e)
	{
		string goodAnswer = sourceLangIndex == 0 ?  quiz.Question.Hungarian : quiz.Question.OtherLanguage;

		if (sender is Label button)//(sender is Button button)
		{
			if (quiz.SuccessfulResponse(button.Text))
			{ //j�v�lasz eset�n
				button.Background = Colors.Green;

				await Speaker.Speech(quiz.Question.OtherLanguage, langPicker.Items[0].ToString());
				
				goodResponseCount++;
			}
			else
			{//rossz v�lasz eset�n
				button.Background = Colors.Red;
			
				AskWorld.Background = Colors.LightGreen;
				SetButtonColor(Colors.LightGreen,goodAnswer);

				await Speaker.Speech(quiz.Question.OtherLanguage, langPicker.Items[0].ToString());
			}
			SetButtonColor();
		}
		Score.Text= $"K�rd�s : {questionCount} ,	J� v�lasz : {goodResponseCount}";
		NewRound();
	}

	private void SetButtonColor()
	{
		int index = 1;

		foreach (var child in buttonStack.Children) // V�gigmegy�nk a stackben l�v� elemeken
		{
			if (child is Frame frame && frame.Content is Label labelButton) // Ellen�rizz�k, hogy a Frame-ben van-e Button
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

		foreach (var child in buttonStack.Children) // V�gigmegy�nk a stackben l�v� elemeken
		{
			if (child is Frame frame && frame.Content is Label labelButton) // Ellen�rizz�k, hogy a Frame-ben van-e Button
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
		bool answer = await DisplayAlert("Kil�p�s", "Biztosan kil�psz?", "Igen", "Nem");
		if (answer)
		{
			await Navigation.PopModalAsync();//Bez�r �s visszat�r a h�v� lapra
		}
	}

	private void UpdateButtonText(int index, string newText)
	{
		foreach (var child in buttonStack.Children) // V�gigmegy�nk a stackben l�v� elemeken
		{
			if (child is Frame frame && frame.Content is Label labelButton) // Ellen�rizz�k, hogy a Frame-ben van-e Button
			{
				if (labelButton.AutomationId == $"choice_{index}") // Ha az AutomationId egyezik
				{
					labelButton.Text = newText; // Gomb sz�veg�nek friss�t�se
					break; // Nem kell tov�bb keresni
				}
			}
		}
	}

	private void NewRound()
	{
		quiz = new Quiz();
		quiz.NewRound();

		//Nyelv kiv�laszt�sa/be�ll�t�sa (Random)
		SelectLanguage();

		//K�rd�s be�r�sa a megfelel� nyelven
		AskWorld.Text =sourceLangIndex==0 ?  quiz.Question.OtherLanguage : quiz.Question.Hungarian;

		//J� v�lasz elhelyez�se
		int joValaszIndex = random.Next(1, 6);
		var joValasz= destinationLangIndex == 0 ? quiz.Question.OtherLanguage : quiz.Question.Hungarian;
		UpdateButtonText(joValaszIndex, joValasz);

		//Rossz v�laszok elhelyez�se
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
		Score.Text = $"K�rd�s : {questionCount} ,	J� v�lasz : {goodResponseCount}";
	}

	private void SelectLanguage()
	{
		sourceLangIndex = random.Next(0, 2);
		destinationLangIndex = sourceLangIndex ^ 1;
	}
}




