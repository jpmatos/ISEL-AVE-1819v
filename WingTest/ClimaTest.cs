using System;
using NUnit.Framework;
using System.Net;
using Clima;
using System.Collections.Generic;

namespace Csvier.Test
{
    [TestFixture]
    public class ClimaTest
    {
        [Test]
        public void TestLoadSearchOporto()
        {
            using (WeatherWebApi api = new WeatherWebApi())
            {
                LocationInfo[] locals = api.Search("oporto");
                Assert.AreEqual(6, locals.Length);
                Assert.AreEqual("Cuba", locals[5].Country);
            }
        }

        [Test]
        public void TestLoadPastWeatherOnJanuaryAndCheckMaximumTempC()
        {
            using (WeatherWebApi api = new WeatherWebApi())
            {
                IEnumerable<WeatherInfo> infos = api.PastWeather(37.017, -7.933, DateTime.Parse("2019-01-01"),
                    DateTime.Parse("2019-01-30"));
                int max = int.MinValue;
                foreach (WeatherInfo wi in infos)
                {
                    if (wi.TempC > max) max = wi.TempC;
                }

                Assert.AreEqual(19, max);
                // Console.WriteLine(String.Join("\n", infos));
            }
        }

        [Test]
        public void CleanCvsExampleTEst()
        {
            string sampleWeatherInLisbonFiltered =
                "2019-01-01,24,17,63,6,10,74,ENE,116,http://cdn.worldweatheronline.net/images/wsymbols01_png_64/wsymbol_0002_sunny_intervals.png,Partly cloudy,0.0,59,10,1031,43,14,57,6,43,13,56,11,17,13,56\n" +
                "2019-01-02,24,18,64,6,9,179,S,116,http://cdn.worldweatheronline.net/images/wsymbols01_png_64/wsymbol_0002_sunny_intervals.png,Partly cloudy,0.0,57,10,1030,15,14,57,6,42,13,56,11,17,13,56\n" +
                "2019-01-03,24,16,60,7,11,89,E,113,http://cdn.worldweatheronline.net/images/wsymbols01_png_64/wsymbol_0001_sunny.png,Sunny,0.0,67,10,1026,3,13,55,7,45,12,54,11,18,12,54\n" +
                "2019-01-04,24,16,60,9,15,78,ENE,116,http://cdn.worldweatheronline.net/images/wsymbols01_png_64/wsymbol_0002_sunny_intervals.png,Partly cloudy,0.1,73,10,1028,27,14,57,9,48,13,55,14,23,13,55";

            CsvParser pastWeather = new CsvParser(typeof(WeatherInfo))
                .CtorArg("date", 0)
                .CtorArg("tempC", 2);
            IEnumerable<object> items = pastWeather
                .Load(sampleWeatherInLisbonFiltered)
                .Parse();

            WeatherInfo[] expected = new WeatherInfo[]
            {
                new WeatherInfo(new DateTime(2019, 1, 1), 17),
                new WeatherInfo(new DateTime(2019, 1, 2), 18),
                new WeatherInfo(new DateTime(2019, 1, 3), 16),
                new WeatherInfo(new DateTime(2019, 1, 4), 16)
            };

            int i = 0;
            foreach (WeatherInfo item in items)
            {
                Assert.AreEqual(expected[i].Date, item.Date);
                Assert.AreEqual(expected[i].TempC, item.TempC);
                i++;
            }
        }
    }
}