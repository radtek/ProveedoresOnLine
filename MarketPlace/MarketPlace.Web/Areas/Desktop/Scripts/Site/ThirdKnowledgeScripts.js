/*Init survey program object*/
var Third_KnowledgeSimpleSearchObject = {
    ObjectId: '',
    Url: '',
    ReSearch: false,
    QueryDetailsRoleOption: '',
    Init: function (vInitObject) {
        this.ObjectId = vInitObject.ObjectId;
        this.Url = vInitObject.Url;
        this.ReSearch = vInitObject.ReSearch;
        this.QueryDetailsRoleOption = vInitObject.QueryDetailsRoleOption;

        if (this.ReSearch == "True") {
            //Call Research Function
            Third_KnowledgeSimpleSearchObject.SimpleSearch();
        }
    },

    SimpleSearch: function () {        
        Third_KnowledgeSimpleSearchObject.Loading_Generic_Show();
        if ($('#' + Third_KnowledgeSimpleSearchObject.ObjectId + '_Form').length > 0) {
            $('#' + Third_KnowledgeSimpleSearchObject.ObjectId + '_DivResult').html('')
            var validator = $('#' + Third_KnowledgeSimpleSearchObject.ObjectId + '_EditProjectDialog_Form').data("kendoValidator");

            $.ajax({
                type: "POST",
                url: $('#' + Third_KnowledgeSimpleSearchObject.ObjectId + '_Form').attr('action'),
                data: $('#' + Third_KnowledgeSimpleSearchObject.ObjectId + '_Form').serialize(),
                success: function (result) {                    
                    //Set param to send report
                    $('#' + Third_KnowledgeSimpleSearchObject.ObjectId + '_FormQueryPublicId').val(result.RelatedThidKnowledgeSearch.CollumnsResult.QueryPublicId);
                    $('#' + Third_KnowledgeSimpleSearchObject.ObjectId + '_showreport').show();                    

                    Third_KnowledgeSimpleSearchObject.Loading_Generic_Hidden();
                    $('#' + Third_KnowledgeSimpleSearchObject.ObjectId + '_DivResult').html('')
                    var tittlestDiv = '';
                    var resultDiv = '';
                    debugger;
                    if (result.TKGroupByListViewModel != null && result.TKGroupByListViewModel.length > 0) {

                        $.each(result.TKGroupByListViewModel, function (item, value) {
                            debugger;
                            if (value.m_Item1 == "INFORMACIÓN BÁSICA") {
                                resultDiv = '';
                                resultDiv += '<div class="row">' +
                                '<div class="col-sm-12 col-lg-12 POMPTKDetailTitle"><strong>INFORMACIÓN BÁSICA ' + value.m_Item2 + '</strong></div>' +
                                '</div>' +
                                '<br />' +
                                '<div class="row">' +
                                '<div class="col-sm-4 POMPProviderBoxInfo text-left"><strong>Nombre Encontrado</strong></div>' +
                                '<div class="col-sm-4 POMPProviderBoxInfo text-left"><strong>Identificación Consultada</strong></div>' +
                                '</div><br />';
                                resultDiv += '<div class="row POMPBorderbottom">';
                                $.each(value.m_Item2, function (item, value) {
                                    resultDiv += '<div class="col-sm-4 POMPProviderBoxInfo text-left"><p>';
                                    resultDiv += value.RequestName + '</p></div>';
                                    resultDiv += '<div class="col-sm-4 POMPProviderBoxInfo text-left"><p>';
                                    resultDiv += value.IdNumberRequest + '</p></div>';
                                    resultDiv += '  <div class="col-sm-4 POMPProviderBoxInfo text-right"><p>' +
                                        '<a target = "_blank" href="' + BaseUrl.SiteUrl + 'ThirdKnowledge/TKDetailSingleSearch?QueryPublicId=' + value.QueryPublicId + '&QueryBasicPublicId=' + value.QueryPublicId + '&ElasticId=' + value.ElasticId +
                                                '">Ver Detalle</a>' +
                                                '</p></div></div> <br /> <br /> <br />';
                                })
                                $('#' + Third_KnowledgeSimpleSearchObject.ObjectId + '_DivResult').append(resultDiv);
                            }
                            else {
                                resultDiv = '';
                                tittlestDiv = '<div class="col-sm-1 col-lg-1 POMPProviderBoxInfo"><strong>Prioridad</strong></div>'                                            
                                            + '<div class="col-sm-3 col-lg-3 POMPProviderBoxInfo"><strong>Nom. Consultado</strong></div>'
                                            + '<div class="col-sm-1 col-lg-1 POMPProviderBoxInfo"><strong>Id.Consultada</strong></div>'
                                            + '<div class="col-sm-3 col-lg-3 POMPProviderBoxInfo"><strong>Nom. Encontrado</strong></div>'
                                            + '<div class="col-sm-1 col-lg-1 POMPProviderBoxInfo"><strong>Id.Encontrada</strong></div>'
                                            + '<div class="col-sm-2 col-lg-2 POMPProviderBoxInfo"><strong>Nombre Lista</strong></div>'
                                            + '<div class="col-sm-2 col-lg-1 POMPProviderBoxInfo"></div>';


                                resultDiv += '<div class="row text-center">';
                                resultDiv += '<div id="POMPSSResultName" class="col-xs-12 text-left"  style="padding-top:7px"><strong>' + value.m_Item1 + '</strong></div>'
                                resultDiv += '</div>';
                                resultDiv += '<div class="conatiner-fluid POMPTKDetailContainer">';
                                resultDiv += '<div class="row text-center">';
                                resultDiv += '<br/>';
                                resultDiv += tittlestDiv;
                                resultDiv += '</div>';
                                resultDiv += '</div>';
                                resultDiv += '<br/>';
                                resultDiv += '<div class="conatiner-fluid POMPTKDetailContainer">';
                                $.each(value.m_Item2, function (item, value) {
                                    resultDiv += '<div class="row text-center">';
                                    if (value.Priority != null) {
                                        resultDiv += '<div class="col-sm-1 POMPProviderBoxInfo">' + value.Priority + '</div>';
                                    }                                  

                                    if (value.RequestName != null) {
                                        resultDiv += '<div class="col-sm-3 POMPProviderBoxInfo">' + value.RequestName + '</div>';
                                    }

                                    if (value.IdNumberRequest != null) {
                                        resultDiv += '<div class="col-sm-1 POMPProviderBoxInfo">' + value.IdNumberRequest + '</div>';
                                    }

                                    if (value.NameResult != null) {
                                        resultDiv += '<div class="col-sm-3 POMPProviderBoxInfo">' + value.NameResult + '</div>';
                                    }
                                    if (value.IdentificationNumberResult != null) {
                                        resultDiv += '<div class="col-sm-1  POMPProviderBoxInfo">' + value.IdentificationNumberResult + '</div>';
                                    }
                                    if (value.ListName != null) {
                                        resultDiv += '<div class="col-sm-2 POMPProviderBoxInfo">' + value.ListName + '</div>';
                                    }                                    
                                    if (value.QueryPublicId  != null && Third_KnowledgeSimpleSearchObject.QueryDetailsRoleOption == 'True') {                                        
                                        resultDiv += '<div class="col-sm-1 POMPProviderBoxInfo">' + '<a target = "_blank" href="' + Third_KnowledgeSimpleSearchObject.Url + '?QueryPublicId=' + value.QueryPublicId + '&QueryBasicPublicId=' + value.QueryBasicPublicId +'&ElasticId='+ value.ElasticId  + '&ReturnUrl=null">' + "Ver_Detalle" + '</a>' + '</div>';
                                    }                                                                        
                                    resultDiv += '</div>';
                                    resultDiv += '<div class="row text-center">';
                                    resultDiv += '<hr class="Tk-DetailSingleSearchSeparator"/>';
                                    resultDiv += '</div>';
                                });
                                resultDiv += '</div><br/><br/>';

                                $('#' + Third_KnowledgeSimpleSearchObject.ObjectId + '_DivResult').append(resultDiv);
                            }
                        });
                    }
                    else {
                        resultDiv = '<div class="row"><div class="col-md-11 col-sm-11 text-center"><br/><br/><br/><label>LA BÚSQUEDA</label><br/><label class="POMPNoresultText">no arrojó coincidencias</label></div></div>',
                        $('#' + Third_KnowledgeSimpleSearchObject.ObjectId + '_DivResult').append(resultDiv);
                    }

                    $('#' + Third_KnowledgeSimpleSearchObject.ObjectId + '_Queries').html('');
                    $('#' + Third_KnowledgeSimpleSearchObject.ObjectId + '_Queries').append(result.RelatedThirdKnowledge.CurrentPlanModel.RelatedPeriodModel[0].TotalQueries);

                },
                error: function (result) {
                    Third_KnowledgeSimpleSearchObject.Loading_Generic_Hidden();
                },
            })
        }
    },

    Loading_Generic_Show: function () {
        kendo.ui.progress($("#loading"), true);
    },

    Loading_Generic_Hidden: function () {
        kendo.ui.progress($("#loading"), false);
    },    
};

var Third_KnowledgeMasiveSearchObject = {
    ObjectId: '',
    CompanyPublicId: '',
    PeriodPublicId: '',

    Init: function (vInitObject) {
        this.ObjectId = vInitObject.ObjectId
        this.CompanyPublicId = vInitObject.CompanyPublicId
        this.PeriodPublicId = vInitObject.PeriodPublicId,
        Third_KnowledgeMasiveSearchObject.LoadFile();
    },

    LoadFile: function () {
        $('#' + Third_KnowledgeMasiveSearchObject.ObjectId + '_FileUpload').kendoUpload({
            multiple: false,
            localization: {
                "select": "Selecionar Archivos"
            },
            async: {
                saveUrl: BaseUrl.ApiUrl + '/ThirdKnowledgeApi?TKLoadFile=true&CompanyPublicId=' + Third_KnowledgeMasiveSearchObject.CompanyPublicId + '&PeriodPublicId=' + Third_KnowledgeMasiveSearchObject.PeriodPublicId,
                autoUpload: true
            },
            success: function (e) {
                $('#' + Third_KnowledgeMasiveSearchObject.ObjectId + '_MessageResult').html('');
                $('#' + Third_KnowledgeMasiveSearchObject.ObjectId + '_MessageResult').append(e.response.LoadMessage);
                $('#' + Third_KnowledgeMasiveSearchObject.ObjectId + '_Queries').html('');
                $('#' + Third_KnowledgeMasiveSearchObject.ObjectId + '_Queries').append(e.response.AdditionalInfo);

                Header_NewNotification();
            },
            error: function (e) {
            }
        });
    },
};

var Third_KnowledgeDetailSearch = {
    QueryPublicId: ''
    , PageNumber: 0
    , RowCount: 0
    , SearchUrl: ''
    , InitDate: ''
    , EndDate: ''
    , Enable: ''
    , IsSuccess: ''


    , Init: function (vInitObject) {
        this.ObjectId = vInitObject.ObjectId;
        this.SearchUrl = vInitObject.SearchUrl;
        this.QueryPublicId = vInitObject.QueryPublicId;
        this.InitDate = vInitObject.InitDate;
        this.EndDate = vInitObject.EndDate;
        this.PageNumber = vInitObject.PageNumber;
        this.RowCount = vInitObject.RowCount;
        this.Enable = vInitObject.Enable;
        this.IsSuccess = vInitObject.IsSuccess;

    },

    RenderAsync: function () {
        //Change event

        $('#' + Third_KnowledgeDetailSearch.ObjectId + '_FilterId').click(function () {
            Third_KnowledgeDetailSearch.Search(vInitObject);
        });
    },

    Search: function (vSearchObject) {
        var oUrl = this.SearchUrl + '?QueryPublicId=' + vSearchObject.QueryPublicId;
        oUrl += '&InitDate=' + '';
        oUrl += '&EndDate=' + '';

        if (vSearchObject != null && vSearchObject.PageNumber != null) {
            oUrl += '&PageNumber=' + vSearchObject.PageNumber;
        }
        window.location = oUrl;
    },
}

var Third_KnowledgeSearch = {
    CustomerPublicId: ''
    , PageNumber: 0
    , RowCount: 0
    , SearchUrl: ''
    , InitDate: ''
    , EndDate: ''
    , SearchType: ''
    , Status: ''

    , Init: function (vInitObject) {        
        this.ObjectId = vInitObject.ObjectId;
        this.SearchUrl = vInitObject.SearchUrl;
        this.CustomerPublicId = vInitObject.CustomerPublicId;
        this.InitDate = vInitObject.InitDate;
        this.EndDate = vInitObject.EndDate;
        this.PageNumber = vInitObject.PageNumber;
        this.RowCount = vInitObject.RowCount;
        this.SearchType = vInitObject.SearchType;
        this.Status = vInitObject.Status;
    },
    RenderAsync: function () {
        //Change event
        $('#' + Third_KnowledgeSearch.ObjectId + '_FilterId').click(function () {
            Third_KnowledgeSearch.Search(null);
        });
    },
    Search: function (vSearchObject) {
        if (vSearchObject != null) {
            var oUrl = this.SearchUrl + '?';
            if (vSearchObject != null && vSearchObject.PageNumber != null)
                oUrl += '&PageNumber=' + vSearchObject.PageNumber;
            else
                oUrl += '&PageNumber=' + '';
            if (vSearchObject != null && vSearchObject.InitDate != null)
                oUrl += '&InitDate=' + vSearchObject.InitDate;
            else
                oUrl += '&InitDate=' + '';
            if (vSearchObject != null && vSearchObject.EndDate != null)
                oUrl += '&EndDate=' + vSearchObject.EndDate;
            else
                oUrl += '&EndDate=' + '';
            if (vSearchObject != null && vSearchObject.SearchType != null)
                oUrl += '&SearchType=' + vSearchObject.SearchType;
            else
                oUrl += '&SearchType=' + '';
            if (vSearchObject != null && vSearchObject.Status != null)
                oUrl += '&Status=' + vSearchObject.Status;
            else
                oUrl += '&Status=' + '';
            window.location = oUrl;
        }
        else {
            var oUrl = this.SearchUrl;
            oUrl += '?InitDate=' + $('#' + Third_KnowledgeSearch.ObjectId + '_InitDateId').val();
            oUrl += '&EndDate=' + $('#' + Third_KnowledgeSearch.ObjectId + '_EndDateId').val();
            oUrl += '&SearchType=' + $('#' + Third_KnowledgeSearch.ObjectId + '_QueryType').val();
            oUrl += '&Status=' + $('#' + Third_KnowledgeSearch.ObjectId + '_QueryStatus').val();
            oUrl += '&PageNumber=0';
            window.location = oUrl;
        }
    },

    Third_Knowledge_ReSearchMasive: function (vReSearchObj) {
        Third_KnowledgeSimpleSearchObject.Loading_Generic_Show();
        $.ajax({
            type: "POST",
            url: BaseUrl.ApiUrl + '/ThirdKnowledgeApi?TKReSearchMasive=true&CompanyPublicId=' + vReSearchObj.CustomerPublicId + '&PeriodPublicId=' + vReSearchObj.PediodPublicId + '&FileName=' + vReSearchObj.FileName,
            success: function (result) {
                Third_KnowledgeSimpleSearchObject.Loading_Generic_Hidden();
                Dialog_ShowMessage("Carga Exitosa", "El archivo es correcto, en unos momentos recibirá un correo con el respectivo resultado de la validación.", window.location.href);

            },
            error: function (result) {
                Third_KnowledgeSimpleSearchObject.Loading_Generic_Hidden();
                Dialog_ShowMessage("Error", "Ocurrió un problema al realizar la consulta, por favor verifique su conexión a internet.", window.location.href);
            },
        })
    }
}

var ThirdKnowledge_ReportViewerObj = {    
    RenderReportViewer: function (vInitObject) {
        var cmbToAppend = '<center><span>Formatos disponibles<span>:&nbsp;&nbsp;<select name= "' + vInitObject.ObjectId + '_cmbFormat">';
        $.each(vInitObject.Options, function (item, value) {
            if (value == "EXCEL" || value == "Excel") {
                cmbToAppend += '<option value="Excel">' + value + '</option>';
            }
            else if (value == "Pdf" || value == "PDF") {
                cmbToAppend += ' <option value="pdf">' + value + '</option>';
            }
        });
        cmbToAppend += '</select></center>';
        cmbToAppend += '<input type="hidden" name="DownloadReport" id="DownloadReport" value="true" />';

        var QueryPublicId = $('#ThirdKnowledge_FormQueryPublicId').val();
        cmbToAppend += '<input type="hidden" name="ThirdKnowledge_FormQueryPublicId" id="ThirdKnowledge_QueryPublicId" value="' + QueryPublicId + '" />';        
        
        $('#' + vInitObject.ObjectId + '_DialogForm').empty();
        $('#' + vInitObject.ObjectId + '_DialogForm').append(cmbToAppend);
        $('#' + vInitObject.ObjectId + '_Dialog').dialog({
            title: vInitObject.Tittle,
            modal: true,
            buttons: {
                'Descargar': function () {
                    $('#' + vInitObject.ObjectId + '_DialogForm').submit();
                    $(this).dialog("close");
                }
            }
        });
    }
}

var MyQueriesSearchObj = {
     SearchUrl: ''
    ,FilterList:''

    ,Init: function(vInitObject) {                
        this.SearchUrl = vInitObject.SearchUrl;
        this.FilterList = vInitObject.FilterList;
    }

    ,Search: function (vSearchParam)
    {   
        debugger;
        if (vSearchParam.PageNumber != null) {
            var url = this.SearchUrl;
            url = '?PageNumber=' + vSearchParam.PageNumber;
            url += '&InitDate=' + vSearchParam.InitDate;
            url += '&EndDate=' + vSearchParam.EndDate;
            url += '&SearchType=' + vSearchParam.SearchType;
            url += '&Status=' + vSearchParam.Status;
            url += '&User=' + vSearchParam.User;
            url += '&Domain=' + vSearchParam.Domain;
            
            window.location = url;
        }
        else {
            var ListToParse = '';
            var userFromList = '';
            var queryTypeFromList = '';
            var statusFromList = '';
            var dateFFromList = '';
            var dateToFromList = '';
            var domainFromList = '';
            if (vSearchParam.vSearchParam.Filters != "" && vSearchParam.vSearchParam.Filters != null) {
                ListToParse = vSearchParam.vSearchParam.Filters.split(',');
                for (var i = 0; i < ListToParse.length; i++) {
                    if (ListToParse[i].split(';')[1] != '' && ListToParse[i].split(';')[1] == "701001") {
                        if (vSearchParam.vSearchParam.UserEnable == null) {
                            userFromList = ListToParse[i].split(';')[0];
                        }
                    }
                    if (ListToParse[i].split(';')[1] != '' && ListToParse[i].split(';')[1] == "701002") {
                        if (vSearchParam.vSearchParam.QueryTypeEnable == null) {
                            queryTypeFromList = ListToParse[i].split(';')[0];
                        }
                    }
                    if (ListToParse[i].split(';')[1] != '' && ListToParse[i].split(';')[1] == "701003") {
                        if (vSearchParam.vSearchParam.StatusEnable == null) {
                            statusFromList = ListToParse[i].split(';')[0];
                        }
                    }
                    if (ListToParse[i].split(';')[1] != '' && ListToParse[i].split(';')[1] == "701004") {
                        if (vSearchParam.vSearchParam.DomainEnable == null) {
                            domainFromList = ListToParse[i].split(';')[0];
                        }
                    }
                    if (ListToParse[i].split(';')[1] != '' && ListToParse[i].split(';')[1] == "701005") {
                        dateFFromList = ListToParse[i].split(';')[0];
                    }
                    if (ListToParse[i].split(';')[1] != '' && ListToParse[i].split(';')[1] == "701006") {
                        dateToFromList = ListToParse[i].split(';')[0];
                    }
                }
            }

            var url = this.SearchUrl;
            url = '?PageNumber=' + '0';
            if (document.getElementById("dt_Filter_From").value != '') {
                url += '&InitDate=' + document.getElementById("dt_Filter_From").value;
            }
            else if (dateFFromList != '') {
                url += '&InitDate=' + dateFFromList;
            }
            else {
                url += '&InitDate=' + '';
            }
            if (document.getElementById("dt_Filter_To").value != '') {
                url += '&EndDate=' + document.getElementById("dt_Filter_To").value;
            }
            else if (dateToFromList != '') {
                url += '&EndDate=' + dateToFromList
            }
            else {
                url += '&EndDate=' + '';
            }
            if (vSearchParam.vSearchParam.QueryType != null && vSearchParam.vSearchParam.QueryTypeEnable != null && vSearchParam.vSearchParam.QueryTypeEnable == true) {
                url += '&SearchType=' + vSearchParam.vSearchParam.QueryType
            }
            else if (queryTypeFromList != '') {
                url += '&SearchType=' + queryTypeFromList
            }
            else {
                url += '&SearchType=' + '';
            }
            if (vSearchParam.vSearchParam.Status != null && vSearchParam.vSearchParam.StatusEnable != null && vSearchParam.vSearchParam.StatusEnable == true) {
                url += '&Status=' + vSearchParam.vSearchParam.Status
            }
            else if (statusFromList != '') {
                url += '&Status=' + statusFromList;
            }
            else {
                url += '&Status=' + '';
            }
            if (vSearchParam.vSearchParam.User != null && vSearchParam.vSearchParam.UserEnable != null && vSearchParam.vSearchParam.UserEnable == true) {
                url += '&User=' + vSearchParam.vSearchParam.User
            }
            else if (userFromList != '') {
                url += '&User=' + userFromList;
            }
            else {
                url += '&User=' + '';
            }
            if (vSearchParam.vSearchParam.Domain != null && vSearchParam.vSearchParam.DomainEnable != null && vSearchParam.vSearchParam.DomainEnable == true) {
                url += '&Domain=' + vSearchParam.vSearchParam.Domain
            }
            else if (domainFromList != '') {
                url += '&Domain=' + domainFromList;
            }
            else {
                url += '&Domain=' + '';
            }
            window.location = url;
        }
    }
}
