// This method filters the Users list from various conditions and returns the new User list to partial view.

public PartialViewResult FilterUserSearch(string filter)
{
    // Get all users into a list
    var allUsers = db.Users.ToList();

    // If no filter set, return all users
    if (filter == null || filter == "")
    {
        return PartialView(allUsers);
    }
    
    // Define list for filtered users
    var filtered = new List<ApplicationUser>();

    // Set filtercase for switch action by parsing filter string
    var filtercase = "";
    if (filter.Substring(0, 3) == "dpt") filtercase = "Department";
    else if (filter.Substring(0, 3) == "pos") filtercase = "Position";
    else filtercase = filter;

    switch (filtercase)
    {
        // Filter users that are Clocked in/out
        case "Clocked In":
            foreach (var user in allUsers)
            {
                if (user.GetStatus() == "Clocked In") filtered.Add(user);
            }
            break;

        case "Clocked Out":
            foreach (var user in allUsers)
            {
                if (user.GetStatus() == "Clocked Out") filtered.Add(user);
            }
            break;
        
        // Filter users that are Full time/Part time
        case "Full time":
            filtered = db.Users.Where(x => x.Fulltime == true).ToList();
            break;

        case "Part time":
            filtered = db.Users.Where(x => x.Fulltime == false).ToList();
            break;
        
        // Filter users by department or Position
        case "Department":
            filtered = db.Users.Where(x => x.Department == filter.Substring(4)).ToList();
            break;

        case "Position":
            filtered = db.Users.Where(x => x.Position == filter.Substring(4)).ToList();
            break;
        
        // If none of these conditions match, set list to all users.
        default:
            filtered = allUsers;
            break;
    }

    // Return the partial view of filtered users
    return PartialView("_UserList", filtered);
}