namespace SpiWpf.Entities.Models
{
    public class ServerCon
    {
        public int ServerId { get; set; }

        public string? IpServidor { get; set; }

        public string? Usuario { get; set; }

        public string? Clave { get; set; }

        public int ApiPort { get; set; }
    }
}
