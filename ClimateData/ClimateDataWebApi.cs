using System;
using Csvier;
using Request;

namespace ClimateData
{
    public class ClimateDataWebApi : IClimateDataWebApi
    {
        private const string CLIMATEDATA_HOST = "http://climatedataapi.worldbank.org/climateweb/rest/v1/country/";
        private const string EXTENSION = ".csv";
        /*
         * {0} - Time Period - Values: mavg; annualavg; manom; annualanom.
         * {1} - Precipitation or Temperature - Values: pr; tas.
         * {2} - Year Start - Values: [1920, 1940, 1960, 1980, 2020, 2040, 2060, 2080]
         * {3} - Year End - Values: [1939, 1959, 1979, 1999, 2039, 2059, 2079, 2099]
         * {4} - ISO3 Country Code - eg. PRT
         * Year Start and Year End must be on the same index
         */
        private const string PAST_CLIMATEDATA = CLIMATEDATA_HOST +
                                                "{0}/{1}/{2}/{3}/{4}" +
                                                EXTENSION;
        
        readonly IHttpRequest req;

        public ClimateDataWebApi() : this(new HttpRequest())
        {
        }

        public ClimateDataWebApi(IHttpRequest req)
        {
            this.req = req;
        }
        
        public void Dispose()
        {
            req.Dispose();
        }

        public TemperatureYearInfo[] PastAverageAnualTemperature(int from, int to, string country){
            
            string searchQuery = string.Format(PAST_CLIMATEDATA, "annualavg", "tas", from.ToString(), to.ToString(), country);
            string searchResult = req.GetBody(searchQuery);

            CsvParser<TemperatureYearInfo> climateParser = (CsvParser<TemperatureYearInfo>) new CsvParser<TemperatureYearInfo>(',')
                .CtorArg("GCM", 0)
                .CtorArg("FromYear", 2)
                .CtorArg("ToYear", 3)
                .PropArg("Temperature", 4);

            TemperatureYearInfo[] items = climateParser
                .Load(searchResult)
                .Remove(1)
                .RemoveEmpties()
                .Parse();
            
            TemperatureYearInfo[] res = new TemperatureYearInfo[items.Length];
            Array.Copy(items, res, items.Length);

            return res;
        }
    }
}