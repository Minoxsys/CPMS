$(document).ready(function() {

    $('#ManageUsersTable').jtable({
        title: 'User List',
        paging: true,
        pageSize: 10,
        sorting: true,
        defaultSorting: 'Fullname ASC',
        actions: {
            listAction: $("#ListUsersUrl").val(),
            updateAction: $("#UpdateUserUrl").val(),
            createAction: $("#AddUserUrl").val()
        },
        fields: {
            Id: {
                key: true,
                create: false,
                edit: false,
                list: false
            },
            FullName: {
                title: 'Full Name',
                inputClass: 'validate[required]'
            },
            Username: {
                title: 'Username',
                inputClass: 'validate[required]'
            },
            Email: {
                title: 'Email',
                inputClass: 'validate[required,custom[email]]'
            },
            Password: {
                title: 'Password',
                type: 'password',
                list: false
            },
            ConfirmPassword: {
                title: 'Confirm Password',
                type: 'password',
                list: false
            },
            IsActive: {
                title: 'Status',
                type: 'checkbox',
                values: { 'false': 'Inactive', 'true': 'Active' },
                defaultValue: 'true'
            },
            RoleId: {
                title: 'Role',
                options: $("#GetRolesUrl").val()
            }
        },
        formCreated: function (event, data) {
            data.form.validationEngine();
        },
        formSubmitting: function (event, data) {
            return data.form.validationEngine('validate');
        },
        formClosed: function (event, data) {
            data.form.validationEngine('hide');
            data.form.validationEngine('detach');
        }
    });

    $('#LoadRecordsButton').click(function (e) {
        e.preventDefault();
        $('#ManageUsersTable').jtable('load', {
            fullname: $('#fullname').val(),
            username: $('#username').val()
        });
    });

    $('#ManageUsersTable').jtable('load');

});