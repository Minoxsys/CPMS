$(document).ready(function () {

    $('#EventMilestoneTable').jtable({
        title: 'The Event Milestone List',
        paging: true,
        pageSize: 10,
        sorting: true,
        actions: {
            listAction: $("#EventMilestoneListUrl").val(),
            updateAction: $("#UpdateEventMilestoneUrl").val(),
        },
        fields: {
            EventMilestoneId: {
                key: true,
                edit: false,
                list: false
            },
            ParentEventDescription: {
                title: 'Parent Event',
                edit: false
            },
            EventMilestoneDescription: {
                title: 'Event Milestone',
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
            var thList = $("#EventMilestoneTable .jtable th");
            var widths = ["25%", "25%", "25%", "10%", "7%", "7%","1%"];
            for (var i = 0; i < thList.length; i++) {
                $(thList[i]).css("width", widths[i]);
            }
            var trList = $("#EventMilestoneTable .jtable tr.jtable-data-row");
            for (var j = 0; j < trList.length; j++) {
                $(trList[j]).css("height", "60px");
            }
        }
    });

    $('#LoadRecordsButton').click(function (e) {
        e.preventDefault();
        $('#EventMilestoneTable').jtable('load', {
            parentEvent: $('#parentEvent').val(),
            eventMilestone: $('#eventMilestone').val(),
            eventForTarget: $('#eventForTarget').val()
        });
    });

    $('#EventMilestoneTable').jtable('load');
});
