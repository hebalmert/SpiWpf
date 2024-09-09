using SpiWpf.Entities.Enum;

namespace SpiWpf.Entities.DTOs
{
    public class ContractPlanDTO
    {
        public int ContractId { get; set; }

        public int ContractPlanId { get; set; }

        public int PlanId { get; set; }

        public string PlanName { get; set; } = null!;

        public int SpeedUp { get; set; }

        public SpeedUpType SpeedUpType { get; set; }

        public int SpeedDown { get; set; }

        public SpeedDownType SpeedDownType { get; set; }

        public int TasaReuso { get; set; }

        public decimal Precio { get; set; }

        public bool IsCreado { get; set; }


        //Propiedades Virtuales 
        public string VelocidadDown => Convert.ToString(SpeedDown) + SpeedDownType;

        public string VelocidadUp => Convert.ToString(SpeedUp) + SpeedUpType;

        //muestra velocidad en Up / Down
        public string VelocidadTotal => $"{VelocidadUp}/{VelocidadDown}";
    }
}
