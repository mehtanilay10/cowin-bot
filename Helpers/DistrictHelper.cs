using System.Collections.Generic;
using System.IO;
using System.Linq;
using FindVaccineCenterBot.Models;
using Newtonsoft.Json;

namespace FindVaccineCenterBot.Helpers
{
    public static class DistrictHelper
    {
        public static List<int> GetDistrictCodes(string districtName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\districts.json");
            string jsongData = File.ReadAllText(filePath);
            List<DistrictDetails> districtDetails = JsonConvert.DeserializeObject<List<DistrictDetails>>(jsongData);

            return districtDetails
                .Where(x => x.district_name.Contains(districtName, System.StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.district_id)
                .ToList();
        }
    }
}
