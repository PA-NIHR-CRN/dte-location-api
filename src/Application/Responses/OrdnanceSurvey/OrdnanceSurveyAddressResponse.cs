using System.Collections.Generic;
using Newtonsoft.Json;

namespace Application.Responses.OrdnanceSurvey
{
    public class OrdnanceSurveyAddressResponse
    {
        public List<Result> Results { get; set; }
    }

    public class Result
    {
        [JsonProperty("DPA")] public Dpa Dpa { get; set; }
    }

    public class Dpa
    {
        [JsonProperty("DEPARTMENT_NAME")] public string DepartmentName { get; set; }

        [JsonProperty("PO_BOX_NUMBER")] public string PoBoxNumber { get; set; }

        [JsonProperty("ADDRESS")] public string Address { get; set; }

        [JsonProperty("SUB_BUILDING_NAME")] public string SubBuildingName { get; set; }

        [JsonProperty("BUILDING_NAME")] public string BuildingName { get; set; }

        [JsonProperty("BUILDING_NUMBER")] public string BuildingNumber { get; set; }

        [JsonProperty("ORGANISATION_NAME")] public string OrganisationName { get; set; }

        [JsonProperty("THOROUGHFARE_NAME")] public string ThroughFareName { get; set; }

        [JsonProperty("DEPENDENT_THOROUGHFARE_NAME")]
        public string DependantThroughFareName { get; set; }

        [JsonProperty("DEPENDENT_LOCALITY")] public string DependentLocality { get; set; }

        [JsonProperty("DOUBLE_DEPENDENT_LOCALITY")]
        public string DoubleDependentLocality { get; set; }

        [JsonProperty("POST_TOWN")] public string PostTown { get; set; }

        [JsonProperty("POSTCODE")] public string Postcode { get; set; }

        [JsonProperty("COUNTRY_CODE")] public string CountryCode { get; set; }

        [JsonProperty("X_COORDINATE")] public string X_Coordinate { get; set; }

        [JsonProperty("Y_COORDINATE")] public string Y_Coordinate { get; set; }
        [JsonProperty("LAT")] public float Lat { get; set; }
        [JsonProperty("LNG")] public float Lng { get; set; }
    }
}
