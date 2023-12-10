using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
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

		public void Startalarm(DateTime time)
		{
			alarmtime = time;
		}

	
		public bool IsAlarmTijdKlaar()
		{

			//vergelijkt de huidige tijd met de alarm tijd
			int tijd = DateTime.Compare(DateTime.Now, alarmtime);
			
			//kijkt of de huidige tijd later is dan de alarmtijd
			//en dan returned het true
			return tijd > 0;
        
        }
    }
}
