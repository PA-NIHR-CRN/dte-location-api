using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Application.Models;
using Application.Responses.OrdnanceSurvey;

namespace Application.Mappings
{
    public class OrdnanceSurveyResponseMapper
    {
        public static PostcodeAddressModel MapTo(Dpa source)
        {
            return BuildAddress(source);
        }

        // THIS is a port over from the javascript used in the OS API form example
        // https://labs.os.uk/public/os-data-hub-examples/os-places-api/capture-and-verify-example-form-filling
        private static PostcodeAddressModel BuildAddress(Dpa source)
        {
            // The following is based on the rules for generating multi-line addresses which are
            // documented in Chapter 9 of the AddressBase Premium Getting Started Guide:
            // https://www.ordnancesurvey.co.uk/documents/product-support/getting-started/addressbase-premium-getting-started-guide.pdf

            // Define an empty address array variable.
            var arrAddrLine = new List<string>();

            // Define variables for DPA address components (blank if NULL).
            var dpaDepartmentName = source.DepartmentName ?? "";
            var dpaOrganisationName = source.OrganisationName ?? "";
            var dpaSubBuildingName = source.SubBuildingName ?? "";
            var dpaBuildingName = source.BuildingName ?? "";
            var dpaBuildingNumber = source.BuildingNumber ?? "";
            var dpaPoBoxNumber = source.PoBoxNumber ?? "";
            var dpaDependentThoroughfareName = source.DependantThroughFareName ?? "";
            var dpaThoroughfareName = source.ThroughFareName ?? "";
            var dpaDoubleDependentLocality = source.DoubleDependentLocality ?? "";
            var dpaDependentLocality = source.DependentLocality ?? "";
            var dpaPostTown = source.PostTown ?? "";
            var dpaPostcode = source.Postcode ?? "";
            var dpaX_Coordinate = source.X_Coordinate ?? "";
            var dpaY_Coordinate = source.Y_Coordinate ?? "";

            // Add a "PO BOX " prefix to the PO Box Number integer.
            if (dpaPoBoxNumber != "") dpaPoBoxNumber = $"PO BOX ${dpaPoBoxNumber}";

            // Define arrays for the premises and thoroughfare components of the address.
            var arrPremises = new List<string> { dpaBuildingNumber, dpaSubBuildingName, dpaBuildingName }.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            var arrThoroughfareLocality = new List<string> { dpaDependentThoroughfareName, dpaThoroughfareName, dpaDoubleDependentLocality, dpaDependentLocality }.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

            // Define an empty string to store the appropriately combined/structured premises and
            // thoroughfare components.
            var strPremisesThoroughfareLocality = "";

            // Define a regular expression to test for a letter suffix (e.g. '11A') or number
            // range (e.g. '3-5'). Combine the first values from the premises and thoroughfare
            // arrays into a string; before removing them from the array.
            const string numberLetterRegex = "^[1-9]+[a-zA-Z]$";
            const string numberRangeDashRegex = "^[1-9]+-[1-9]+$";
            var subBuildingNameMatch = Regex.IsMatch(dpaSubBuildingName, numberLetterRegex) || Regex.IsMatch(dpaSubBuildingName, numberRangeDashRegex);
            var buildingNameMatch = Regex.IsMatch(dpaBuildingName, numberLetterRegex) || Regex.IsMatch(dpaBuildingName, numberRangeDashRegex);
            if (subBuildingNameMatch || buildingNameMatch || dpaBuildingNumber != "")
            {
                strPremisesThoroughfareLocality = $"{arrPremises[0]} {arrThoroughfareLocality[0]}";
                arrThoroughfareLocality.RemoveAt(0);
                arrPremises.RemoveAt(0);
            }

            // Push the Organisation Name, Department Name and PO Box Number to the address array.
            arrAddrLine.Add(dpaOrganisationName);
            arrAddrLine.Add(dpaDepartmentName);
            arrAddrLine.Add(dpaPoBoxNumber);

            // Merge the structured premises and thoroughfare components into the address array.
            arrAddrLine = arrAddrLine.Concat(arrPremises).ToList();
            arrAddrLine.Add(strPremisesThoroughfareLocality);
            arrAddrLine = arrAddrLine.Concat(arrThoroughfareLocality).ToList();

            // Remove any duplicates and blanks from the address array.
            arrAddrLine = arrAddrLine.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

            return new PostcodeAddressModel
            {
                FullAddress = source.Address,
                AddressLine1 = arrAddrLine.ElementAtOrDefault(0),
                AddressLine2 = arrAddrLine.ElementAtOrDefault(1),
                AddressLine3 = arrAddrLine.ElementAtOrDefault(2),
                AddressLine4 = arrAddrLine.ElementAtOrDefault(3),
                Town = dpaPostTown,
                Postcode = dpaPostcode,
                X_Coordinate = dpaX_Coordinate,
                Y_Coordinate = dpaY_Coordinate
            };
        }
    }
}