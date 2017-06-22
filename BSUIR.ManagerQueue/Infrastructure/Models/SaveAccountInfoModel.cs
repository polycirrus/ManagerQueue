using System.ComponentModel.DataAnnotations;

namespace BSUIR.ManagerQueue.Infrastructure.Models
{
    public class SaveAccountInfoModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }

        public int? PositionId { get; set; }
        public string JobTitle { get; set; }

        [Required]
        public UserType Type { get; set; }
    }
}
