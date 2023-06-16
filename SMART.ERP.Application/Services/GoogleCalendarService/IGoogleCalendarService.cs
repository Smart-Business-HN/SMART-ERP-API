using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

namespace SMART.ERP.Application.Services.GoogleCalendarService
{
    public interface IGoogleCalendarService
    {
        Task<Event> CreateEventAsync(CalendarService service, string email, DateTime initDate, DateTime endDate, string subject, string message);
        Task UpdateEventAsync(CalendarService service, string eventId, DateTime initDate, DateTime endDate, string subject, string message);
        Task DeleteEventAsync(CalendarService service, string eventId);
        CalendarService GetCalendarServiceAsync();
    }
}
