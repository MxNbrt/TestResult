angular.module('FormUtils', [
   'dx',
   'GridUtils'
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
                dataField: "Version",
                label: { text: "Version" }
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
                template: function (data, itemElement) {
                    itemElement.append("<div>")
                        .dxTextBox({
                            value: GetDateTimeString(new Date(data.component.option('formData')[data.dataField]), true)
                        });
                }
            },
            {
                dataField: "StartTime",
                label: { text: "Testdatum" },
                template: function (data, itemElement) {
                    itemElement.append("<div>")
                        .dxTextBox({
                            value: GetDateTimeString(new Date(data.component.option('formData')[data.dataField]), true)
                        });
                }
            },
            {
                dataField: "Duration",
                label: { text: "Laufzeit" }
            }
        ]
    };
};

function GetDebugFormObject(searchButtonClick) {
    return {
        colCount: 3,
        labelLocation: 'top',
        onEditorEnterKey: searchButtonClick,
        formData: {
            "TestSuiteName": "",
            "TestCaseName": "",
            "MessageText": "",
            "DbType": "MSSQL",
            "Version": "6.0",
            // default date is now minus one month
            "FromDate": new Date(new Date().getFullYear(), new Date().getMonth() - 1, new Date().getDate())
        },
        items: [
            {
                dataField: "TestSuiteName",
                label: { text: "Suite" }
            },
            {
                dataField: "TestCaseName",
                label: { text: "Case" }
            },
            {
                dataField: "MessageText",
                label: { text: "Fehlertext" }
            },
            {
                dataField: "DbType",
                label: { text: "Datenbank" },
                editorType: "dxSelectBox",
                editorOptions: {
                    items: ["MSSQL", "ORACLE"]
                }
            },
            {
                dataField: "Version",
                label: { text: "Version" },
                editorType: "dxSelectBox",
                editorOptions: {
                    items: ["5.4", "5.5", "6.0"]
                }
            },
            {
                dataField: "FromDate",
                label: { text: "Ab Datum" },
                editorType: "dxDateBox",
                editorOptions: {
                    width: "100%",
                    displayFormat: "dd.MM.yyyy"
                },
                validationRules: [{
                    type: "required",
                    message: "Aus Performancegründen muss ein 'Ab Datum' eingegeben werden."
                }]
            }
        ]
    };
};