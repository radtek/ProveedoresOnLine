/*ReportObject*/
var ReportObject = {

    ObjectId: '',
    ObjectDetailId: '',
    DateFormat: '',
    ReportSelected: '',
    ProviderOptions: new Array(),
    TypeInfo: '',

    Init: function (vInitObject) {
        this.ObjectId = vInitObject.ObjectId;
        this.ObjectDetailId = vInitObject.ObjectDetailId;
        this.DateFormat = vInitObject.DateFormat;
        this.TypeInfo = vInitObject.TypeInfo;
        if (vInitObject.ProviderOptions != null) {
            $.each(vInitObject.ProviderOptions, function (item, value) {
                ReportObject.ProviderOptions[value.Key] = value.Value;
            });
        }
    },

    RenderAsync: function () {
        if (this.TypeInfo == "2") {
            ReportObject.RenderReportGenered();
        } else {
            ReportObject.RenderReport();
        }

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
                { name: 'ViewEnable', template: $('#' + ReportObject.ObjectId + '_ViewEnablesTemplate').html() },
                { name: 'ShortcutToolTip', template: $('#' + ReportObject.ObjectId + '_ShortcutToolTipTemplate').html() },
            ],
            dataSource: {
                schema: {
                    model: {
                        id: 'ReportId',
                        fields: {
                            ReportId: { editable: false, nullable: false },
                            ReportName: { editable: true },
                            Enable: { editable: true, type: 'boolean', defaultValue: true },
                            User: { editable: false },
                            ReportTypeId: { editable: true }
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
                    create: function (options) {
                        $.ajax({
                            url: BaseUrl.ApiUrl + '/ReportApi?CC_Report_Upsert=true&ReportId=null',
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
                            url: BaseUrl.ApiUrl + '/ReportApi?CC_Report_Upsert=true&ReportId=' + ReportObject.ReportSelected.ReportId,
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
            change: function (e) {
                var selectedRows = this.select();
                for (var i = 0; i < selectedRows.length; i++) {
                    debugger;
                    ReportObject.ReportSelected = this.dataItem(selectedRows[i]);
                    ReportObject.RenderReportDetail();                }
            },
            editable: {
                mode: "popup",
                window: {
                    title: "Sucursales",
                }
            },
            columns: [{
                title: "Acciones",
                width: "120px",
                command: [{
                    name: 'edit',
                    text: 'Editar',
                }],
            }, {
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
        $('#' + ReportObject.ObjectDetailId).kendoGrid({
            editable: true,
            navigatable: true,
            pageable: false,
            scrollable: true,
            selectable: true,
            toolbar: [
                { name: 'create', text: 'Nuevo' },
                { name: 'save', text: 'Guardar' },
                { name: 'cancel', text: 'Descartar' },      
                ],
            dataSource: {
                schema: {
                    model: {
                        id: 'Parent',
                        fields: {
                            ReportId: { editable: false, nullable: true },
                            ReportInfoId: { editable: false, nullable: true },
                            Parent: { editable: false },
                            ReportInfoType: { editable: true },
                            Field: { editable: true },
                            ReportInfoFieldId: { editable: false, nullable: true },
                            Enable: { editable: true, type: 'boolean', defaultValue: true },
                            ReportInfoEnableId: { editable: false, nullable: true },
                            ReportTypeId: { editable: true }
                        }
                    }
                },
                transport: {
                    read: function (options) {
                        $.ajax({
                            url: BaseUrl.ApiUrl + '/ReportApi?CC_ReportInfo=true&ReportId=' + ReportObject.ReportSelected.ReportId,
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
                    create: function (options) {
                        options.data.ReportId = ReportObject.ReportSelected.ReportId;
                        
                        $.ajax({
                            url: BaseUrl.ApiUrl + '/ReportApi?CC_ReportInfo_Upsert=true&parent=null',
                            dataType: 'json',
                            type: 'post',
                            data: {
                                DataToUpsert: kendo.stringify(options.data)
                            },
                            success: function (result) {
                                debugger;
                                options.success(result);
                                Message('success', 'Se agrego un  Campo a ' + ReportObject.ReportSelected.ReportName + '.');
                            },
                            error: function (result) {
                                options.error(result);
                                Message('error', 'Error en la fila con el id ' + result[0].ReportInfoId + '.');
                            }
                        });
                        debugger;
                    },
                    update: function (options) {
                        $.ajax({
                            url: BaseUrl.ApiUrl + '/ReportApi?CC_ReportInfo_Upsert=true&parent=' + options.data.Parent + '&Field=' + ReportObject.ReportSelected.Field + '&ReportInfoType=' + ReportObject.ReportSelected.ReportInfoType,
                            dataType: 'json',
                            type: 'post',
                            data: {
                                DataToUpsert: kendo.stringify(options.data)
                            },
                            success: function (result) {
                                options.success(result);
                                Message('success', 'Se editó el reporte  ' + ReportObject.ReportSelected.ReportName + '.');
                            },
                            error: function (result) {
                                options.error(result);
                                Message('error', 'Error en el reporte ' + ReportObject.ReportSelected.ReportName + '.');
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
                field: 'ReportInfoType',
                title: 'Tipo de Informacion',
                width: '190px',
                template: function (dataItem) {
                    var oReturn = 'Seleccione una opción.';
                    debugger;
                    var infoTypeId = 805;

                    if (ReportObject.ReportSelected.ReportTypeId == "2014002") { //Survey
                        infoTypeId = 12010; //corregir
                    }
                    else if (ReportObject.ReportSelected.ReportTypeId == "2014003") {//Thirdknowlege
                        infoTypeId = 12012;
                    } else { //POL
                        infoTypeId = 805; //corregir
                    }
                    $.each(ReportObject.ProviderOptions[infoTypeId], function (item, value) {                        
                        if (dataItem.ReportInfoType == value.ItemId) {
                            oReturn = value.ItemName;
                        }
                    });

                    return oReturn;
                },
                editor: function (container, options) {
                    
                    var infoTypeId = 805;

                    if (ReportObject.ReportSelected.ReportTypeId == "2014002") { //Survey
                        infoTypeId = 12010;
                    }
                    else if (ReportObject.ReportSelected.ReportTypeId == "2014003") {//Thirdknowlege
                        infoTypeId = 12012;
                    } else { //POL
                        infoTypeId = 805;
                    }

                    $('<input required data-bind="value:' + options.field + '"/>')
                        .appendTo(container)
                        .kendoDropDownList({
                            dataSource: ReportObject.ProviderOptions[infoTypeId],
                            dataTextField: 'ItemName',
                            dataValueField: 'ItemId',
                            optionLabel: 'Seleccione una opción'
                        });
                }
            }
            , {
                field: 'Field',
                title: 'Campo',
                width: '190px',
                template: function (dataItem) {

                    var oReturn = 'Seleccione una opción.';

                    //General Information
                    if (dataItem != null && dataItem.ReportInfoType == "805001") {
                        if (dataItem != null && dataItem.Field != null) {
                            $.each(ReportObject.ProviderOptions[203], function (item, value) {
                                if (dataItem.Field == value.ItemId) {
                                    oReturn = value.ItemName;
                                }
                            });
                        }
                    }

                        //Legal Information
                    else if (dataItem != null && dataItem.ReportInfoType == "805002") {
                        if (dataItem != null && dataItem.Field != null) {
                            $.each(ReportObject.ProviderOptions[601], function (item, value) {
                                if (dataItem.Field == value.ItemId) {
                                    oReturn = value.ItemName;
                                }
                            });
                        }
                    }

                        //Financial Information
                    else if (dataItem != null && dataItem.ReportInfoType == "805003") {
                        if (dataItem != null && dataItem.Field != null) {
                            $.each(ReportObject.ProviderOptions[501], function (item, value) {
                                if (dataItem.Field == value.ItemId) {
                                    oReturn = value.ItemName;
                                }
                            });
                        }
                    }

                        //Commercial Information
                    else if (dataItem != null && dataItem.ReportInfoType == "805004") {
                        if (dataItem != null && dataItem.Field != null) {
                            $.each(ReportObject.ProviderOptions[301], function (item, value) {
                                if (dataItem.Field == value.ItemId) {
                                    oReturn = value.ItemName;
                                }
                            });
                        }
                    }

                        //HSEQ Information
                    else if (dataItem != null && dataItem.ReportInfoType == "805005") {
                        if (dataItem != null && dataItem.Field != null) {
                            $.each(ReportObject.ProviderOptions[701], function (item, value) {
                                if (dataItem.Field == value.ItemId) {
                                    oReturn = value.ItemName;
                                }
                            });
                        }
                    }

                        //Aditional Information
                    else if (dataItem != null && dataItem.ReportInfoType == "805006") {
                        if (dataItem != null && dataItem.Field != null) {
                            $.each(ReportObject.ProviderOptions[1701], function (item, value) {
                                if (dataItem.Field == value.ItemId) {
                                    oReturn = value.ItemName;
                                }
                            });
                        }
                    }

                        //Performance Evaluation
                    else if (dataItem != null && dataItem.ReportInfoType == "805007") {
                        if (dataItem != null && dataItem.Field != null) {
                            $.each(ReportObject.ProviderOptions[1204], function (item, value) {
                                if (dataItem.Field == value.ItemId) {
                                    oReturn = value.ItemName;
                                }
                            });
                        }
                    }

                        //ItemsSurvey
                    else if (dataItem != null && dataItem.ReportInfoType == "120101") {
                        if (dataItem != null && dataItem.Field != null) {
                            $.each(ReportObject.ProviderOptions[1202], function (item, value) {
                                if (dataItem.Field == value.ItemId) {
                                    oReturn = value.ItemName;
                                }
                            });
                        }
                    }

                        //ConfigSurvey
                    else if (dataItem != null && dataItem.ReportInfoType == "120102") {
                        if (dataItem != null && dataItem.Field != null) {
                            $.each(ReportObject.ProviderOptions[1203], function (item, value) {
                                if (dataItem.Field == value.ItemId) {
                                    oReturn = value.ItemName;
                                }
                            });
                        }
                    }

                        //SurveyAssigned
                    else if (dataItem != null && dataItem.ReportInfoType == "120103") {
                        if (dataItem != null && dataItem.Field != null) {
                            $.each(ReportObject.ProviderOptions[1204], function (item, value) {
                                if (dataItem.Field == value.ItemId) {
                                    oReturn = value.ItemName;
                                }
                            });
                        }
                    }

                        //SurveyFinished
                    else if (dataItem != null && dataItem.ReportInfoType == "120104") {
                        if (dataItem != null && dataItem.Field != null) {
                            $.each(ReportObject.ProviderOptions[1205], function (item, value) {
                                if (dataItem.Field == value.ItemId) {
                                    oReturn = value.ItemName;
                                }
                            });
                        }
                    }

                        //Thirdknowlege Info
                    else if (dataItem != null && dataItem.ReportInfoType == "1201201") {
                        if (dataItem != null && dataItem.Field != null) {
                            $.each(ReportObject.ProviderOptions[12011], function (item, value) {
                                if (dataItem.Field == value.ItemId) {
                                    oReturn = value.ItemName;
                                }
                            });
                        }
                    }


                    return oReturn;
                },
                editor: function (container, options) {

                    //General Information
                    if (options.model.ReportInfoType == "805001") {


                        $('<input required data-bind="value:' + options.field + '"/>')
                       .appendTo(container)
                       .kendoDropDownList({
                           dataSource: ReportObject.ProviderOptions[203],
                           dataTextField: 'ItemName',
                           dataValueField: 'ItemId',
                           optionLabel: 'Seleccione una opción'
                       });
                    }

                        ///Legal Information
                    else if (options.model.ReportInfoType == "805002") {


                        $('<input required data-bind="value:' + options.field + '"/>')
                       .appendTo(container)
                       .kendoDropDownList({
                           dataSource: ReportObject.ProviderOptions[601],
                           dataTextField: 'ItemName',
                           dataValueField: 'ItemId',
                           optionLabel: 'Seleccione una opción'
                       });
                    }

                        //Financial Information
                    else if (options.model.ReportInfoType == "805003") {


                        $('<input required data-bind="value:' + options.field + '"/>')
                       .appendTo(container)
                       .kendoDropDownList({
                           dataSource: ReportObject.ProviderOptions[501],
                           dataTextField: 'ItemName',
                           dataValueField: 'ItemId',
                           optionLabel: 'Seleccione una opción'
                       });
                    }

                        //Commercial Information
                    else if (options.model.ReportInfoType == "805004") {


                        $('<input required data-bind="value:' + options.field + '"/>')
                       .appendTo(container)
                       .kendoDropDownList({
                           dataSource: ReportObject.ProviderOptions[301],
                           dataTextField: 'ItemName',
                           dataValueField: 'ItemId',
                           optionLabel: 'Seleccione una opción'
                       });
                    }

                        //HSEQ Information

                    else if (options.model.ReportInfoType == "805005") {


                        $('<input required data-bind="value:' + options.field + '"/>')
                       .appendTo(container)
                       .kendoDropDownList({
                           dataSource: ReportObject.ProviderOptions[701],
                           dataTextField: 'ItemName',
                           dataValueField: 'ItemId',
                           optionLabel: 'Seleccione una opción'
                       });
                    }

                        //Aditional Information
                    else if (options.model.ReportInfoType == "805006") {


                        $('<input required data-bind="value:' + options.field + '"/>')
                       .appendTo(container)
                       .kendoDropDownList({
                           dataSource: ReportObject.ProviderOptions[1701],
                           dataTextField: 'ItemName',
                           dataValueField: 'ItemId',
                           optionLabel: 'Seleccione una opción'
                       });
                    }

                        //Performance Evaluation
                    else if (options.model.ReportInfoType == "805007") {


                        $('<input required data-bind="value:' + options.field + '"/>')
                       .appendTo(container)
                       .kendoDropDownList({
                           dataSource: ReportObject.ProviderOptions[1204],
                           dataTextField: 'ItemName',
                           dataValueField: 'ItemId',
                           optionLabel: 'Seleccione una opción'
                       });
                    }


                        //ItemSurvey
                    else if (options.model.ReportInfoType == "120101") {


                        $('<input required data-bind="value:' + options.field + '"/>')
                       .appendTo(container)
                       .kendoDropDownList({
                           dataSource: ReportObject.ProviderOptions[1202],
                           dataTextField: 'ItemName',
                           dataValueField: 'ItemId',
                           optionLabel: 'Seleccione una opción'
                       });
                    }

                        //ConfigSurvey
                    else if (options.model.ReportInfoType == "120102") {


                        $('<input required data-bind="value:' + options.field + '"/>')
                       .appendTo(container)
                       .kendoDropDownList({
                           dataSource: ReportObject.ProviderOptions[1203],
                           dataTextField: 'ItemName',
                           dataValueField: 'ItemId',
                           optionLabel: 'Seleccione una opción'
                       });
                    }

                        //SuervyAssigned
                    else if (options.model.ReportInfoType == "120103") {


                        $('<input required data-bind="value:' + options.field + '"/>')
                       .appendTo(container)
                       .kendoDropDownList({
                           dataSource: ReportObject.ProviderOptions[1204],
                           dataTextField: 'ItemName',
                           dataValueField: 'ItemId',
                           optionLabel: 'Seleccione una opción'
                       });
                    }
                        //SuervyFinished
                    else if (options.model.ReportInfoType == "120104") {


                        $('<input required data-bind="value:' + options.field + '"/>')
                       .appendTo(container)
                       .kendoDropDownList({
                           dataSource: ReportObject.ProviderOptions[1205],
                           dataTextField: 'ItemName',
                           dataValueField: 'ItemId',
                           optionLabel: 'Seleccione una opción'
                       });
                    }

                        //Thirdknowledge info
                    else if (options.model.ReportInfoType == "1201201") {


                        $('<input required data-bind="value:' + options.field + '"/>')
                       .appendTo(container)
                       .kendoDropDownList({
                           dataSource: ReportObject.ProviderOptions[12011],
                           dataTextField: 'ItemName',
                           dataValueField: 'ItemId',
                           optionLabel: 'Seleccione una opción'
                       });
                    }
                    else {

                        $('<input required data-bind="value:' + options.field + '"/>')
                       .appendTo(container)
                       .kendoDropDownList({
                           dataSource: ReportObject.ProviderOptions[1204],
                           dataTextField: 'ItemName',
                           dataValueField: 'ItemId',
                           optionLabel: 'Seleccione una opción'
                       });
                    }


                }
            }, {
                field: 'ReportInfoId',
                title: 'Id Interno',
                width: '78px',
            }],
        });
    },

    RenderReportGenered: function () {
        $('#' + ReportObject.ObjectId).kendoGrid({
            editable: true,
            navigatable: true,
            pageable: false,
            scrollable: true,
            selectable: true,
            toolbar: [
               
            ],
            dataSource: {
                schema: {
                    model: {
                        id: 'ReportId',
                        fields: {

                            FechaInicio: { editable: false, nullable: false },
                            ReportId: { editable: false, nullable: false },
                            ReportName: { editable: false },
                            Enable: { editable: true, type: 'boolean', defaultValue: true },
                            User: { editable: false },
                            ReportTypeId: { editable: false }
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
                    create: function (options) {
                        $.ajax({
                            url: BaseUrl.ApiUrl + '/ReportApi?CC_Report_Upsert=true&ReportId=null',
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
                            url: BaseUrl.ApiUrl + '/ReportApi?CC_Report_Upsert=true&ReportId=' + ReportObject.ReportSelected.ReportId,
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
                title: "Acción",
                template: function(dataItem){
                    return '<a id="dialogRefId" type="button" value="Generar" href="javascript:ParamDialog(\'' + ReportObject.ObjectId + '\',\'' + dataItem.ReportTypeId + '\');">Generar</a>';

                        },
                width: 150
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




function ParamDialog(IdDiv, obj) {
    debugger;
    if (obj == 2014003) {

        $("#divGridReport_GeneratedTK_Template").show();
        $("#divGridReport_GeneratedTK_Template").dialog({ title: "Parametros del Reporte" });
    }

    else if (obj == 2014002) {

        $("#divGridReport_GeneratedEV_Template").show();
        $("#divGridReport_GeneratedEV_Template").dialog({ title: "Parametros del Reporte" });
    }

    else {
        $("#divGridReport_GeneratedPOL_Template").show();
        $("#divGridReport_GeneratedPOL_Template").dialog({ title: "Parametros del Reporte" });
    }
   
}