$(document).ready(function () {
    var url = window.location.search;

    if (url.includes('returns')) {
        loadDataTable('returns');
    }
    else if (url.includes('inprocess')) {
        loadDataTable('inprocess');
    }
    else if (url.includes('completed')) {
        loadDataTable('completed');
    }
    else if (url.includes('cancelled')) {
        loadDataTable('cancelled');
    }
    else if (url.includes('shipped')) {
        loadDataTable('shipped');
    }
    else if (url.includes('pending')) {
        loadDataTable('pending');
    }
    else {
        loadDataTable('all');
    }
});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Order/GetAll?status=" + status
        },
        "columns": [
            { "data": "id", "width": "5%" },
            {
                "data": "shippingAddress",
                "render": function (data) {
                    return data.firstName + " " + data.lastName;
                },
                "width": "20%"
            },
            { "data": "shippingAddress.phoneNumber", "width": "15%" },
            { "data": "applicationUser.email", "width": "15%" },
            { "data": "orderStatus", "width": "15%" },
            {
                "data": "orderDate",
                "render": function (data) {
                    return `${new Date(data).toLocaleDateString(
                        'en-us',
                        { year: "numeric", month: "short", day: "numeric" })}`;
                },
                "width": "10%"
            },
            {
                "data": "orderTotal",
                "render": function (data) {
                    return `${data.toFixed(2)}`
                },
                "width": "10%"
            },
            {
                "data": "id",
                "render": function (data) {

                    return `<div class="w-75 btn-group" role="group">
                                <a href="/Order/Details?orderId=${data}" class="btn btn-outline-dark">Details</a>
                            </div>`;
                },
                "width": "10%"
            }
        ]
    });
}