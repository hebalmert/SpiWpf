namespace SpiWpf.Entities.Models
{
    public class BindingInfoCls
    {
        public int ContractId { get; set; }

        public int ContractBindId { get; set; }

        public string Servidor { get; set; } = null!;

        public string IpAddress { get; set; } = null!;

        public string Mac { get; set; } = null!;

        public string EstatusBinding { get; set; } = null!;

        public string Color { get; set; } = null!;

        public string MkIndex { get; set; } = null!;

        public bool IsCreado { get; set; }
    }
}
