// This is the function to display a calendar of user schedules and timeoffevents
// This calendar uses the fullCalendar JavaScript events calendar https://fullcalendar.io

function showCalendar(userId, eventType = '', showdept = false) {

    if (eventType == 'schedule') eventTypeUrl = GetUserScheduleURL + '/' + userId;
    else eventTypeUrl = GetUserEventsURL + '/' + userId;

    // if showdept parameter is true, set IncludeDepartment as true in url string
    if (showdept) eventTypeUrl += "?IncludeDepartment=true";

    $('#calendar').hide().fullCalendar('destroy');
    $('#calendar').fadeIn().fullCalendar({
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaWeek,agendaDay'
        },
        // Show modal with event details if event is clicked on calendar
        eventClick: function (event) {
            $('#modalTitle').html(event.FirstName + " " + event.LastName + " - " + event.title);
            if (!event.description) {
                event.description = "none";
            }
            var s = new Date(event.start);
            var e = new Date(event.end);
            var month = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
            $('#modalBody').html(month[s.getMonth()] + "  " + s.getDate() + ", " + s.getFullYear() + ": " + s.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }) + " - " + month[e.getMonth()] + "  " + e.getDate() + ", " + e.getFullYear() + ": " + e.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }));
            $('#modalNote').html("<b>Notes:</b> " + event.description);
            $('#eventUrl').attr('href', event.url);
            $('#fullCalModal').modal();
            return false;
        },
        events: eventTypeUrl,
        defaultView: 'month',
        editable: true,
        allDaySlot: false,
        selectable: true,
        slotMinutes: 15,
        displayEventEnd: true,
        timezone: "local"
    });
};
