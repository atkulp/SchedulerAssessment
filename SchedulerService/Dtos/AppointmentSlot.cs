namespace SchedulerService.Dtos
{
    // TimeSlot date/time and Provider are actually fine as a unique key, but
    // if we let a provider cancel a slot then re-add it, maybe we
    // want the old one with a Cancelled status alongside a new one in
    // which case we'd want a new primary key.
    public record AppointmentSlot(DateTime TimeSlot, string Provider);
}
