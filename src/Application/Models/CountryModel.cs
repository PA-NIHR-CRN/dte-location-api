namespace Application.Models
{
    public class CountryModel
    {
        public CountryModel(string countryName, string countryCode)
        {
            CountryName = countryName;
            CountryCode = countryCode;
        }
            
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
    }
}