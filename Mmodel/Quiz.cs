using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexikope.Mmodel
{
	internal class Quiz
	{
		static Random rnd = new Random();
		List<SzotarBejegyzes> questionList;
		public SzotarBejegyzes Question { get; private set; }
		public SzotarBejegyzes[] WrongAnswers { get; private set; }

		public Quiz()
		{
			 WrongAnswers = new SzotarBejegyzes[5];
			questionList = TempListLoad();
			NewRound();
		}

		/// <summary>
		/// Átmeneti listába beteszük a kategoriának megfelelő szavakat amikből majd sorsolunk 
		/// </summary>
		/// <returns></returns>
		private List<SzotarBejegyzes> TempListLoad()
		{
			return Szotar.Bejegyzesek
					.Where(x => AppState.SelectedCategory == "Minden kategória" || x.Category == AppState.SelectedCategory)
					.ToList();
		}

		public int GetRandomizeNumber(int min, int max)
		{
			return rnd.Next(min, max + 1);
		}



		private SzotarBejegyzes[] GetWrongAnswer(int questionIndex)
		{
			
			int index;
			for (int i = 0; i < 5; i++)
			{
				do
				{
					index = GetRandomizeNumber(0, questionList.Count - 1);
				}
				while (index == questionIndex);
				WrongAnswers[i] = questionList[index];
			}
			return WrongAnswers;
		}

		public void NewRound()
		{
			//Választ egy kérdést
			var questionIndex = GetRandomizeNumber(0, questionList.Count - 1);
			var qItem = questionList[questionIndex];
			Question = qItem;

			//Választ rossz válasz lehetőségeket
			WrongAnswers = GetWrongAnswer(questionIndex);


		}

		public bool SuccessfulResponse(string userAsnwer)
		{
			if (userAsnwer == Question.OtherLanguage || userAsnwer == Question.Hungarian)
			{
				return true;
			}
			return false;
		}

	}
}
