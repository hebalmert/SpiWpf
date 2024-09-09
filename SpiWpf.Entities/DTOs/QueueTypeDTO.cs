namespace SpiWpf.Entities.DTOs
{
    public class QueueTypeDTO
    {
        public int QueueTypeId { get; set; }

        public string TypeName { get; set; } = null!;

        public bool Down { get; set; }

        public bool Up { get; set; }

        public bool Active { get; set; }

        public int CorporateId { get; set; }
    }
}
