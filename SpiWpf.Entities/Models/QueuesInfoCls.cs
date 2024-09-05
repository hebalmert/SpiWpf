namespace SpiWpf.Entities.Models
{
    public class QueuesInfoCls
    {
        public int ContractId { get; set; }

        public int ContractQueId { get; set; }

        public string Servidor { get; set; } = null!;

        public string IpAddress { get; set; } = null!;

        public string PlanName { get; set; } = null!;

        public string SpeedPlan { get; set; } = null!;

        public string MkIndex { get; set; } = null!;

        public bool IsCreado { get; set; }
    }
}
