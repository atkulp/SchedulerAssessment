using Microsoft.AspNetCore.Mvc;
using SchedulerService.Dtos.Requests;
using SchedulerService.Dtos.Responses;
using SchedulerService.Repositories;

namespace SchedulerService.Controllers.ProviderScheduler
{
    [ApiController]
    [Route("[controller]")]
    public partial class ProviderSchedulerController : ControllerBase
    {
        private readonly ILogger<ProviderSchedulerController> _logger;
        private readonly ISchedulerRepository _schedulerRepository;

        public ProviderSchedulerController(ILogger<ProviderSchedulerController> logger, ISchedulerRepository schedulerRepository)
        {
            _logger = logger;
            _schedulerRepository = schedulerRepository;
        }

        /// <summary>
        /// Allows providers to submit times they are available for appointments
        /// e.g. On Friday the 13th of August, Dr. Jekyll wants to work between 8am and 3pm
        /// </summary>
        [HttpPut(Name = "SubmitAvailableSlots")]
        [EndpointDescription("Adds a range of appointment slots for a provider")]
        public SubmitAvailableSlotResponse Put(SubmitAvailableSlotRequest request)
        {
            _logger.LogDebug("SubmitAvailableSlots");

            // Should have validation throughout. Fluent Validation is better!
            if (request.ToDateTime < request.FromDateTime)
            {
                throw new ArgumentException("Range error: To must be after the From", nameof(request.ToDateTime));
            }

            // Would have service injected to get user identity details from current request
            string provider = User.Identity?.Name ?? "provider";

            var slotList = _schedulerRepository.SubmitAvailableSlots(provider, request.FromDateTime, request.ToDateTime);

            return new SubmitAvailableSlotResponse([.. slotList]);
        }

    }
}
