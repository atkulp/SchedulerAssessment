namespace SchedulerService.Dtos
{
    public class Appointment
    {
        public Appointment(string client, AppointmentSlot slot)
        {
            Client = client;
            Slot = slot;
            ReservationTime = DateTime.UtcNow;
        }

        public AppointmentSlot Slot { get; set; }

        public string Client { get; set; }

        public DateTime ReservationTime { get; set; }

        public DateTime? ConfirmationTime { get; set; }
    }
}