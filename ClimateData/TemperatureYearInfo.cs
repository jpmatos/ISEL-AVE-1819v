namespace ClimateData
{
    public class TemperatureYearInfo
    {
        public string GCM { get; }
        public int FromYear { get; }
        public int ToYear { get; }
        public double Temperature { get; set; }

        public TemperatureYearInfo(string GCM, int FromYear, int ToYear)
        {
            this.GCM = GCM;
            this.FromYear = FromYear;
            this.ToYear = ToYear;
        }

        public TemperatureYearInfo(string GCM, int FromYear, int ToYear, double Temperature)
        {
            this.GCM = GCM;
            this.FromYear = FromYear;
            this.ToYear = ToYear;
            this.Temperature = Temperature;
        }
    }
}