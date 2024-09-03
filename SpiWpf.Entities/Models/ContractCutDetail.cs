namespace SpiWpf.Entities.Models
{
    public class ContractCutDetail
    {
        public int ContractCutDetailId { get; set; }

        public int ContractCutId { get; set; }

        public DateTime DateSuspended { get; set; }

        public int ClientId { get; set; }

        public int ContractId { get; set; }

        public string? PlanName { get; set; } = null!;

        public decimal? MontoPlan { get; set; }

        public int CorporateId { get; set; }

    }
}
