$(document).ready(function () {
    var url = window.location.search;

    if (url.includes('unlisted')) {
        loadDataTable('unlisted');
    }
    else if (url.includes('listed')) {
        loadDataTable('listed');
    }
    else {
        loadDataTable('all');
    }
});

function loadDataTable(status) {
    dataTable = $('#categoryTbl').DataTable({
        "responsive": true,
        "ajax": {
            "url": "/Category/GetAll?status=" + status
        },
        "columns": [
            { "data": "name", "width": "25%" },
            {
                "data": "isListed",
                "render": function (data) {
                    if (data)
                        return `<span class="btn btn-success w-100">Listed</span>`;
                    else
                        return `<span class="btn btn-danger w-100">Unlisted</span>`;
                },
                "width": "10%"
            },
            { "data": "discount", "width": "10%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                    <div class="btn-group w-100" role="group">
                        <a class="btn btn-sm btn-secondary mx-1" href="/Category/Edit?id=${data}">Edit</a>
                        <a href="/Category/ToggleListing?id=${data}" class="btn btn-danger mx-1">Toggle Status</a>
                        <a href="/Category/AddOffer?id=${data}" class="btn btn-info mx-1">Modify Offer</a>
                        <a onClick="Delete('/Category/Delete?id=${data}')" class="btn btn-outline-danger mx-1">Delete</a>
                    `;
                },
                "width": "55%"
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
                        'Category have been deleted!',
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