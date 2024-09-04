namespace SpiWpf.Entities.Models
{
    public class ContractDetailCls
    {
        public int ContractId { get; set; }

        public DateTime FechaContract { get; set; }

        public string? ControlContrato { get; set; } = null!;

        public string Contratista { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Documento { get; set; } = null!;

        public string Telefono { get; set; } = null!;

        public string Correo { get; set; } = null!;

        public string Direccion { get; set; } = null!;

        public string Ciudad { get; set; } = null!;

        public string Zona { get; set; } = null!;

        public bool IsAntena { get; set; }

        public bool IsInvoice { get; set; }

        public string StateType { get; set; } = null!;

        public string Color { get; set; } = null!;

        public int CorporateId { get; set; }
    }
}
