using System;
using ClimateData;
using NUnit.Framework;

namespace Csvier.Test
{
    [TestFixture]
    public class ClimateDataTest
    {
        [Test]
        public void TestLoadYearlyAverageInPortugal()
        {
            using(ClimateDataWebApi api = new ClimateDataWebApi())
            {
                TemperatureYearInfo[] temps = api.PastAverageAnualTemperature(1980, 1999, "PRT");
                Assert.AreEqual(19.02503749302857, temps[3].Temperature);
                Assert.AreEqual("ingv_echam4", temps[6].GCM);
                Assert.AreEqual(1980, temps[14].FromYear);
            }
        }
    }
}