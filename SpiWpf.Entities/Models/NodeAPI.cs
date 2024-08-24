namespace SpiWpf.Entities.Models
{
    public class NodeAPI
    {
        public int NodeId { get; set; }

        public string NodesName { get; set; } = null!;

        public string IpNetwork { get; set; } = null!;

        public string MarkModel { get; set; } = null!;

        public string Zona { get; set; } = null!;

        public bool APClientes { get; set; }

        public int Frecuency { get; set; }

        public string Channel { get; set; } = null!;

        public bool Active { get; set; }

        public int CorporateId { get; set; }
    }
}
