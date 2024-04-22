namespace Application.Models
{
    public class CoordinatesModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int SRID { get; set; } = 4326; 
    }
}
