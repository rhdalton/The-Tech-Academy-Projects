// This method retrieves the user's schedule for display in a calendar. Start and End are passed in from the FullCalendar API
// Optional parameter IncludeDepartment can be used to fetch schedules from all users in current department for display

public ActionResult getUserSchedule(string Id, DateTime start, DateTime end, bool IncludeDepartment = false)
{
    string serializedSchedule = "";
    // If IncludeDepartment is true, get schedules of all users in current Department
    if (IncludeDepartment)
    {
        // Get current user's department                
        var userDept = db.Users.Where(x => x.Id == Id).Select(d => d.Department).FirstOrDefault();
        // Then get a list of user Ids for the department
        List<string> userList = db.Users.Where(x => x.Department == userDept).Select(i => i.Id).ToList();

        // Establish a list of department schedules and Timeoffevents
        List<Schedule> userSchedules = new List<Schedule>();
        List<List<TimeOffEvent>> deptTimeOffEvents = new List<List<TimeOffEvent>>();
        // Loop through each user and get their Schedules and Timeoffevents to put into their respective lists
        // Important to do both in a single loop to ensure user schedules at timeoffevents are in sync in both lists
        foreach (var userId in userList)
        {
            Schedule userSchedule = db.Schedules.Where(g => g.UserId == userId).FirstOrDefault();
            userSchedules.Add(userSchedule);

            List<TimeOffEvent> userTimeOffRequests = db.TimeOffEvents.Where(e => e.User.Id == userId).AsEnumerable().ToList();
            deptTimeOffEvents.Add(userTimeOffRequests);
        }

        // Parse these two lists for display in Calendar
        serializedSchedule = CurrentSchedulesFromDept(userSchedules, deptTimeOffEvents, start, end);
    }
    else
    {
        Schedule userSchedule = db.Schedules.Where(g => g.UserId == Id).FirstOrDefault();
        //gets timeOffRequests that match userID and fall within requested time frame
        List<TimeOffEvent> userTimeOffRequests = db.TimeOffEvents.Where(TOE => TOE.User.Id == Id).AsEnumerable().ToList();

        // Parse string to pass to FullCalendar
        serializedSchedule = CurrentScheduleToCalendar(userSchedule, userTimeOffRequests, start, end);
    }

    // Return the seralized calendar to display
    return this.Content(serializedSchedule, "application/json");
}

// Get a list of schedules from a specific department, return schedules
public List<Schedule> getDepartmentSchedules(string dept)
{
    List<Schedule> deptSchedules = (from s in db.Schedules
                                    join a in db.Users on s.UserId equals a.Id
                                    where a.Department == dept
                                    orderby a.Id ascending
                                    select s).ToList();
    return deptSchedules;
}

// Get a list of a User's Timeoffevents from a department
public List<List<TimeOffEvent>> getDepartmentTimeOffEvents(string dept)
{
    var deptTimeOffevents = new List<List<TimeOffEvent>>();

    var userIds = (from u in db.Users where u.Department == dept orderby u.Id ascending select u.Id).ToList();

    // loop through each user and get time off events of each and put in list
    foreach (string userId in userIds)
    {
        List<TimeOffEvent> userTimeoffEvents = db.TimeOffEvents.Where(x => x.User.Id == userId).AsEnumerable().ToList();
        deptTimeOffevents.Add(userTimeoffEvents);
    }

    return deptTimeOffevents;
}


// This method parses all User's schedules and Timeoffevents from a department into a string to pass to the Calendar
public static string CurrentSchedulesFromDept(List<Schedule> schedules, List<List<TimeOffEvent>> timeoffevents, DateTime startDate, DateTime endDate)
{
    if (schedules != null)
    {
        // Full eventlist of all users, this will be what is sent to the calendar
        var fullEventList = new List<CalendarEvent>();

        // Color of each user's schedule events on calendar, will cycle through colors for all users
        // Avoid using green or red as they are default for Timeoffevents
        var colorCycle = new List<string>() { "purple", "blue", "#007880", "#ba5800" };
        var i = 0; // color counter   
        var u = 0; // user timeoutrequest counter

        // Loop through each user's schedule and add event to userEventList
        foreach (Schedule schedule in schedules)
        {
            // Eventlist of individual user - to be combined at end with all users.
            // We use indivdiual user eventlists so user Timeoffevents don't conflict other user schedules and only apply to current user
            var userEventList = new List<CalendarEvent>();

            // Parses user schedule and returns userEventList
            userEventList = parseUserScheduleForCalendar(schedule, userEventList);

              // increase i to change color for next user
            i++;
            // if i greater that colors in colorCycle list, go back to start of color list
            if (i >= colorCycle.Count) i = 0;

            // Add current user's Timeoffevents to userEventList
            // int u is the index of department's user Timeoffevents list, which is a single user's Timeoffevents
            foreach (var timeoffevent in timeoffevents[u])
            {
                // Parse and set TimeOffRequests in userEventList
                userEventList = parseTimeOffEventsForCalendar(timeoffevent, userEventList);
            }

            // add userEventList to full eventList after all user events and timeoffevents parsed
            fullEventList.AddRange(userEventList);

            // after done adding user schedule and timeoffrequests to fullEventList, go to next user
            // int u is user count, starting from 0.
            u++;
        }

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        string scheduletoFullCalendar = serializer.Serialize(fullEventList);
        // Return full events in string to display
        return scheduletoFullCalendar;
    }
    else
    {
        return null;
    }
}