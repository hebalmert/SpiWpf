namespace SpiWpf.Entities.Models
{
    public class ContractCls
    {
        public int ContractId { get; set; }

        public string? ControlContrato { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Contratista { get; set; } = null!;

        public string Zona { get; set; } = null!;

        public string StateType { get; set; } = null!;

        public string Color { get; set; } = null!;

        public int CorporateId { get; set; }

        public bool IsBinding { get; set; }

        public bool IsQueues { get; set; }
    }
}
