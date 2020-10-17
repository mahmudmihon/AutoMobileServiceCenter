using ASC.Models.BaseTypes;
using ASC.Models.Models;
using ASC.Utilities;
using ASC.Web.Areas.ServiceRequests.Models;
using ASC.Web.Controllers;
using ASC.Web.DataAccess.Interfaces;
using ASC.Web.Models;
using ASC.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Areas.ServiceRequests.Controllers
{
    [Area("ServiceRequests")]
    public class ServiceRequestController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<ServiceRequest> _repo;
        private readonly IRepository<ServiceRequestMessage> _messageOperations;
        private readonly IUnitOfWork _uow;
        private readonly ISmsSender _smsSender;

        public ServiceRequestController(UserManager<ApplicationUser> userManager, IRepository<ServiceRequest> repo, IUnitOfWork uow,
            IRepository<ServiceRequestMessage> messageOperations, ISmsSender smsSender)
        {
            _userManager = userManager;
            _repo = repo;
            _uow = uow;
            _smsSender = smsSender;
            _messageOperations = messageOperations;
        }

        [HttpGet]
        public IActionResult ServiceRequest()
        {
            return View(new ServiceRequestViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ServiceRequest(ServiceRequestViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }


            var serviceRequest = new ServiceRequest
            {
                Email = HttpContext.User.GetCurrentUserDetails().Email,
                VehicleName = request.VehicleName,
                VehicleType = request.VehicleType,
                RequestedDate = request.RequestedDate,
                RequestedServices = request.RequestedServices,
                Status = Status.New.ToString()
            };

            await _repo.AddAsync(serviceRequest);
            await _uow.SaveAsync();

            return RedirectToAction("Dashboard", "Dashboard", new { Area = "ServiceRequests" });
        }

        [HttpGet]
        public async Task<IActionResult> ServiceRequestDetails(string id)
        {
            var serviceRequestDetails = _repo.GetBySpecificCondition($"Id == {int.Parse(id)}");

            // Access Check
            if (HttpContext.User.IsInRole(Roles.Engineer.ToString())
                && serviceRequestDetails.ServiceEngineer != HttpContext.User.GetCurrentUserDetails().Email)
            {
                throw new UnauthorizedAccessException();
            }

            if (HttpContext.User.IsInRole(Roles.User.ToString())
                && serviceRequestDetails.Email != HttpContext.User.GetCurrentUserDetails().Email)
            {
                throw new UnauthorizedAccessException();
            }

            ViewBag.Status = Enum.GetValues(typeof(Status)).Cast<Status>().Select(v => v.ToString()).ToList();
            ViewBag.ServiceEngineers = await _userManager.GetUsersInRoleAsync(Roles.Engineer.ToString());

            return View(new UpdateServiceRequestViewModel
            {
                Id = int.Parse(id),
                Email = serviceRequestDetails.Email,
                VehicleName = serviceRequestDetails.VehicleName,
                VehicleType = serviceRequestDetails.VehicleType,
                RequestedServices = serviceRequestDetails.RequestedServices,
                RequestedDate = serviceRequestDetails.RequestedDate,
                Status = serviceRequestDetails.Status,
                ServiceEngineer = serviceRequestDetails.ServiceEngineer
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateServiceRequestDetails(UpdateServiceRequestViewModel serviceRequest)
        {
            var originalServiceRequest = _repo.GetBySpecificCondition($"Id == {serviceRequest.Id}");
            originalServiceRequest.RequestedServices = serviceRequest.RequestedServices;

            var isServiceRequestsStatusUpdated = false;

            // Update Status only if user role is either Admin or Engineer
            // Or Customer can update the status if it is only in Pending Customer Approval.
            if (HttpContext.User.IsInRole(Roles.Admin.ToString()) ||
                HttpContext.User.IsInRole(Roles.Engineer.ToString()) ||
                (HttpContext.User.IsInRole(Roles.User.ToString()) && originalServiceRequest.Status == Status.PendingCustomerApproval.ToString()))
            {
                if(originalServiceRequest.Status != serviceRequest.Status)
                {
                    isServiceRequestsStatusUpdated = true;
                }
                originalServiceRequest.Status = serviceRequest.Status;
            }

            // Update Service Engineer field only if user role is Admin
            if (HttpContext.User.IsInRole(Roles.Admin.ToString()))
            {
                originalServiceRequest.ServiceEngineer = serviceRequest.ServiceEngineer;
            }

            _repo.Update(originalServiceRequest);
            await _uow.SaveAsync();

            //if (HttpContext.User.IsInRole(Roles.Admin.ToString()) ||
            //    HttpContext.User.IsInRole(Roles.Engineer.ToString()) || originalServiceRequest.Status == Status.PendingCustomerApproval.ToString())
            //{
            //    await _emailSender.SendEmailAsync(originalServiceRequest.PartitionKey,
            //            "Your Service Request is almost completed!!!",
            //            "Please visit the ASC application and review your Service request.");
            //}

            if(isServiceRequestsStatusUpdated)
            {
                await SendSmsAndWebNotifications(originalServiceRequest);
            }

            return RedirectToAction("ServiceRequestDetails", "ServiceRequest",
                new { Area = "ServiceRequests", Id = serviceRequest.Id });
        }

        private async Task SendSmsAndWebNotifications(ServiceRequest serviceRequest)
        {
            // Send SMS Notification
            var phoneNumber = (await _userManager.FindByEmailAsync(serviceRequest.Email)).PhoneNumber;
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                await _smsSender.SendSmsAsync(string.Format("+88{0}", phoneNumber),
                            string.Format("Service Request Status updated to {0}", serviceRequest.Status));
            }

            //// Get Customer name
            //var customerName = (await _userManager.FindByEmailAsync(serviceRequest.PartitionKey)).UserName;

            //// Send web notifications
            //_signalRConnectionManager.GetHubContext<ServiceMessagesHub>()
            //   .Clients
            //   .User(customerName)
            //   .publishNotification(new
            //   {
            //       status = serviceRequest.Status
            //   });
        }

        public IActionResult CheckDenialService(DateTime requestedDate)
        {
            var serviceRequests = _repo.GetByCondition($"RequestedDate >= \"{DateTime.Now.AddDays(-90)}\" && Email == \"{HttpContext.User.GetCurrentUserDetails().Email}\" && Status == \"Denied\"").ToList();

            if (serviceRequests.Any())
                return Json(data: $"There is a denied service request for you in last 90 days. Please contact ASC Admin.");

            return Json(data: true);
        }

        [HttpGet]
        public IActionResult ServiceRequestMessages(string serviceRequestId)
        {
            return Json(_messageOperations.GetByCondition($"Id == {int.Parse(serviceRequestId)}").ToList().OrderByDescending(p => p.MessageDate));
        }

        //[HttpGet]
        //public IActionResult SearchServiceRequests()
        //{
        //    return View(new SearchServiceRequestsViewModel());
        //}

        //[HttpGet]
        //public async Task<IActionResult> SearchServiceRequestResults(string email, DateTime? requestedDate)
        //{
        //    List<ServiceRequest> results = new List<ServiceRequest>();
        //    if(String.IsNullOrEmpty(email) && !requestedDate.HasValue)
        //        return Json(new { data = results });

        //    if(HttpContext.User.IsInRole(Roles.Admin.ToString()))
        //        results = await _serviceRequestOperations.GetServiceRequestsByRequestedDateAndStatus(requestedDate, null, email);
        //    else
        //        results = await _serviceRequestOperations.GetServiceRequestsByRequestedDateAndStatus(requestedDate, null, email, HttpContext.User.GetCurrentUserDetails().Email);

        //    return Json(new { data = results.OrderByDescending(p => p.RequestedDate).ToList() });
        //}
    }
}
