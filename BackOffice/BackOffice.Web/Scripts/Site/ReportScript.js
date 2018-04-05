/*ReportObject*/
var ReportObject = {

    ObjectId: '',
    DateFormat: '',
    ReportInfo: null,
    ProviderOptions: new Array(),

    Init: function (vInitObject) {
        this.ObjectId = vInitObject.ObjectId;
        this.DateFormat = vInitObject.DateFormat;
        if (vInitObject.ProviderOptions != null) {
            $.each(vInitObject.ProviderOptions, function (item, value) {
                ReportObject.ProviderOptions[value.Key] = value.Value;
            });
        }
    },

    RenderAsync: function () {
        ReportObject.RenderReport();

        //focus on the grid
        $('#' + ReportObject.ObjectId).data("kendoGrid").table.focus();

        //config keyboard
        ReportObject.ConfigKeyBoard();

        //Config Events
        ReportObject.ConfigEvents();
    },

    ConfigKeyBoard: function () {

        //init keyboard tooltip
        $('.divGrid_kbtooltip').tooltip();

        $(document.body).keydown(function (e) {
            if (e.altKey && e.shiftKey && e.keyCode == 71) {
                //alt+shift+g

                //save
                $('#' + ReportObject.ObjectId).data("kendoGrid").saveChanges();
            }
            else if (e.altKey && e.shiftKey && e.keyCode == 78) {
                //alt+shift+n

                //new field
                $('#' + ReportObject.ObjectId).data("kendoGrid").addRow();
            }
            else if (e.altKey && e.shiftKey && e.keyCode == 68) {
                //alt+shift+d

                //new field
                $('#' + ReportObject.ObjectId).data("kendoGrid").cancelChanges();
            }
        });
    },

    ConfigEvents: function () {

        //config grid visible enables event
        $('#' + ReportObject.ObjectId + '_ViewEnable').change(function () {
            $('#' + ReportObject.ObjectId).data('kendoGrid').dataSource.read();
        });
    },

    GetViewEnable: function () {

        return $('#' + ReportObject.ObjectId + '_ViewEnable').length > 0 ? $('#' + ReportObject.ObjectId + '_ViewEnable').is(':checked') : true;
    },

    RenderReport: function () {
        $('#' + ReportObject.ObjectId).kendoGrid({
            editable: true,
            navigatable: true,
            pageable: false,
            scrollable: true,
            selectable: true,
            toolbar: [
                { name: 'create', text: 'Nuevo' },
                { name: 'save', text: 'Guardar' },
                { name: 'cancel', text: 'Descartar' },
                { name: 'ViewEnable', template: $('#' + ReportObject.ObjectId + '_ViewEnablesTemplate').html() },
                { name: 'ShortcutToolTip', template: $('#' + ReportObject.ObjectId + '_ShortcutToolTipTemplate').html() },
            ],
            dataSource: {
                schema: {
                    model: {
                        id: 'ContactId',
                        fields: {
                            ReportId: { editable: false, nullable: true },
                            ReportName: { editable: true },
                            Enable: { editable: true, type: 'boolean', defaultValue: true },
                            User: { editable: false },
                            ReportTypeId: "ReportType.ItemInfoId"
                        }
                    }
                },
                transport: {
                    read: function (options) {

                        $.ajax({
                            url: BaseUrl.ApiUrl + '/ReportApi?CC_Report=true',
                            dataType: 'json',
                            success: function (result) {
                                options.success(result);
                            },
                            error: function (result) {
                                options.error(result);
                                Message('error', result);
                            }
                        });
                    },
                    create: function (options) {//ToDo: change for Report
                        $.ajax({
                            url: BaseUrl.ApiUrl + '/ProviderApi?GIContactUpsert=true&ProviderPublicId=' + ReportObject.ProviderPublicId + '&ContactType=' + ReportObject.ContactType,
                            dataType: 'json',
                            type: 'post',
                            data: {
                                DataToUpsert: kendo.stringify(options.data)
                            },
                            success: function (result) {
                                options.success(result);
                                Message('success', 'Se creó el registro.');
                            },
                            error: function (result) {
                                options.error(result);
                                Message('error', result);
                            }
                        });
                    },
                    update: function (options) {
                        $.ajax({
                            url: BaseUrl.ApiUrl + '/ProviderApi?GIContactUpsert=true&ProviderPublicId=' + ReportObject.ProviderPublicId + '&ContactType=' + ReportObject.ContactType,
                            dataType: 'json',
                            type: 'post',
                            data: {
                                DataToUpsert: kendo.stringify(options.data)
                            },
                            success: function (result) {
                                options.success(result);
                                Message('success', 'Se editó la fila con el id ' + options.data.ContactId + '.');
                            },
                            error: function (result) {
                                options.error(result);
                                Message('error', 'Error en la fila con el id ' + options.data.ContactId + '.');
                            }
                        });
                    },//ToDo: change for Report
                },
                requestStart: function () {
                    kendo.ui.progress($("#loading"), true);
                },
                requestEnd: function () {
                    kendo.ui.progress($("#loading"), false);
                }
            },
            change: function (e) {
                var selectedRows = this.select();
                for (var i = 0; i < selectedRows.length; i++) {
                    debugger;
                    ReportObject.ReportInfo = this.dataItem(selectedRows[i].configReportInfo);
                    ReportObject.RenderReportDetail();

                    //config grid infro visible enable event
                    $('#' + ReportObject.ObjectId + '_Detail_ViewEnable').change(function () {
                        $('#' + ReportObject.ObjectId + '_Detail').data('kendoGrid').dataSource.read();
                    });
                }
            },
            columns: [{
                field: 'Enable',
                title: 'Habilitado',
                width: '100px',
                template: function (dataItem) {
                    var oReturn = '';

                    if (dataItem.Enable == true) {
                        oReturn = 'Si'
                    }
                    else {
                        oReturn = 'No'
                    }
                    return oReturn;
                },
            }, {
                field: 'ReportName',
                title: 'Nombre',
                width: '180px',
            }, {
                field: 'ReportTypeId',
                title: 'Tipo de Reporte',
                width: '190px',
                template: function (dataItem) {
                    var oReturn = 'Seleccione una opción.';
                    if (dataItem != null && dataItem.ReportTypeId != null) {
                        $.each(ReportObject.ProviderOptions[2014], function (item, value) {
                            if (dataItem.ReportTypeId == value.ItemId) {
                                oReturn = value.ItemName;
                            }
                        });
                    }
                    return oReturn;
                },
                editor: function (container, options) {
                    $('<input required data-bind="value:' + options.field + '"/>')
                        .appendTo(container)
                        .kendoDropDownList({
                            dataSource: ReportObject.ProviderOptions[2014],
                            dataTextField: 'ItemName',
                            dataValueField: 'ItemId',
                            optionLabel: 'Seleccione una opción'
                        });
                },
            }, {
                field: 'User',
                title: 'Usuario',
                width: '180px',
            }, {
                field: 'ReportId',
                title: 'Id Interno',
                width: '78px',
            }],
        });
    },

    RenderReportDetail: function () {
        $('#' + ReportObject.ObjectId).kendoGrid({
            editable: true,
            navigatable: true,
            pageable: false,
            scrollable: true,
            toolbar: [
                { name: 'create', text: 'Nuevo' },
                { name: 'save', text: 'Guardar' },
                { name: 'cancel', text: 'Descartar' },
                { name: 'ViewEnable', template: $('#' + ReportObject.ObjectId + '_ViewEnablesTemplate').html() },
                { name: 'ShortcutToolTip', template: $('#' + ReportObject.ObjectId + '_ShortcutToolTipTemplate').html() },
            ],
            dataSource: {
                schema: {
                    model: {
                        id: 'ContactId',
                        fields: {
                            ReportId: { editable: false, nullable: true },
                            ReportName: { editable: true },
                            Enable: { editable: true, type: 'boolean', defaultValue: true },
                            User: { editable: false },
                            ReportTypeId: "ReportType.ItemInfoId"
                        }
                    }
                },
                transport: {
                    read: function (options) {
                        debugger;
                        /*$.ajax({
                            url: BaseUrl.ApiUrl + '/ReportApi?CC_Report=true',
                            dataType: 'json',
                            success: function (result) {
                                options.success(result);
                            },
                            error: function (result) {
                                options.error(result);
                                Message('error', result);
                            }
                        });*/
                        options.success(ReportObject.ReportInfo);
                    },
                    create: function (options) {
                        $.ajax({
                            url: BaseUrl.ApiUrl + '/ProviderApi?GIContactUpsert=true&ProviderPublicId=' + ReportObject.ProviderPublicId + '&ContactType=' + ReportObject.ContactType,
                            dataType: 'json',
                            type: 'post',
                            data: {
                                DataToUpsert: kendo.stringify(options.data)
                            },
                            success: function (result) {
                                options.success(result);
                                Message('success', 'Se creó el registro.');
                            },
                            error: function (result) {
                                options.error(result);
                                Message('error', result);
                            }
                        });
                    },
                    update: function (options) {
                        $.ajax({
                            url: BaseUrl.ApiUrl + '/ProviderApi?GIContactUpsert=true&ProviderPublicId=' + ReportObject.ProviderPublicId + '&ContactType=' + ReportObject.ContactType,
                            dataType: 'json',
                            type: 'post',
                            data: {
                                DataToUpsert: kendo.stringify(options.data)
                            },
                            success: function (result) {
                                options.success(result);
                                Message('success', 'Se editó la fila con el id ' + options.data.ContactId + '.');
                            },
                            error: function (result) {
                                options.error(result);
                                Message('error', 'Error en la fila con el id ' + options.data.ContactId + '.');
                            }
                        });
                    },
                },
                requestStart: function () {
                    kendo.ui.progress($("#loading"), true);
                },
                requestEnd: function () {
                    kendo.ui.progress($("#loading"), false);
                }
            },
            columns: [{
                field: 'Enable',
                title: 'Habilitado',
                width: '100px',
                template: function (dataItem) {
                    var oReturn = '';

                    if (dataItem.Enable == true) {
                        oReturn = 'Si'
                    }
                    else {
                        oReturn = 'No'
                    }
                    return oReturn;
                },
            }, {
                field: 'ReportName',
                title: 'Nombre',
                width: '180px',
            }, {
                field: 'ReportTypeId',
                title: 'Tipo de Reporte',
                width: '190px',
                template: function (dataItem) {
                    var oReturn = 'Seleccione una opción.';
                    if (dataItem != null && dataItem.ReportTypeId != null) {
                        $.each(ReportObject.ProviderOptions[2014], function (item, value) {
                            if (dataItem.ReportTypeId == value.ItemId) {
                                oReturn = value.ItemName;
                            }
                        });
                    }
                    return oReturn;
                },
                editor: function (container, options) {
                    $('<input required data-bind="value:' + options.field + '"/>')
                        .appendTo(container)
                        .kendoDropDownList({
                            dataSource: ReportObject.ProviderOptions[2014],
                            dataTextField: 'ItemName',
                            dataValueField: 'ItemId',
                            optionLabel: 'Seleccione una opción'
                        });
                },
            }, {
                field: 'User',
                title: 'Usuario',
                width: '180px',
            }, {
                field: 'ReportId',
                title: 'Id Interno',
                width: '78px',
            }],
        });
    },

    
};