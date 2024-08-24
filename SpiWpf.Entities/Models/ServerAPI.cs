namespace SpiWpf.Entities.Models
{
    public class ServerAPI
    {
        public int ServerId { get; set; }

        public string? ServerName { get; set; } 

        public string? IpNetwork { get; set; }

        public string? Usuario { get; set; }

        public string? Clave { get; set; } 

        public int ApiPort { get; set; }

        public string? WanName { get; set; } 

        public string? Marka { get; set; }

        public string? MarkModelo { get; set; } 

        public string? Zona { get; set; }

        public bool Active { get; set; }

        public int CorporateId { get; set; }
    }
}
