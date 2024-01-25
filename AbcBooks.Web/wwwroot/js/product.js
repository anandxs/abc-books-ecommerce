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
    dataTable = $('#productTable').DataTable({
        "ajax": {
            "url": "/Product/GetAll?status=" + status
        },
        "columns": [
            { "data": "id", "width": "5%" },
            {
                "data": "title",
                "render": function (data) {
                    return data
                },
                "width": "20%"
            },
            { "data": "mrp", "width": "7%" },
            { "data": "discount", "width": "10%" },
            { "data": "stock", "width": "10%" },
            { "data": "category.name", "width": "6%" },
            {
                "data": "isListed",
                "render": function (data) {
                    let listingStatus;
                    if (data)
                        listingStatus = `<span class="btn btn-success">Listed</span>`
                    else
                        listingStatus = `<span class="btn btn-danger">Unlisted</span>`

                    return listingStatus;
                },
                "width": "7%"
            },
            {
                "data": "id",
                "render": function (data) {

                    let renderHtml = `<div class="row">
                        <div class="col-12 col-sm-3">
                        <a class="btn btn-sm btn-info w-100" href="/Home/Details?productId=${data}">
							Info</span>
						</a>
                        </div>
                        <div class="col-12 col-sm-3">
                        <a class="btn btn-sm btn-secondary w-100" href="/Product/Edit?id=${data}">
							Edit</span>
						</a>
                        </div>
                        <div class="col-12 col-sm-3">
                        <a class="btn btn-sm btn-danger w-100" href="/Product/ToggleListing?id=${data}">
							Change Status
						</a>
                        </div>
                        <div class="col-12 col-sm-3">
                        <a class="btn btn-sm btn-outline-danger w-100" onClick=Delete('/Product/Delete?id=${data}')>
							Delete
						</a>
                        </div>
                    </div>`

                    return renderHtml;
                },
                "width": "35%"
            }
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
                        'Product have been deleted!',
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