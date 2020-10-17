using System;

namespace ASC.Models.Models
{
    public class ServiceRequestMessage
    {
        public int Id { get; set; }
        public string FromDisplayName { get; set; }
        public string FromEmail { get; set; }
        public string Message { get; set; }
        public DateTime? MessageDate { get; set; }
    }
}
