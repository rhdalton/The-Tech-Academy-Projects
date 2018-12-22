// Actions for changing the role of a user in the system, roles include Terminated, ViewMode, User, Admin

[HttpPost]
public async Task<bool> TerminateUser(LoginViewModel lvm, string userToChange, string Role)
{
    // Check if valid admin user to make changes to user account
    if (CheckCredentials(lvm))
    {
        // If terminating a user, check for any active Worktime events or schedules and deactivate them
        if(Role == "Terminate")
        {
            // Check any active worktime events from terminated user that has not ended
            // if any exists, end the worktime event now
            ApplicationUser terminatedUser = db.Users.FirstOrDefault(x => x.Id == userToChange);
            var terminateEvents = db.WorkTimeEvents.FirstOrDefault(x => x.Id == terminatedUser.Id && !x.End.HasValue);
            if (terminateEvents != null)
            {
                terminateEvents.Note = "User terminated by admin";
                terminateEvents.Clockout();
                db.SaveChanges();
            }
            // Check if any existing schedules active without an End date, set end date to now
            var endSchedule = db.Schedules.FirstOrDefault(x => x.UserId == terminatedUser.Id && !x.ScheduleEndDay.HasValue);
            if (endSchedule != null)
            {
                endSchedule.SetScheduleEndDay();
                db.SaveChanges();
            }
        }        
        // Using UserController, change user role to Terminated
        using (UserController u = new UserController(UserManager, SignInManager))
        {
            await u.ChangeRole(userToChange, Role);
        }
        return true;
    }
    else
    {
        return false;
    }
}

// This method is to re-active a user that has been terminated
[HttpPost]
public async Task<bool> ReEmployUser(LoginViewModel lvm, string userToChange)
{
    if (CheckCredentials(lvm))
    {
        using (UserController u = new UserController(UserManager, SignInManager))
        {
            await u.ChangeRole(userToChange, "User");
        }
        return true;
    }
    else
    {
        return false;
    }
}