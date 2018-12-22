// This Function sends filter request to UserController to show specific users in list

function loadUsers(filter) {
    // Set usertable element
    var usertable = $("#indexusertable");
    $.ajax({
        url: IndexUserListURL,
        type: 'GET',
        data: {
            filter: filter
        },
        success: function (data) {
            if (data === undefined || data.length == 0) {
                usertable.html('No users found.');
            } else {
                usertable.html(data);
            }
            // Show which filters are currently set
            if (filter !== undefined) {
                var f = filter.split("-");
                if (f[1] !== undefined) f[0] = f[1];
                $("#viewfilter").show();
                $("#viewfiltertype").html(f[0]);
            }
        },
        error: function (request, error) {
            alert("Request: " + JSON.stringify(request));
        }
    });
};

// Option to clear all filters and show all users
function clearFilters() {
    $('#viewfilter').hide();
    $('#indexSearch').val("")
    loadUsers();
}