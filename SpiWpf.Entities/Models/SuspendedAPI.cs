namespace SpiWpf.Entities.Models
{
    public class SuspendedAPI
    {
        public int ContractSuspendedId { get; set; }

        public DateTime DateSuspended { get; set; }

        public string? NombreCliente { get; set; }

        public string? Contrato { get; set; }

        public string? Motivo { get; set; }

        public string? PlanName { get; set; } = null!;

        public decimal? MontoPlan { get; set; }

        public int CorporateId { get; set; }
    }
}
