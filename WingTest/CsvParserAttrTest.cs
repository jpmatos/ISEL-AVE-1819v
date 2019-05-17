using System;
using Clima;
using NUnit.Framework;

namespace Csvier.Test
{
    [TestFixture]
    public class CsvParserAttrTest
    {
        [Test]
        public void CleanCvsExampleTest()
        {
            const string sampleWeatherInLisbonFiltered =
                "2019-01-01,24,17,63,6,10,74,ENE,116,http://cdn.worldweatheronline.net/images/wsymbols01_png_64/wsymbol_0002_sunny_intervals.png,Partly cloudy,0.0,59,10,1031,43,14,57,6,43,13,56,11,17,13,56\n" +
                "2019-01-02,24,18,64,6,9,179,S,116,http://cdn.worldweatheronline.net/images/wsymbols01_png_64/wsymbol_0002_sunny_intervals.png,Partly cloudy,0.0,57,10,1030,15,14,57,6,42,13,56,11,17,13,56\n" +
                "2019-01-03,24,16,60,7,11,89,E,113,http://cdn.worldweatheronline.net/images/wsymbols01_png_64/wsymbol_0001_sunny.png,Sunny,0.0,67,10,1026,3,13,55,7,45,12,54,11,18,12,54\n" +
                "2019-01-04,24,16,60,9,15,78,ENE,116,http://cdn.worldweatheronline.net/images/wsymbols01_png_64/wsymbol_0002_sunny_intervals.png,Partly cloudy,0.1,73,10,1028,27,14,57,9,48,13,55,14,23,13,55";

            CsvParserAttr<WeatherInfo> pastWeather = (CsvParserAttr<WeatherInfo>) new CsvParserAttr<WeatherInfo>( "Ctor3")
                .CtorArg("date", 0)
                .CtorArg("tempC", 2)
                .PropArg("PrecipMM", 11)
                .PropArg("Desc", 10);
            WeatherInfo[] items = pastWeather
                .Load(sampleWeatherInLisbonFiltered)
                .Parse();

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

        [Test]
        public void CvsQueryText()
        {
            const string sampleWeather =
                "#The CSV format is in following way:-\n" +
                "#The day information is available in following format:-\n" +
                "#date,maxtempC,maxtempF,mintempC,mintempF,sunrise,sunset,moonrise,moonset,moon_phase,moon_illumination\n" +
                "\n" +
                "#Hourly information follows below the day in the following way:-\n" +
                "#date,time,tempC,tempF,windspeedMiles,windspeedKmph,winddirdegree,winddir16point,weatherCode,weatherIconUrl,weatherDesc,precipMM,humidity,visibilityKm,pressureMB,cloudcover,HeatIndexC,HeatIndexF,DewPointC,DewPointF,WindChillC,WindChillF,WindGustMiles,WindGustKmph,FeelsLikeC,FeelsLikeF\n" +
                "\n" +
                "Not Available\n" +
                "2019-01-01,17,63,12,54,07:45 AM,05:26 PM,03:18 AM,02:23 PM,Waning Crescent,34\n" +
                "2019-01-01,24,17,63,6,10,74,ENE,116,http://cdn.worldweatheronline.net/images/wsymbols01_png_64/wsymbol_0002_sunny_intervals.png,Partly cloudy,0.0,59,10,1031,43,14,57,6,43,13,56,11,17,13,56\n" +
                "2019-01-02,18,64,12,53,07:45 AM,05:27 PM,04:19 AM,02:59 PM,Waning Crescent,28\n" +
                "2019-01-02,24,18,64,6,9,179,S,116,http://cdn.worldweatheronline.net/images/wsymbols01_png_64/wsymbol_0002_sunny_intervals.png,Partly cloudy,0.0,57,10,1030,15,14,57,6,42,13,56,11,17,13,56\n" +
                "2019-01-03,16,60,11,52,07:46 AM,05:27 PM,05:19 AM,03:39 PM,Waning Crescent,21\n" +
                "2019-01-03,24,16,60,7,11,89,E,113,http://cdn.worldweatheronline.net/images/wsymbols01_png_64/wsymbol_0001_sunny.png,Sunny,0.0,67,10,1026,3,13,55,7,45,12,54,11,18,12,54\n" +
                "2019-01-04,16,60,13,55,07:46 AM,05:28 PM,06:16 AM,04:22 PM,Waning Crescent,14\n" +
                "2019-01-04,24,16,60,9,15,78,ENE,116,http://cdn.worldweatheronline.net/images/wsymbols01_png_64/wsymbol_0002_sunny_intervals.png,Partly cloudy,0.1,73,10,1028,27,14,57,9,48,13,55,14,23,13,55\n" +
                "2019-01-05,16,61,11,52,07:46 AM,05:29 PM,07:10 AM,05:10 PM,Waning Crescent,7\n" +
                "2019-01-05,24,16,61,8,13,70,ENE,113,http://cdn.worldweatheronline.net/images/wsymbols01_png_64/wsymbol_0001_sunny.png,Sunny,0.0,64,10,1032,0,13,55,6,43,12,53,13,21,12,53\n";
            CsvParserAttr<WeatherInfo> pastWeather = (CsvParserAttr<WeatherInfo>) new CsvParserAttr<WeatherInfo>("Ctor3").Load(sampleWeather);

            pastWeather.RemoveWith("#");
            Assert.AreEqual("",
                pastWeather.arr[0]);

            pastWeather.RemoveEmpties();
            Assert.AreEqual("Not Available",
                pastWeather.arr[0]);

            pastWeather.Remove(1);
            Assert.AreEqual("2019-01-01,17,63,12,54,07:45 AM,05:26 PM,03:18 AM,02:23 PM,Waning Crescent,34",
                pastWeather.arr[0]);

            pastWeather.RemoveEvenIndexes();
            Assert.AreEqual(
                "2019-01-02,24,18,64,6,9,179,S,116,http://cdn.worldweatheronline.net/images/wsymbols01_png_64/wsymbol_0002_sunny_intervals.png,Partly cloudy,0.0,57,10,1030,15,14,57,6,42,13,56,11,17,13,56",
                pastWeather.arr[1]);

            pastWeather.RemoveOddIndexes();
            Assert.AreEqual(
                "2019-01-03,24,16,60,7,11,89,E,113,http://cdn.worldweatheronline.net/images/wsymbols01_png_64/wsymbol_0001_sunny.png,Sunny,0.0,67,10,1026,3,13,55,7,45,12,54,11,18,12,54",
                pastWeather.arr[1]);
        }
    }
}