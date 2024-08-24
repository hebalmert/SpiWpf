namespace SpiWpf.Entities.Models
{
    public class PlanAPI
    {
        public int PlanId { get; set; }

        public string PlanName { get; set; } = null!;

        public string Categoria { get; set; } = null!;

        public string SpeedUp { get; set; } = null!;

        public string SpeedDown { get; set; } = null!;

        public int Reuso { get; set; }

        public decimal Impuesto { get; set; }

        public decimal Precio { get; set; }

        public bool Active { get; set; }

        public int CorporateId { get; set; }

        public string Velocidad => $"{SpeedUp} / {SpeedDown}";
    }
}
