using ASC.Utilities.ValidationAttributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace ASC.Web.Areas.ServiceRequests.Models
{
    public class ServiceRequestViewModel
    {
        [Required]
        [Display(Name = "Vehicle Name")]
        public string VehicleName { get; set; }

        [Required]
        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; }

        [Required]
        [Display(Name = "Requested Services")]
        public string RequestedServices { get; set; }

        [Required]
        [FutureDate(90)]
        [Remote(action: "CheckDenailService", controller: "ServiceRequest", areaName: "ServiceRequests")]
        [Display(Name = "Requested Date")]
        public DateTime? RequestedDate { get; set; }
    }
}
