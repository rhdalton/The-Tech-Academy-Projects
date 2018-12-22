// This function parses edit profile form and passes it to the controller via Ajax

function EditUser() {
    var datastring = JSON.stringify($('#edituserform').serializeObject());

    $.post({
        url: SaveUserDetailsURL,
        contentType: "application/json",
        data: datastring,
        success: function (data) {
            // return data to page
            $("#user-details-content").innerHTML = data;
            // Re-enable client-side model validation after data is returned
            $.validator.unobtrusive.parse("#edituserform");
        },
        error: function (request, error) {
            alert("Request: " + JSON.stringify(request));
        }
    });
}

// This function seralizes the form
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