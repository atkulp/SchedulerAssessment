using SchedulerService.Dtos;

namespace SchedulerService.Dtos.Requests
{
    public record ReserveAppointmentSlotRequest(AppointmentSlot Slot);
}
