namespace SpiWpf.Entities.DTOs
{
    public class DashBoardDTO
    {
        public int CltActivos { get; set; }

        public int CltSuspendidos { get; set; }

        public int CltSoportes { get; set; }

        public int Servidores { get; set; }

        public decimal NotaCobros { get; set; }

        public decimal NotaPendiente { get; set; }
    }
}
