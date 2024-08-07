using SchedulerService.Dtos;
using SchedulerService.Extensions;
using System.Collections.Generic;

namespace SchedulerService.Repositories
{
    public class DummySchedulerRepository : ISchedulerRepository
    {
        // These are silly of course. A persistence repository would replace them
        private readonly List<Appointment> appointments = [];
        private readonly List<AppointmentSlot> appointmentSlots = [];

        public DummySchedulerRepository()
        {
            // Let's make some high-quality dummy data
            SubmitAvailableSlots("jeckyl", DateTime.UtcNow.AddHours(25), DateTime.UtcNow.AddHours(28));
            SubmitAvailableSlots("hyde", DateTime.UtcNow.AddHours(24), DateTime.UtcNow.AddHours(27));
        }

        /// <summary>
        /// Get the appointment slots for this provider (ignores reserved/confirmed for now)
        /// </summary>
        public IEnumerable<AppointmentSlot> GetAvailableSlots(string provider)
        {
            return appointmentSlots.Where(s => s.Provider == provider && s.TimeSlot > DateTime.UtcNow);
        }

        public Appointment? ReserveAppointmentSlot(string client, string provider, DateTime slot)
        {
            if (slot.Subtract(DateTime.UtcNow).TotalHours < 24 )
            {
                throw new Exception("Reservations must be made at least 24 hours in advance");
            }

            var appointmentSlot = appointmentSlots.FirstOrDefault(s => s.Provider == provider && s.TimeSlot == slot)
                ?? throw new Exception("No appointment slot for this provider at this time");

            // Get appointment in slot, if any
            var appointment = GetAppointmentForSlot(provider, slot);

            // See if there's any matching appointment
            if (appointment != null)
            {
                // Already something here. Make sure it belongs to this client
                if (appointment.Client == client)
                {
                    // If reserved, but not confirmed, refresh their ticket (is this right?)
                    if (appointment.ConfirmationTime == null)
                    {
                        appointment.ReservationTime = DateTime.UtcNow;
                    }

                    // Whether it was already confirmed or not, it's valid so return it.
                    return appointment;
                }
                else
                {
                    // Valid reservation for a different client
                    throw new Exception("Time slot is unavailable to reserve");
                }
            }

            // Not reserved yet. It's all yours
            appointment = new Appointment(client, appointmentSlot);
            appointments.Add(appointment);

            return appointment;
        }

        public Appointment? ConfirmAppointmentSlot(string client, string provider, DateTime slot)
        {
            var appointment = GetAppointmentForSlot(provider, slot);

            // See if there's any matching appointment
            if (appointment != null)
            {
                // Make sure it belongs to this client
                if (appointment.Client == client)
                {
                    // Mark as confirmed (ignore if it already is)
                    if (appointment.ConfirmationTime == null)
                    {
                        // Update to confirmed and return it
                        appointment.ConfirmationTime = DateTime.UtcNow;
                    }

                    // Whether it was already confirmed or not, it's valid so return it.
                    return appointment;
                }
                else
                {
                    throw new Exception("Time slot is unavailable to confirm");
                }
            }

            throw new Exception("Must reserve before confirming");
        }

        /// <summary>
        /// Allows the provider to submit a range of time newSlots available for appointments
        /// </summary>
        public IEnumerable<AppointmentSlot> SubmitAvailableSlots(string provider, DateTime fromDateTime, DateTime toDateTime)
        {
            // We have to assume that at some point we will need to
            // allow for different slot configurations for different providers or specialties.
            // This would be better as injectable based on some kind of SlotProvider.
            var slotTimes = fromDateTime.GetSlotsForTimeRange(toDateTime);

            var newSlots = new List<AppointmentSlot>();

            // Now we need to cycle through the slotTimes to create only those not yet existing.
            foreach (var slot in slotTimes)
            {
                // Skip any existing time newSlots for this provider
                var existingSlot = appointmentSlots.FirstOrDefault(s => s.Provider == provider && s.TimeSlot == slot);

                if (existingSlot == null)
                {
                    // Only add new entries
                    newSlots.Add(new AppointmentSlot(slot, provider));
                }
            }

            // Save our new slot entities
            this.appointmentSlots.AddRange(newSlots);

            // Return newSlots
            return newSlots;
        }

        private Appointment? GetAppointmentForSlot(string provider, DateTime slot)
        {
            var appointment = this.appointments.Where(s => s.Slot.Provider == provider && s.Slot.TimeSlot == slot).FirstOrDefault();

            // Is there an appointment for this slot?
            if (appointment != null)
            {
                var expired = DateTime.UtcNow.Subtract(appointment.ReservationTime).TotalMinutes > 30;

                // See if it's confirmed or within reservation window
                if (appointment.ConfirmationTime != null || !expired)
                {
                    // This is a valid reservation (confirmed or not)
                    return appointment;
                }

                // Mark this appointment reservation as expired (or just remove it)
                this.appointments.Remove(appointment);
            }

            return null;
        }

    }
}
