$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#wishlistTbl').DataTable({
        "ajax": {
            "url": "/Wishlist/GetAll"
        },
        "columns": [
            { "data": "product.title", "width": "30%" },
            { "data": "product.description","width": "50" },
            {
                "data": "product.id",
                "render": function (data) {
                    let options = `<div>
                        <a class="btn btn-info" href="/Home/Details?productId=${data}">Go To Page</a>
                        <a class="btn btn-danger" href="/Wishlist/RemoveFromWishlist?productId=${data}">Remove From Wishlist</a>
                    </div>`;

                    return options;
                },
                "width": "20"
            }
        ]
    });
}