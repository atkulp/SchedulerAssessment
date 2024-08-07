using Microsoft.AspNetCore.Mvc;
using SchedulerService.Dtos;
using SchedulerService.Dtos.Requests;
using SchedulerService.Repositories;

namespace SchedulerService.Controllers.ProviderScheduler
{
    [ApiController]
    [Route("[controller]")]
    public partial class ClientSchedulerController : ControllerBase
    {
        private readonly ILogger<ClientSchedulerController> _logger;
        private readonly ISchedulerRepository _schedulerRepository;

        public ClientSchedulerController(ILogger<ClientSchedulerController> logger, ISchedulerRepository schedulerRepository)
        {
            _logger = logger;
            _schedulerRepository = schedulerRepository;
        }

        /// <summary>
        /// Allows a client to retrieve a list of available appointment slots
        /// - Appointment slots are 15 minutes long
        /// </summary>
        [HttpGet(Name = "GetAvailableSlots")]
        [EndpointDescription("Returns the 15-minute appointment slots for a provider")]
        public IEnumerable<AppointmentSlot> Get([FromQuery]GetAvailableSlotsRequest request)
        {
            _logger.LogDebug("GetAvailableSlots");

            // Go to persistence layer and return slots without appointments
            return _schedulerRepository.GetAvailableSlots(request.Provider);
        }

        /// <summary>
        /// Allows clients to reserve an available appointment slot
        /// - Reservations must be made at least 24 hours in advance
        /// - Reservations expire after 30 minutes if not confirmed and are again available for other clients to reserve that appointment slot
        /// </summary>
        [HttpPut(Name = "ReserveAppointmentSlot")]
        [EndpointDescription("Sets a given appointment slot as reserved (but not confirmed)")]
        public ActionResult Put(ReserveAppointmentSlotRequest request)
        {
            _logger.LogDebug("ReserveAppointmentSlot");
            string client = User.Identity?.Name ?? "client";

            // Could return 404 if slot not found
            // Could return 409 if already reserved/confirmed by another client
            return Ok(_schedulerRepository.ReserveAppointmentSlot(client, request.Slot.Provider, request.Slot.TimeSlot));
        }

        /// <summary>
        /// Allows clients to confirm their reservation
        /// - Reservations must be made at least 24 hours in advance
        /// - Reservations expire after 30 minutes if not confirmed and are again available for other clients to reserve that appointment slot
        /// </summary>
        [HttpPost(Name = "ConfirmAppointmentSlot")]
        [EndpointDescription("Sets a reserved appointment to confirmed.")]
        public ActionResult Post(ConfirmAppointmentSlotRequest request)
        {
            _logger.LogDebug("ConfirmAppointmentSlot");
            string client = User.Identity?.Name ?? "client";

            // Could return 404 if slot not found
            // Could return 404 if reservation for this user is expired
            // Could return 409 if not reserved yet
            // Could return 409 if already reserved/confirmed by another client
            return Ok(_schedulerRepository.ConfirmAppointmentSlot(client, request.Slot.Provider, request.Slot.TimeSlot));
        }
    }
}
