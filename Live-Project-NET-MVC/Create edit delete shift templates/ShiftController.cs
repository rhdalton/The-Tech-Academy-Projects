// POST: Employer/Shift/Create
// Controller action for creating a shift template

[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Create([Bind(Include = "Id,StartTime,EndTime")] ScheduleShiftTemplate shift)
{
    // A valid modelstate is not enough for validation, since Starttime and Endtime properties can allow null values,
    // so we check to make sure modelstate is valid and start/end times are not null to create the shift
    if (ModelState.IsValid && shift.StartTime != null && shift.EndTime != null)
    {
        db.ScheduleShiftTemplates.Add(shift);
        db.SaveChanges();
        // redirct to "ShiftModal" action
        return RedirectToAction("ShiftModal");
    }
    // If shift Start/End times null, return model error
    else if (shift.StartTime == null || shift.EndTime == null)
    {
        ModelState.AddModelError(string.Empty, "Start Time and End Time can't be blank.");
    }
    return PartialView(shift);
}

// This action returns the selected shift from the dropdown to edit
//GET: Employer/Shift/Edit/5
public ActionResult Edit(string id)
{
    // if id = 1 ( Create New shift template ) load the Create.cshtml contents into partial view
    if (id == "1")
    {
        // Assign blank values to new shift, Id = 1 is just a placeholder id value, a real id is generated when new shift is created
        ScheduleShiftTemplate newshift = new ScheduleShiftTemplate() { Id = "1", StartTime = null, EndTime = null };
        return PartialView("Create", newshift);
    }
    else if (id == null)
    {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    }
    else
    {
        ScheduleShiftTemplate shift = db.ScheduleShiftTemplates.Find(id);
        if (shift == null)
        {
            return HttpNotFound();
        }
        return PartialView("Edit", shift);
    }
}

// This is the Post of editing a shift, check for valid values and update database.
[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Edit([Bind(Include = "Id,StartTime,EndTime")] ScheduleShiftTemplate shift)
{
    // A valid modelstate is not enough for validation, since Starttime and Endtime properties can allow null values,
    // so we check to make sure modelstate is valid and start/end times are not null to create the shift
    if (ModelState.IsValid && shift.StartTime != null && shift.EndTime != null)
    {
        db.Entry(shift).State = EntityState.Modified;
        db.SaveChanges();
        // if edit successful return to ShiftModal
        return RedirectToAction("ShiftModal");
    }
    else if (shift.StartTime == null || shift.EndTime == null)
    {
        ModelState.AddModelError(string.Empty, "Start Time and End Time can't be blank.");
    }
    // if edit fail load partial with model errors
    return PartialView(shift);
}

// POST: Employer/Shift/Delete/5
[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult DeleteConfirmed([Bind(Include = "Id")] ScheduleShiftTemplate shift)
{
    ScheduleShiftTemplate deleteshift = db.ScheduleShiftTemplates.Find(shift.Id);
    db.ScheduleShiftTemplates.Remove(deleteshift);
    db.SaveChanges();
    // if delete successful return to ShiftModal
    return RedirectToAction("ShiftModal");
}