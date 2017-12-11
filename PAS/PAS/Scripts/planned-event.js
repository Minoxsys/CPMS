$(document).ready(function() {
    $('#planned-events-table').dataTable().makeEditable({
        sUpdateURL: $("#UpdatePlannedEvent").val(),
        "aoColumns": [
            null,
            null,
            null,
            {
                placeholder: ""
            },
            null
        ],
        fnShowError: function(message, action) {
            switch (action) {
                case "update":
                    break;
            }
        }
    });
});