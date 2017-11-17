angular.module('FormUtils', [
    'GridUtils',
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
                editorType: "dxDateBox",
                label: { text: "Builddatum" },
                editorOptions: { displayFormat: "dd.MM.yyyy" }
            },
            {
                dataField: "StartTime",
                editorType: "dxDateBox",
                label: { text: "Testdatum" },
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
                dataField: "EndTime",
                label: { text: "Endzeit" },
                editorType: "dxDateBox",
                editorOptions: { type: "time", displayFormat: "HH:mm" }
            },
            {
                itemType: "empty"
            }
        ]
    };
};