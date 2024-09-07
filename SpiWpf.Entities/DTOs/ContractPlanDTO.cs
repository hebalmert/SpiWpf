namespace SpiWpf.Entities.DTOs
{
    public class ContractPlanDTO
    {
        public int ContractId { get; set; }

        public int ContractPlanId { get; set; }

        public int PlanId { get; set; }

        public string PlanName { get; set; } = null!;

        public string Velocidad { get; set; } = null!;

        public decimal Precio { get; set; }

        public bool IsCreado { get; set; }
    }
}
