namespace SpiWpf.Entities.Models
{
    public class ContractToCutCls
    {
        public int idContrato { get; set; }

        public int idCorporate { get; set; }

        public int idCliente { get; set; }

        public int idServer { get; set; }

        public string nameserver { get; set; } = null!;

        public string ipservidor { get; set; } = null!;

        public string us { get; set; } = null!;

        public string pss { get; set; } = null!;

        public int puerto { get; set; }

        public string idIpBinding { get; set; } = null!;

        public string NamePlan { get; set; } = null!;

        public decimal ValorPlan { get; set; }
    }
}
