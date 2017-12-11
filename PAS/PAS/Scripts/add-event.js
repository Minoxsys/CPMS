$(document).ready(function () {
    $('#events').dataTable();
    $('#datepicker').datepicker({
        format: "dd/mm/yyyy"
    });

    $('.inputDatePicker').on('change', function () {
        $('.datepicker').hide();
    });
});