using System.ComponentModel.DataAnnotations;

namespace SpiWpf.Entities.Models
{
    public class ContractBind
    {
        public int ContractBindId { get; set; }

        public int ContractId { get; set; }

        public int ServerId { get; set; }

        public int IpNetId { get; set; }

        public int CargueDetailId { get; set; }

        public int HotSpotTypeId { get; set; }

        public string? MikrotikId { get; set; }

        public string? ServerName { get; set; }

        public string? IpServer { get; set; }

        public string? IpCliente { get; set; }

        public string? MacCliente { get; set; }
    }
}
