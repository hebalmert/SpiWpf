namespace SpiWpf.Entities.Models
{
    public class ClientsAPI
    {
        public int ClientId { get; set; }

        public string? Document { get; set; }

        public string? FullName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? UserName { get; set; }

        public bool Active { get; set; }
    }
}
