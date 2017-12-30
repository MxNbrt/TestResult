angular.module('FormUtils', [
   'dx'
]);

function GetFormObject(data) {
    return {
        colCount: 4,
        formData: data,
        readOnly: true,
        items: [
            {
                dataField: "AppArea",
                label: { text: "Programm" }
            },
            {
                dataField: "Alias",
                label: { text: "Alias" }
            },
            {
                dataField: "DbType",
                label: { text: "Datenbank" }
            },
            {
                dataField: "ServerName",
                label: { text: "Servername" }
            },
            {
                dataField: "BuildDate",
                label: { text: "Builddatum" },
                editorType: "dxDateBox",
                editorOptions: { displayFormat: "dd.MM.yyyy" }
            },
            {
                dataField: "StartTime",
                label: { text: "Testdatum" },
                editorType: "dxDateBox",
                editorOptions: { displayFormat: "dd.MM.yyyy" },
                colspan: 2
            },
            {
                dataField: "StartTime",
                label: { text: "Startzeit" },
                editorType: "dxDateBox",
                editorOptions: { type: "time", displayFormat: "HH:mm" }
            },
            {
                dataField: "Duration",
                label: { text: "Laufzeit" }
            },
            {
                itemType: "empty"
            }
        ]
    };
};