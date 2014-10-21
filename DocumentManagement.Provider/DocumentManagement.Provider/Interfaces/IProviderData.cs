﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManagement.Provider.Interfaces
{
    interface IProviderData
    {
        string ProviderUpsert(string ProviderPublicId, string Name, int IdentificationTypeId, string IdentificationNumber, string Email);

        int ProviderInfoUpsert(string ProviderPublicId, int? ProviderInfoId, int ProviderInfoTypeId, string Value, string LargeValue);

        int ProviderCustomerInfoUpsert(string ProviderPublicId, string CustomerPublicId, int? ProviderCustomerInfoId, int ProviderCustomerInfoTypeId, string Value, string LargeValue);

        List<DocumentManagement.Provider.Models.Provider.ProviderModel> ProviderSearch(string SearchParam, int PageNumber, int RowCount, out int TotalRows);

        DocumentManagement.Provider.Models.Provider.ProviderModel ProviderGetByIdentification(string IdentificationNumber, int IdenificationTypeId, string CustomerPublicId);

        DocumentManagement.Provider.Models.Provider.ProviderModel ProviderGetById(string ProviderPublicId, int? StepId);
    }
}
