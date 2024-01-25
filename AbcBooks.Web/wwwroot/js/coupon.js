$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#couponTbl').DataTable({
        "ajax": {
            "url": "/Coupon/GetAll"
        },
        "columns": [
            { "data": "id", "width": "10%" },
            { "data": "couponCode","width": "25" },
            { "data": "discountType.value", "width": "15" },
            {
                "data": "usageLimit",
                "render": function (data) {
                    if (data === null)
                        return "No limits";
                    else
                        return data;
                },
                "width": "25%"
            },
            {
                "data": "expirateDate",
                "render": function (data) {
                    if (data === null)
                        return "No end date";
                    else
                        return data;
                },
                "width": "20%"
            },
            {
                "data": "id",
                "render": function (data) {
                    console.log(data);
                    return `<div>
                        <a class="btn btn-warning" href="/Coupon/Edit?id=${data}">Edit</a>
                        <a class="btn btn-danger" onClick="Delete('/Coupon/Delete?id=${data}')">Delete</a>
                    </div>`;
                },
                "width": "15%"
            },
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (response) {
                    Swal.fire(
                        'Deleted!',
                        'Your file has been deleted.',
                        'success'
                    )
                },
                error: function (xhr, status, error) {
                    Swal.fire(
                        'Error',
                        'Something went wrong!',
                        'failed'
                    )
                }
            });
            location.reload();
        }
    })
}