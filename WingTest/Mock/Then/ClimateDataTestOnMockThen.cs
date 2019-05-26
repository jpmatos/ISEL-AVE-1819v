using System;
using System.Collections.Generic;
using System.Linq;
using ClimateData;
using Mocky;
using NUnit.Framework;
using IHttpRequest = Request.IHttpRequest;

namespace Csvier.Test
{
    [TestFixture]
    public class ClimateDataTestOnMockThen
    {
        [Test]
        public void TestLoadYearlyAverageInPortugalOnMock()
        {
            Dictionary<object[], TemperatureYearInfo[]> dic = new Dictionary<object[], TemperatureYearInfo[]>();
            dic.Add(
                new object[]{1980, 1999, "PRT"},
                new TemperatureYearInfo[]
                {
                    new TemperatureYearInfo("", 0, 0, 0),
                    new TemperatureYearInfo("", 0, 0, 0),
                    new TemperatureYearInfo("", 0, 0, 0),
                    new TemperatureYearInfo("", 0, 0, 19.02503749302857),
                    new TemperatureYearInfo("", 0, 0, 0),
                    new TemperatureYearInfo("", 0, 0, 0),
                    new TemperatureYearInfo("ingv_echam4", 0, 0, 0),
                    new TemperatureYearInfo("", 0, 0, 0),
                    new TemperatureYearInfo("", 0, 0, 0),
                    new TemperatureYearInfo("", 0, 0, 0),
                    new TemperatureYearInfo("", 0, 0, 0),
                    new TemperatureYearInfo("", 0, 0, 0),
                    new TemperatureYearInfo("", 0, 0, 0),
                    new TemperatureYearInfo("", 0, 0, 0),
                    new TemperatureYearInfo("", 1980, 0, 0),
                }
            );
            
            Mocker mocker = new Mocker(typeof(IClimateDataWebApi));
            mocker
                .When("PastAverageAnualTemperature")
                .Then<int, int, string, TemperatureYearInfo[]>((from, to, country) =>
                {
                    object[] key = {from, to, country};
                    object[] foundKey = dic.Keys.FirstOrDefault(curr => curr.SequenceEqual(key));
                    bool hasFound = dic.TryGetValue(foundKey, out TemperatureYearInfo[] res);
                    
                    if(!hasFound) throw new KeyNotFoundException();
                    return res;
                });
            
            using(IClimateDataWebApi api = (IClimateDataWebApi)mocker.Create())
            {
                TemperatureYearInfo[] temps = api.PastAverageAnualTemperature(1980, 1999, "PRT");
                Assert.AreEqual(19.02503749302857, temps[3].Temperature);
                Assert.AreEqual("ingv_echam4", temps[6].GCM);
                Assert.AreEqual(1980, temps[14].FromYear);
            }
        }
        
        [Test]
        public void TestLoadYearlyAverageInPortugalOnRequestMockThen()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add(
                "http://climatedataapi.worldbank.org/climateweb/rest/v1/country/annualavg/tas/1980/1999/PRT.csv", 
                "GCM,var,from_year,to_year,annual\n" +
                "bccr_bcm2_0,tas,1980,1999,15.830819266192856\n" +
                "cccma_cgcm3_1,tas,1980,1999,15.970897129614285\n" +
                "cnrm_cm3,tas,1980,1999,15.148875645235716\n" +
                "csiro_mk3_5,tas,1980,1999,19.02503749302857\n" +
                "gfdl_cm2_0,tas,1980,1999,14.149026053296426\n" +
                "gfdl_cm2_1,tas,1980,1999,15.634562901092854\n" +
                "ingv_echam4,tas,1980,1999,18.105167933878572\n" +
                "inmcm3_0,tas,1980,1999,17.06181117466428\n" +
                "ipsl_cm4,tas,1980,1999,14.43794686454286\n" +
                "miroc3_2_medres,tas,1980,1999,16.571744646349995\n" +
                "miub_echo_g,tas,1980,1999,16.36417497907857\n" +
                "mpi_echam5,tas,1980,1999,16.028985159728574\n" +
                "mri_cgcm2_3_2a,tas,1980,1999,15.393576485771428\n" +
                "ukmo_hadcm3,tas,1980,1999,15.511228288926429\n" +
                "ukmo_hadgem1,tas,1980,1999,15.902805873342858\n" +
                ""
                );
            
            Mocker mocker = new Mocker(typeof(IHttpRequest));
            mocker
                .When("GetBody")
                .Then<string, string>(url =>
                {
                    dic.TryGetValue(url, out string value);
                    return value;
                });
            IHttpRequest req = (IHttpRequest)mocker.Create();

            using (IClimateDataWebApi api = new ClimateDataWebApi(req))
            {
                TemperatureYearInfo[] temps = api.PastAverageAnualTemperature(1980, 1999, "PRT");
                Assert.AreEqual(19.02503749302857, temps[3].Temperature);
                Assert.AreEqual("ingv_echam4", temps[6].GCM);
                Assert.AreEqual(1980, temps[14].FromYear);
            }
        }
    }
}