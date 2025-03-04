using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexikope
{
    interface IScreenAwake
    {
		public void KeepScreenOn();
		public void AllowScreenOff();
	}
}
