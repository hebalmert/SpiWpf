namespace SpiWpf.Entities.DTOs
{
    public class QueueParentDTO
    {
        public int QueueParentId { get; set; }

        public string ParentName { get; set; } = null!;

        public int ServerId { get; set; }

        public int PlanId { get; set; }

        public string Down { get; set; } = null!;

        public string Up { get; set; } = null!;

        public string MkId { get; set; } = null!;
    }
}
