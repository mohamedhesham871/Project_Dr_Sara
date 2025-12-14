namespace Project_Dr_Sara.Dtos
{
    public class JWT
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string SecretKey {  get; set; } = string.Empty;
        public int DurationDays { get; set; }


    }
}
