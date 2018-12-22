// This recieves the Ajax submitted form when editing a user's profile info

[HttpPost]
[ValidateAntiForgeryToken]
public PartialViewResult SaveUserDetails(EditUserVm model)
{
    string view = "_EditUserDetails";
    try
    {
        // If the returned model has errors return to _EditUserDetails view with error messages
        if (!ModelState.IsValid)
        {
            var invalidmodel = getEditUserDetailVm(model.Id);
            return PartialView(view, invalidmodel);
        }

        var user = db.Users.SingleOrDefault(x => x.Id == model.Id);
        if (user != null)
        {
            // set new values to database
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.MiddleName = model.MiddleName;
            user.HireDate = model.HireDate;
            user.EndDate = model.EndDate;
            user.Department = model.Department;
            user.Position = model.Position;
            user.BirthDate = model.BirthDate;
            user.Email = model.WorkEmail;
            user.AlternateEmail = model.AlternateEmail;
            user.WorkPhone = model.WorkPhone;
            user.MobilePhone = model.MobilePhone;
            user.PhoneNumber = model.HomePhone;
            user.Address = model.Address;
            user.HourlyPayRate = model.HourlyPayRate;
            user.Fulltime = model.Fulltime;
            user.UserName = model.LoginName;
            db.SaveChanges();
        }
        ViewBag.EditMsg = "User details have been updated!";
        // If save was successful, return final view with success msg
        var userDetailVm = getUserDetailVm(model.Id);
        return PartialView("_UserDetails", userDetailVm);
    }
    catch (Exception e)
    {
        // Catch any potential errors modelstate doesnt catch
        ViewBag.EditMsg = "Error with form data, check to make sure valid information is put in each field.";
    }

    // return _EditUserDetails view with Catch Exception errors and msg
    var userEditDetailVm = getEditUserDetailVm(model.Id);
    return PartialView(view, userEditDetailVm);
}