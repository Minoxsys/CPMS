$(document).ready(function () {
    $('#datepicker').datepicker({
        format: "dd/mm/yyyy"
    });

    $('.inputDatePicker').on('change', function () {
        $('.datepicker').hide();
    });

    $('#patientsTable').dataTable();
});
