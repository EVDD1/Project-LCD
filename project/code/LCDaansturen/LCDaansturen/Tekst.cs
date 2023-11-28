using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCDaansturen
{
    internal class Tekst
    {
		private string info;

		public string Info
		{
			get { return info; }
			set { info = value.ToUpper(); }
		}

		public string toevoegen()
		{

			return $"{info}";
		}
	}
}
