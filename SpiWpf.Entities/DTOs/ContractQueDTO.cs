using SpiWpf.Entities.Enum;

namespace SpiWpf.Entities.DTOs
{
    public class ContractQueDTO
    {
        public int ContractQueId { get; set; }

        public int ContractId { get; set; }

        public string? NombreCliente { get; set; }

        public int ServerId { get; set; }

        public string? Usuario { get; set; }

        public string? Clave { get; set; }

        public int Puerto { get; set; }

        public int IpNetId { get; set; }

        public int PlanId { get; set; }

        public int TasaReuso { get; set; }

        public int SpeedUp { get; set; }

        public SpeedUpType SpeedUpType { get; set; }

        public int SpeedDown { get; set; }

        public SpeedDownType SpeedDownType { get; set; }

        public string? ServerName { get; set; }

        public string? IpServer { get; set; }

        public string? IpCliente { get; set; }

        public string? PlanName { get; set; }

        public string? TotalVelocidad { get; set; }

        public string? MikrotikId { get; set; }


        //Propiedades Virtuales 
        public string VelocidadDown => Convert.ToString(SpeedDown) + SpeedDownType;

        public string VelocidadUp => Convert.ToString(SpeedUp) + SpeedUpType;

        //muestra velocidad en Up / Down
        public string VelocidadTotal => $"{VelocidadUp}/{VelocidadDown}";
    }
}
