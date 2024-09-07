namespace SpiWpf.Entities.DTOs
{
    public class ContractMacDTO
    {
        public int ContractId { get; set; }

        public int ContractMacId { get; set; }

        public int CargueDetailId { get; set; }

        public string? MacCliente { get; set; }

        public bool IsCreado { get; set; }
    }
}
