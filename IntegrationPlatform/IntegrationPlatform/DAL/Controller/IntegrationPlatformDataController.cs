﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntegrationPlatform.Models.Integration;
using ProveedoresOnLine.Company.Models.Company;

namespace IntegrationPlatform.DAL.Controller
{
    internal class IntegrationPlatformDataController : IntegrationPlatform.Interfaces.IIntegrationPlatformData
    {
        #region singleton instance

        private static IntegrationPlatform.Interfaces.IIntegrationPlatformData oInstance;

        internal static IntegrationPlatform.Interfaces.IIntegrationPlatformData Instance
        {
            get
            {
                if (oInstance == null)
                    oInstance = new IntegrationPlatformDataController();
                return oInstance;
            }
        }

        private IntegrationPlatform.Interfaces.IIntegrationPlatformData DataFactory;

        #endregion

        #region Constructor

        public IntegrationPlatformDataController()
        {
            IntegrationPlatformDataFactory factory = new IntegrationPlatformDataFactory();
            DataFactory = factory.GetIntegrationPlatformInstance();
        }

        #endregion

        #region Integration

        public CustomDataModel CustomerProvider_GetCustomData(CompanyModel Customer, string ProviderPublicId)
        {
            return DataFactory.CustomerProvider_GetCustomData(Customer, ProviderPublicId);
        }

        public Models.Integration.CustomDataModel MP_CustomerProvider_GetCustomData(string CustomerPublicId, string ProviderPublicId)
        {
            return DataFactory.MP_CustomerProvider_GetCustomData(CustomerPublicId, ProviderPublicId);
        }

        public List<ProveedoresOnLine.Company.Models.Util.CatalogModel> CatalogGetCustomerOptions(string CustomerPublicId)
        {
            return DataFactory.CatalogGetCustomerOptions(CustomerPublicId);
        }

        #region Integration Sanofi

        public int Sanofi_AditionalData_Upsert(int AditionalDataId, string ProviderPublicId, int AditionalFieldId, string AditionalDataName, bool Enable)
        {
            return DataFactory.Sanofi_AditionalData_Upsert(AditionalDataId, ProviderPublicId, AditionalFieldId, AditionalDataName, Enable);
        }

        public int Sanofi_AditionalDataInfo_Upsert(int AditionalDataInfoId, int AditionalDataId, int? AditionalDataInfoType, string Value, string LargeValue, bool Enable)
        {
            return DataFactory.Sanofi_AditionalDataInfo_Upsert(AditionalDataInfoId, AditionalDataId, AditionalDataInfoType, Value, LargeValue, Enable);
        }

        #endregion

        #region Falabella

        public int Falabella_AditionalData_Upsert(int AditionalDataId, string ProviderPublicId, int AditionalFieldId, string AditionalDataName, bool Enable)
        {
            return DataFactory.Falabella_AditionalData_Upsert(AditionalDataId, ProviderPublicId, AditionalFieldId, AditionalDataName, Enable);
        }

        public int Falabella_AditionalDataInfo_Upsert(int AditionalDataInfoId, int AditionalDataId, int? AditionalDataInfoType, string Value, string LargeValue, bool Enable)
        {
            return DataFactory.Falabella_AditionalDataInfo_Upsert(AditionalDataInfoId, AditionalDataId, AditionalDataInfoType, Value, LargeValue, Enable);
        }

        #endregion

        #region Alpina

        public int Alpina_AditionalData_Upsert(int AditionalDataId, string ProviderPublicId, int AditionalFieldId, string AditionalDataName, bool Enable)
        {
            return DataFactory.Alpina_AditionalData_Upsert(AditionalDataId, ProviderPublicId, AditionalFieldId, AditionalDataName, Enable);
        }

        public int Alpina_AditionalDataInfo_Upsert(int AditionalDataInfoId, int AditionalDataId, int? AditionalDataInfoType, string Value, string LargeValue, bool Enable)
        {
            return DataFactory.Alpina_AditionalDataInfo_Upsert(AditionalDataInfoId, AditionalDataId, AditionalDataInfoType, Value, LargeValue, Enable);
        }

        #endregion

        #region Aje

        public int Aje_AditionalData_Upsert(int AditionalDataId, string ProviderPublicId, int AditionalFieldId, string AditionalDataName, bool Enable)
        {
            return DataFactory.Aje_AditionalData_Upsert(AditionalDataId, ProviderPublicId, AditionalFieldId, AditionalDataName, Enable);
        }

        public int Aje_AditionalDataInfo_Upsert(int AditionalDataInfoId, int AditionalDataId, int? AditionalDataInfoType, string Value, string LargeValue, bool Enable)
        {
            return DataFactory.Aje_AditionalDataInfo_Upsert(AditionalDataInfoId, AditionalDataId, AditionalDataInfoType, Value, LargeValue, Enable);
        }

        #endregion

        #region Integration Publicar

        public int Publicar_AditionalData_Upsert(int AditionalDataId, string ProviderPublicId, int AditionalFieldId, string AditionalDataName, bool Enable)
        {
            return DataFactory.Publicar_AditionalData_Upsert(AditionalDataId, ProviderPublicId, AditionalFieldId, AditionalDataName, Enable);
        }

        public int Publicar_AditionalDataInfo_Upsert(int AditionalDataInfoId, int AditionalDataId, int? AditionalDataInfoType, string Value, string LargeValue, bool Enable)
        {
            return DataFactory.Publicar_AditionalDataInfo_Upsert(AditionalDataInfoId, AditionalDataId, AditionalDataInfoType, Value, LargeValue, Enable);
        }

        #endregion

        #endregion
    }
}
