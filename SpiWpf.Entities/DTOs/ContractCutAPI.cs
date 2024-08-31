namespace SpiWpf.Entities.DTOs
{
    public class ContractCutAPI
    {
        public int ContractCutId { get; set; }

        public DateTime DateCut { get; set; }

        public int YearNumber { get; set; }

        public string? Mes { get; set; }

        public DateTime? DateStr { get; set; }

        public DateTime? DateEnd { get; set; }

        public bool Creado { get; set; }

        public DateTime? DateCreado { get; set; }

        public int CorporateId { get; set; }
    }
}
