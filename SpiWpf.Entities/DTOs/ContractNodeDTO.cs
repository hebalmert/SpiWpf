namespace SpiWpf.Entities.DTOs
{
    public class ContractNodeDTO
    {
        public int ContractId { get; set; }

        public int ContractNodeId { get; set; }

        public int NodeId { get; set; }

        public string NodeName { get; set; } = null!;

        public string Ip { get; set; } = null!;

        public bool IsCreado { get; set; }
    }
}
