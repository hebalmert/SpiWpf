namespace SpiWpf.Entities.DTOs
{
    public class ContractServDTO
    {
        public int ContractId { get; set; }

        public int ContractServerId { get; set; }

        public int ServerId { get; set; }

        public string IpServidor { get; set; } = null!;

        public string NameServidor { get; set; } = null!;

        public string Usuario { get; set; } = null!;

        public string Clave { get; set; } = null!;

        public int ApiPort { get; set; }

        public bool IsCreado { get; set; }
    }
}
