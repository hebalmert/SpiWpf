using System.ComponentModel.DataAnnotations;

namespace SpiWpf.Entities.Models
{
    public class ContractQue
    {
        public int ContractQueId { get; set; }

        public int ContractId { get; set; }

        public int ServerId { get; set; }

        public int IpNetId { get; set; }

        public int PlanId { get; set; }

        public string? ServerName { get; set; }

        public string? IpServer { get; set; }

        public string? IpCliente { get; set; }

        public string? PlanName { get; set; }

        public string? TotalVelocidad { get; set; }

        public string? MikrotikId { get; set; }
    }
}
