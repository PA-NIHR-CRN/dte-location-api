using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Models
{
    public static class OrdnanceSurveyCountries
    {
        public static IEnumerable<CountryModel> Countries { get; } = new List<CountryModel>
        {
            new CountryModel("ENGLAND", "e"),
            new CountryModel("SCOTLAND", "s"),
            new CountryModel("WALES", "w"),
            new CountryModel("NORTHERN IRELAND", "n"),
        };
        
        public static string GetCountryCode(string countryName)
        {
            var country = Countries.FirstOrDefault(x => string.Equals(countryName, x.CountryName, StringComparison.OrdinalIgnoreCase));

            if (country == null)
            {
                throw new Exception($"Can not find country name: {countryName} in {nameof(OrdnanceSurveyCountries)}");
            }
            
            return country.CountryCode;
        }
        
        public static string GetCountryName(string countryCode)
        {
            var country = Countries.FirstOrDefault(x => string.Equals(countryCode, x.CountryCode, StringComparison.OrdinalIgnoreCase));

            if (country == null)
            {
                throw new Exception($"Can not find country code: {countryCode} in {nameof(OrdnanceSurveyCountries)}");
            }
            
            return country.CountryName;
        }
    }
}