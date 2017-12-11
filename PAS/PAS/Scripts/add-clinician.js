$(document).ready(function () {
    $('#clinician-table').dataTable();

    $("#SelectedHospital").change(function () {
        var hospitalId = $("#SelectedHospital").val();
        var url = $("#GetSpecialtiesUrl").val() + "?hospitalId=" + hospitalId;
        $.get(url, function(data) {
            var specialtyList = $("#SelectedSpecialty");
            specialtyList.empty();

            for (var i = 0; i < data.length; i++) {
                var option = new Option(data[i].Code, data[i].Code);
                $(option).html(data[i].DisplayName);
                specialtyList.append(option);
            }
        });
    });
});