using SchedulerService.Dtos;

namespace SchedulerService.Dtos.Responses
{
    public record SubmitAvailableSlotResponse(AppointmentSlot[] TimeSlots);
}
