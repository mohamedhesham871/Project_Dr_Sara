namespace Project_Dr_Sara.Dtos
{
    public class DoctorShortDto
    {
        public string UsreId { get; set; }
        public string ArabicName { get; set; } = string.Empty;
        public string EnglishName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public double Rating { get; set; }
        public string ProfilePictureUrl { get; set; } = string.Empty;



    }
}
