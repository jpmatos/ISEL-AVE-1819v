using Csvier;
using Request;
using System;
using System.Globalization;

namespace Clima
{
    public class WeatherWebApi : IWeatherWebApi
    {
        const string WEATHER_KEY = "2623ee8a52024a6180f02056192605";
        const string WEATHER_HOST = "http://api.worldweatheronline.com/premium/v1/";

        const string PATH_WEATHER = WEATHER_HOST +
                                    "past-weather.ashx?q={0},{1}&date={2}&enddate={3}&tp=24&format=csv&key=" +
                                    WEATHER_KEY;

        const string SEARCH = WEATHER_HOST + "search.ashx?query={0}&format=tab&key=" + WEATHER_KEY;

        readonly CsvParser<WeatherInfo> pastWeather;    //TODO why is this here
        readonly CsvParser<LocationInfo> locations;
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

            CsvParser<WeatherInfo> weatherParser = (CsvParser<WeatherInfo>) new CsvParser<WeatherInfo>(',')
                .CtorArg("date", 0)
                .CtorArg("tempC", 1)
                .PropArg("Desc", 9)
                .PropArg("PrecipMM", 10);
            
            WeatherInfo[] items = weatherParser
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

            CsvParser<LocationInfo> locationsParser = (CsvParser<LocationInfo>) new CsvParser<LocationInfo>('\t')
                .CtorArg("country", 1)
                .CtorArg("region", 2)
                .CtorArg("latitude", 3)
                .CtorArg("longitude", 4);
            LocationInfo[] items = locationsParser
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