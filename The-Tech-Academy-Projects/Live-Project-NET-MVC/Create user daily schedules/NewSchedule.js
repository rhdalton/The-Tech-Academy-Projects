/*
 * These are JavaScript/jQuery functions for new Schedule functionality
 */
function addShiftRow() {
    // Parse last shift row to pass to new row
    var getid = $('#shift-row-container tr').last().attr('id').split('-');
    var id = parseInt(getid[1]) + 1;

    // Pass the new Id to the controller to add new shift row partial
    $.ajax({
        url: NewShiftRowPartialView,
        data: {
            shiftcount: id
        },
        success: function (data) {
            // Add new row to end of table
            $('#shift-row-container tbody').append(data);

            // Show delete last day button
            $('#delete-last-day').show();
            // Enable onchange shift template event for new row            
            enableOnChangeEvent();
        },
        error: function (request, error) {
            alert(JSON.stringify(request));
        }
    });
}

function removeShiftRow() {
    // If only one shift row remaining, hide delete last day button
    if ($('#shift-row-container tr').length <= 3) {
        $('#delete-last-day').hide();
    }
    // Remove last shift row from table
    $('#shift-row-container tr').last().remove();
}

/**
 * This Ajax action submits the form with all shifts to the controller for saving
 */
function saveScheduleShifts() {
    // serialize the form into object
    var form = $('#scheduleform').serializeObject();
    // Parse WorkPeriod fields into correct format for model
    form["WorkPeriods"] = parseWorkPeriodFields();

    // Send form to controller with Ajax and return the partial view to display
    $.post({
        url: SaveScheduleURL,
        traditional: true,
        contentType: "application/json",
        data: JSON.stringify(form),
        success: function (data) {
            $('#shifts-table').html(data);
            // Disable fields of "Day Off" shifts on load
            updateShiftTemplateDisabled();
            // Enable onchange events for shift template dropdown
            enableOnChangeEvent();
        },
        error: function (request, error) {
            alert(JSON.stringify(request));
        }
    });
}

// This function parses all the workperiod shift rows into proper model format for controller
function parseWorkPeriodFields() {
    var workperiods = [];
    $('.shift-row-item').each(function (k) {
        var workperiod = { "StartTime": $('#StartTime-' + k).val(), "EndTime": $('#EndTime-' + k).val(), "IsDayOff": ($('#IsDayOff-' + k).is(':checked')) ? true : false }
        workperiods.push(workperiod);
    });
    return workperiods;
}

// Seralize object function
$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name] !== undefined) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};