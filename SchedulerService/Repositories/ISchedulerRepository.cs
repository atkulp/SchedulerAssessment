using SchedulerService.Dtos;

namespace SchedulerService.Repositories
{
    public interface ISchedulerRepository
    {
        Appointment? ConfirmAppointmentSlot(string client, string provider, DateTime slot);
        IEnumerable<AppointmentSlot> GetAvailableSlots(string provider);
        Appointment? ReserveAppointmentSlot(string client, string provider, DateTime slot);
        IEnumerable<AppointmentSlot> SubmitAvailableSlots(string provider, DateTime fromDateTime, DateTime toDateTime);
    }
}