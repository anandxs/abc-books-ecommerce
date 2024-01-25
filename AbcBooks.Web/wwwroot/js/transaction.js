$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#transactionTbl').DataTable({
        "ajax": {
            "url": "/Transaction/GetAll"
        },
        "columns": [
            { "data": "id", "width": "25%" },
            { "data": "amount", "width": "25%" },
            { "data": "transactionType", "width": "25%" },
            {
                "data": "transactionDate",
                "render": function (data) {
                    return `${new Date(data).toLocaleDateString(
                        'en-us',
                        { year: "numeric", month: "short", day: "numeric" })}`;
                },
                "width": "25%"
            }
        ]
    });
}