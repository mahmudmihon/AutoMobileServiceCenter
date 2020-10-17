using System.ComponentModel.DataAnnotations;

namespace ASC.Web.Areas.ServiceRequests.Models
{
    public class UpdateServiceRequestViewModel : ServiceRequestViewModel
    {
        public int Id { get; set; }

        public string Email { get; set; }

        [Required]
        [Display(Name = "Service Engineer")]
        public string ServiceEngineer { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; }
    }
}
