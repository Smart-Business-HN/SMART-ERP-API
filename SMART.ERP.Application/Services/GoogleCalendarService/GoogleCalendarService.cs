using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SMART.ERP.Domain.Settings;

namespace SMART.ERP.Application.Services.GoogleCalendarService
{
    public class GoogleCalendarService : IGoogleCalendarService
    {
        private readonly GoogleCalendarSettings _calendarSettings;
        public GoogleCalendarService(IOptions<GoogleCalendarSettings> calendarSettings)
        {
            _calendarSettings = calendarSettings.Value;
        }

        public CalendarService GetCalendarServiceAsync()
        {
            try
            {

                string[] scopes = {
                CalendarService.Scope.Calendar,
                CalendarService.Scope.CalendarEvents,
                CalendarService.Scope.CalendarEventsReadonly,
                CalendarService.Scope.CalendarSettingsReadonly,
                };
                var json = JsonConvert.SerializeObject(_calendarSettings, Formatting.Indented, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

                GoogleCredential credential = GoogleCredential.FromJson(json)
                    .CreateScoped(scopes).CreateWithUser("dev@grupoplatino.hn");

                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "PlatinoHN",
                });

                return service;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Event> CreateEventAsync(CalendarService service, string email, DateTime initDate, DateTime endDate, string subject, string message)
        {
            try
            {
                var body = new Event();
                var attendes = new List<EventAttendee>()
                {
                    new EventAttendee() { Email = "dtabora@grupoplatino.hn"},
                };
                body.Attendees = attendes;
                var start = new EventDateTime
                {
                    DateTime = initDate
                };
                var end = new EventDateTime
                {
                    DateTime = endDate
                };
                var parameters = new EventArgs();
                body.Start = start;
                body.End = end;
                body.Summary = subject;
                body.Description = message;

                EventsResource.InsertRequest request = service.Events.Insert(body, "primary");
                request.SendNotifications = true;
                return await request.ExecuteAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateEventAsync(CalendarService service, string eventId, DateTime initDate, DateTime endDate, string subject, string message)
        {
            try
            {
                EventsResource.ListRequest request = service.Events.List("primary");
                request.TimeMin = DateTime.Now;
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.MaxResults = 40;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
                Events events = request.Execute();
                if (events.Items != null && events.Items.Count > 0)
                {
                    foreach (var item in events.Items)
                    {
                        if (item.Id == eventId)
                        {
                            EventDateTime start = new EventDateTime();
                            start.DateTime = new DateTime(initDate.Year, initDate.Month, initDate.Day, 8, 0, 0);
                            EventDateTime end = new EventDateTime();
                            end.DateTime = new DateTime(endDate.Year, endDate.Month, endDate.Day, 17, 0, 0);
                            item.Start = start;
                            item.End = end;
                            item.Summary = subject;
                            item.Description = message;
                            await service.Events.Update(item, "primary", item.Id).ExecuteAsync();
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteEventAsync(CalendarService service, string eventId)
        {
            try
            {
                EventsResource.ListRequest request = service.Events.List("primary");
                request.TimeMin = DateTime.Now;
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.MaxResults = 40;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
                Events events = request.Execute();
                if (events.Items != null && events.Items.Count > 0)
                {
                    foreach (var item in events.Items)
                    {
                        if (item.Id == eventId)
                        {
                            await service.Events.Delete("primary", item.Id).ExecuteAsync();
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
