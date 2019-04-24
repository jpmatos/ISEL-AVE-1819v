using System;

namespace ClimateData
{
    public interface IClimateDataWebApi : IDisposable
    {
        TemperatureYearInfo[] PastAverageAnualTemperature(int from, int to, string country);
    }
}