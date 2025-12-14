namespace Project_Dr_Sara.Dtos
{
    public class GenericResponseDto
    {

        public bool Success { get; set; }
        public string Message { get; set; }=string.Empty;
        public int StatusCode { get; set; }
        public List<string> Errors { get; set; } = [];
        public DateTime Timestamp { get; set; }

    }
}
