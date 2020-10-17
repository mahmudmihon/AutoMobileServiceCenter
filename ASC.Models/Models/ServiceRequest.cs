using System;
using System.ComponentModel.DataAnnotations;

namespace ASC.Models.Models
{
    public class ServiceRequest
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string VehicleName { get; set; }
        public string VehicleType { get; set; }
        public string Status { get; set; }
        public string RequestedServices { get; set; }
        public DateTime? RequestedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string ServiceEngineer { get; set; }
    }
}
