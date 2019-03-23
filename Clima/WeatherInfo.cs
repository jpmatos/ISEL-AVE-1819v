using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clima
{
    public class WeatherInfo
    {
        [Description("Date")]
        public DateTime Date { get;  }
        
        [Description("TempC")]
        public int TempC { get; }
        
        [Description("PrecipMM")]
        public double PrecipMM { get; set; }
        
        [Description("Desc")]
        public String Desc { get; set; }

        [Description("Ctor1")]
        public WeatherInfo()
        {
        }

        [Description("Ctor2")]
        public WeatherInfo(DateTime date)
        {
            this.Date = date;
        }

        [Description("Ctor3")]
        public WeatherInfo(DateTime date, int tempC)
        {
            this.Date = date;
            this.TempC = tempC;
        }

        public override String ToString()
        {
            return "WeatherInfo{" +
                "date=" + Date +
                ", tempC=" + TempC +
                ", precipMM=" + PrecipMM +
                ", desc='" + Desc + '\'' +
                '}';
        }

    }
}
