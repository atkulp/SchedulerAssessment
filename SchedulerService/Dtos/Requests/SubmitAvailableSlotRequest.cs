namespace SchedulerService.Dtos.Requests
{
    public class SubmitAvailableSlotRequest
    {
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
    }
}
