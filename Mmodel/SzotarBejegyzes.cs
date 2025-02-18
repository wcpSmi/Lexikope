using Lexikope.Mmodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Lexikope
{
	public class SzotarBejegyzes
	{


		public string Hungarian {  get; set; }
		public string OtherLanguage {  get; set; }
		public string Category {  get;  set; }

		public SzotarBejegyzes(string magyar,string masNyelv, string kategoria) 
		{
			Hungarian = magyar;
			OtherLanguage = masNyelv;
			Category = kategoria;
		}

		public override string ToString()
		{
			//return base.ToString();
			return AppState.SourceLanguageIndex==0 ? OtherLanguage : Hungarian;
		}
	}
}
