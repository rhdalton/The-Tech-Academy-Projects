// This is the action for creating a new Schedule after the form has been submitted via Ajax

[HttpPost]
[ValidateAntiForgeryToken]
public PartialViewResult Create(ScheduleViewModel model)
{
    // Ensure shifttemplates is passed back to the view
    ViewData["shifttemplates"] = GetShiftTemplates();

    // Get the Application user for this schedule
    var user = db.Users.Find(model.UserId);
    
    // Create new Schedule with model and user
    Schedule new_schedule = new Schedule(model, user);
    

    // A valid Modelstate is not enough to save the schedule because StartTime and EndTime can accept null values,
    // but we don't want null values submitted, so check for null values and return validation error if IsDayOff is false and Start/End time is null

    // Loop through each Workperiod and check for null values for Start/End time values if IsDayOff is false
    int daycount = 0;
    foreach (var workperiod in model.WorkPeriods)
    {
        // This property needs a value for valid modelstate
        workperiod.Schedule = new_schedule;

        // Check if IsDayOff false and StartTime or EndTime is null
        if (!workperiod.IsDayOff && (workperiod.StartTime == null || workperiod.EndTime == null))
        {
            // if Start/End time null, return model with error message
            ModelState.AddModelError(string.Empty, "Start Time and End Time can't be blank if shift is not a Day Off.");
            return PartialView("_scheduleShiftsTable", model);
        }

        // if IsDayOff true, the StartTime field will be blank, but we still need a Start datetime set for database, so set a default of 0:00
        if (workperiod.IsDayOff)
        {
            workperiod.StartTime = new DateTime(2000, 1, 1, 0, 0, 0);
        }
        else
        {
            // If it's not a day off, then we need to set the full Start/End date to the Schedule start day + the Start/End time
            // Then for each additional day in the Schedule, we need to add +1 day to the Start/End times.
            workperiod.StartTime = model.ScheduleStartDay.Add(workperiod.StartTime.GetValueOrDefault().TimeOfDay).AddDays(daycount);
            workperiod.EndTime = model.ScheduleStartDay.Add(workperiod.EndTime.GetValueOrDefault().TimeOfDay).AddDays(daycount);

        }
        daycount++;
    }

    // Check if Modelstate is valid for all properties, if not, return the view with errors
    if (!ModelState.IsValid)
    {
        ModelState.AddModelError(string.Empty, "Invalid values submitted to form, please check the errors below.");
        return PartialView("_scheduleShiftsTable", model);
    }

    // If no errors, save model and return schedule create page
    db.Schedules.Add(new_schedule);
    db.SaveChanges();
    ViewBag.SuccessMsg = "Schedule has been added!";

    return PartialView("_scheduleShiftsTable", model);
}