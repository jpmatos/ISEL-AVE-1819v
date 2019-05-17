using System;
using Clima;
using NUnit.Framework;

namespace Csvier.Test
{
    [TestFixture]
    public class CsvParserFuncTest
    {
        [Test]
        public void CleanCvsExampleTest()
        {
            const string sampleWeatherInLisbonFiltered =
                "2019-01-01,24,17,63,6,10,74,ENE,116,http://cdn.worldweatheronline.net/images/wsymbols01_png_64/wsymbol_0002_sunny_intervals.png,Partly cloudy,0.0,59,10,1031,43,14,57,6,43,13,56,11,17,13,56\n" +
                "2019-01-02,24,18,64,6,9,179,S,116,http://cdn.worldweatheronline.net/images/wsymbols01_png_64/wsymbol_0002_sunny_intervals.png,Partly cloudy,0.0,57,10,1030,15,14,57,6,42,13,56,11,17,13,56\n" +
                "2019-01-03,24,16,60,7,11,89,E,113,http://cdn.worldweatheronline.net/images/wsymbols01_png_64/wsymbol_0001_sunny.png,Sunny,0.0,67,10,1026,3,13,55,7,45,12,54,11,18,12,54\n" +
                "2019-01-04,24,16,60,9,15,78,ENE,116,http://cdn.worldweatheronline.net/images/wsymbols01_png_64/wsymbol_0002_sunny_intervals.png,Partly cloudy,0.1,73,10,1028,27,14,57,9,48,13,55,14,23,13,55";

            CsvParser<WeatherInfo> pastWeather = new CsvParser<WeatherInfo>();
//                .CtorArg("date", 0)
//                .CtorArg("tempC", 2)
//                .PropArg("PrecipMM", 11)
//                .PropArg("Desc", 10);
            WeatherInfo[] items = pastWeather
                .Load(sampleWeatherInLisbonFiltered)
                .Parse((str) =>
                {
                    string[] values = str.Split(pastWeather.separator);
                    WeatherInfo res = new WeatherInfo(DateTime.Parse(values[0]), Int32.Parse(values[2]))
                    {
                        PrecipMM = Double.Parse(values[11]), Desc = values[10]
                    };
                    return res;
                });

            WeatherInfo[] expected = {
                new WeatherInfo(new DateTime(2019, 1, 1), 17),
                new WeatherInfo(new DateTime(2019, 1, 2), 18),
                new WeatherInfo(new DateTime(2019, 1, 3), 16),
                new WeatherInfo(new DateTime(2019, 1, 4), 16)
            };
            expected[0].PrecipMM = 0.0;
            expected[1].PrecipMM = 0.0;
            expected[2].PrecipMM = 0.0;
            expected[3].PrecipMM = 0.1;
            expected[0].Desc = "Partly cloudy";
            expected[1].Desc = "Partly cloudy";
            expected[2].Desc = "Sunny";
            expected[3].Desc = "Partly cloudy";

            int i = 0;
            foreach (WeatherInfo item in items)
            {
                Assert.AreEqual(expected[i].Date, item.Date);
                Assert.AreEqual(expected[i].TempC, item.TempC);
                Assert.AreEqual(expected[i].PrecipMM, item.PrecipMM);
                Assert.AreEqual(expected[i].Desc, item.Desc);
                i++;
            }
        }
    }
}