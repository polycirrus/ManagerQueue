using System.ComponentModel.DataAnnotations;

namespace BSUIR.ManagerQueue.Infrastructure.Models
{
    public class AddQueueEntryModel
    {
        [Required]
        public int QueueId { get; set; }

        [Required]
        public int EntrantId { get; set; }
    }
}
