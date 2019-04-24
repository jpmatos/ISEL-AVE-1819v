using Csvier;
using Request;
using System;
using System.Globalization;

namespace Clima
{
    public class WeatherWebApi : IWeatherWebApi
    {
        const string WEATHER_KEY = "005e3c30b4fb4769a1c205526191503";
        const string WEATHER_HOST = "http://api.worldweatheronline.com/premium/v1/";

        const string PATH_WEATHER = WEATHER_HOST +
                                    "past-weather.ashx?q={0},{1}&date={2}&enddate={3}&tp=24&format=csv&key=" +
                                    WEATHER_KEY;

        const string SEARCH = WEATHER_HOST + "search.ashx?query={0}&format=tab&key=" + WEATHER_KEY;

        readonly CsvParser pastWeather;    //TODO why is this here
        readonly CsvParser locations;
        readonly IHttpRequest req;

        public WeatherWebApi() : this(new HttpRequest())
        {
        }

        public WeatherWebApi(IHttpRequest req)
        {
            this.req = req;
        }

        public void Dispose()
        {
            req.Dispose();
        }

        public WeatherInfo[] PastWeather(double lat, double log, DateTime from, DateTime to)
        {
            string latStr = lat.ToString("0.000", CultureInfo.InvariantCulture);
            string logStr = log.ToString("0.000", CultureInfo.InvariantCulture);
            string fromStr = from.Date.ToString("yyyy-MM-dd");
            string toStr = to.Date.ToString("yyyy-MM-dd");

            string searchQuery = string.Format(PATH_WEATHER, latStr, logStr, fromStr, toStr);
            string searchResult = req.GetBody(searchQuery);

            CsvParser weatherParser = (CsvParser) new CsvParser(typeof(WeatherInfo), ',')
                .CtorArg("date", 0)
                .CtorArg("tempC", 2)
                .PropArg("PrecipMM", 11)
                .PropArg("Desc", 10);
            object[] items = weatherParser
                .Load(searchResult)
                .RemoveWith("#")
                .Remove(1)
                .RemoveEmpties()
                .RemoveEvenIndexes()
                .Parse();

            WeatherInfo[] res = new WeatherInfo[items.Length];
            Array.Copy(items, res, items.Length);

            return res;
        }

        public LocationInfo[] Search(string query)
        {
            string searchQuery = string.Format(SEARCH, query);
            string searchResult = req.GetBody(searchQuery);

            CsvParser locationsParser = (CsvParser) new CsvParser(typeof(LocationInfo), '\t')
                .CtorArg("country", 1)
                .CtorArg("region", 2)
                .CtorArg("latitude", 3)
                .CtorArg("longitude", 4);
            object[] items = locationsParser
                .Load(searchResult)
                .RemoveWith("#")
                .RemoveEmpties()
                .Parse();

            LocationInfo[] res = new LocationInfo[items.Length];
            Array.Copy(items, res, items.Length);

            return res;
        }
    }
}