function initializeDataTable(tableId) {
    $(document).ready(function () {
        var table = new DataTable(tableId, {
            language: {
                url: '//cdn.datatables.net/plug-ins/2.2.2/i18n/es-ES.json',
            },
            columnDefs: [
                { "className": "dt-left", "targets": "_all" }
            ]
        });
    });
}