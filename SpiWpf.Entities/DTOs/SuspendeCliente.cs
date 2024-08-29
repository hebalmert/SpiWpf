namespace SpiWpf.Entities.DTOs
{
    public class SuspendeCliente
    {
        public int ClientId { get; set; }

        public int ContractId { get; set; }

        public string? ControlContrato { get; set; }

        public string? Motivo { get; set; }

        public string? PlanName { get; set; }

        public decimal? MontoPlan { get; set; }
    }
}
