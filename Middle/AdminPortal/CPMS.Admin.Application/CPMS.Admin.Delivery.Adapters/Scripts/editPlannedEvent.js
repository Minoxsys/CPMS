$(document).ready(function () {

    $('#PlannedEventTable').jtable({
        title: 'The Planned Event List',
        paging: true,
        pageSize: 10,
        sorting: true,
        actions: {
            listAction: $("#PlannedEventListUrl").val(),
            updateAction: $("#UpdatePlannedEventUrl").val(),
        },
        fields: {
            PlannedEventId: {
                key: true,
                edit: false,
                list: false
            },
            ParentEventCode: {
                title: 'Parent Event',
                edit: false
            },
            PlannedEventCode: {
                title: 'Planned Event',
                edit: false
            },
            EventForDateReferenceForTarget: {
                title: 'Event Used For Target Date',
                edit: false
            },
            IsMandatory: {
                title: 'Mandatory',
                edit: false
            },
            TargetNumberOfDays: {
                title: 'Target'
            },
            ClockType: {
                title: 'Clock',
                edit: false,
                sorting:false
            }
        },
        recordsLoaded: function (e, data) {
            var thList = $("#PlannedEventTable .jtable th");
            var widths = ["25%", "25%", "25%", "10%", "7%", "7%","1%"];
            for (var i = 0; i < thList.length; i++) {
                $(thList[i]).css("width", widths[i]);
            }
            var trList = $("#PlannedEventTable .jtable tr.jtable-data-row");
            for (var j = 0; j < trList.length; j++) {
                $(trList[j]).css("height", "60px");
            }
        }
    });

    $('#LoadRecordsButton').click(function (e) {
        e.preventDefault();
        $('#PlannedEventTable').jtable('load', {
            parentEvent: $('#parentEvent').val(),
            plannedEvent: $('#plannedEvent').val(),
            eventForTarget: $('#eventForTarget').val()
        });
    });

    $('#PlannedEventTable').jtable('load');
});
