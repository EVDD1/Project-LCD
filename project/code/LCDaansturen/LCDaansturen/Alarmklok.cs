using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCDaansturen
{
    internal class Alarmklok
    {
		private DateTime alarmtime;

		public DateTime Alarmtime
		{
			get { return alarmtime; }
			set { alarmtime = value; }
		}

		public void startalarm(DateTime time)
		{
			alarmtime = time;
		}

		public bool IsAlarmTijdKlaar()
		{
		
			int tijd = DateTime.Compare(DateTime.Now, alarmtime);
			return tijd > 0;
		}
    }
}
