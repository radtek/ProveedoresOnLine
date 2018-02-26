using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Models.Provider
{
    public class ProviderAditionalDocumentViewModel
    {
        public ProveedoresOnLine.Company.Models.Util.GenericItemModel RelatedAditionalDocument { get; private set; }

        public string AditionalDocumentId { get; set; }

        public string AditionalDocumentName { get; set; }

        public bool Enable { get; set; }

        #region Aditional Documents

        public string AD_Title { get; set; }

        public string AD_File { get; set; }

        public string AD_FileId { get; set; }

        public string AD_RelatedCustomer { get; set; }

        public string AD_RelatedCustomerId { get; set; }

        public string AD_RelatedCustomerName { get; set; }

        public List<ProviderCustomerViewModel> AD_RelatedCustomerList { get; set; }

        public string AD_RelatedUser { get; set; }

        public string AD_RelatedUserId { get; set; }

        public string AD_CreateDate { get; set; }

        public string AD_ValueId { get; set; }

        public string AD_Value { get; set; }

        public string AD_InitialDateId { get; set; }
        public string AD_InitialDate { get; set; }

        public string AD_EndDateId { get; set; }
        public string AD_EndDate { get; set; }

        public string AD_VigencyId { get; set; }
        public string AD_Vigency { get; set; }

        public string AD_DescriptionId { get; set; }
        public string AD_Description { get; set; }

        #endregion

        #region Aditional Data

        public string ADT_Title { get; set; }

        public string ADT_DataTypeId { get; set; }

        public string ADT_DataType { get; set; }

        public string ADT_DataValueId { get; set; }

        public string ADT_DataValue { get; set; }

        public string ADT_RelatedCustomer { get; set; }

        public string ADT_RelatedCustomerId { get; set; }

        public string ADT_RelatedCustomerName { get; set; }

        public string ADT_RelatedUser { get; set; }

        public string ADT_RelatedUserId { get; set; }

        public string ADT_CreateDate { get; set; }

        #endregion

        public ProviderAditionalDocumentViewModel() { }

        public ProviderAditionalDocumentViewModel(ProveedoresOnLine.Company.Models.Util.GenericItemModel oRelatedAditionalDocument,
                                                  ProveedoresOnLine.CompanyCustomer.Models.Customer.CustomerModel oCustomerList)
        {
            RelatedAditionalDocument = oRelatedAditionalDocument;

            AditionalDocumentId = RelatedAditionalDocument.ItemId.ToString();
            AditionalDocumentName = RelatedAditionalDocument.ItemName;
            Enable = RelatedAditionalDocument.Enable;

            #region Aditional Document

            AD_FileId = RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_File).
                Select(x => x.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            AD_File = RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_File).
                Select(x => x.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            AD_RelatedCustomerId = RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_RelatedCustomer).
                Select(x => x.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            AD_RelatedCustomer = RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_RelatedCustomer).
                Select(x => x.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            AD_RelatedCustomerName = oCustomerList.RelatedProvider.Where(x => x.RelatedProvider.CompanyPublicId == AD_RelatedCustomer).
                 Select(x => x.RelatedProvider.CompanyName).
                 DefaultIfEmpty(string.Empty).
                 FirstOrDefault();

            AD_RelatedCustomerList = new List<ProviderCustomerViewModel>();
            RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_RelatedCustomer && x.Enable == true).All(w => {
                AD_RelatedCustomerList.Add(new ProviderCustomerViewModel()
                {
                    CP_Customer = oCustomerList.RelatedProvider.Where(y => y.RelatedProvider.CompanyPublicId == w.Value).Select(y => y.RelatedProvider.CompanyName).SingleOrDefault(),
                    CP_CustomerPublicId = w.Value
                });
                
                return true;
            });

            AD_RelatedUserId = RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_RelatedUser).
                Select(x => x.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            AD_RelatedUser = RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_RelatedUser).
                Select(x => x.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            AD_CreateDate = RelatedAditionalDocument.LastModify.ToString();

            AD_Title = RelatedAditionalDocument.ItemName;

            AD_ValueId = RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_Value).
                Select(x => x.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            AD_Value = RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_Value).
                Select(x => x.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            AD_InitialDateId = RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_InitialDate).
               Select(x => x.ItemInfoId.ToString()).
               DefaultIfEmpty(string.Empty).
               FirstOrDefault();
            AD_InitialDate = RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_InitialDate).
                Select(x => x.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            AD_EndDateId = RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_EndDate).
                Select(x => x.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            AD_EndDate = RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_EndDate).
                Select(x => x.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            AD_VigencyId = RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_Vigency).
                Select(x => x.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            AD_Vigency = RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_Vigency).
                Select(x => x.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            AD_DescriptionId = RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_Description).
                Select(x => x.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            AD_Description = RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDocumentInfoType.AD_Description).
                Select(x => x.LargeValue).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            #endregion

            #region Aditional Data

            ADT_DataTypeId = RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDataInfoType.ADT_DataType).
                    Select(x => x.ItemInfoId.ToString()).
                    DefaultIfEmpty(string.Empty).
                    FirstOrDefault();

            ADT_DataType = RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDataInfoType.ADT_DataType).
                Select(x => x.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            ADT_DataValueId = RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDataInfoType.ADT_Value).
                Select(x => x.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            ADT_DataValue = RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDataInfoType.ADT_Value).
                Select(x => x.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            ADT_RelatedCustomerId = RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDataInfoType.ADT_RelatedCustomer).
                Select(x => x.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            ADT_RelatedCustomer = RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDataInfoType.ADT_RelatedCustomer).
                Select(x => x.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            ADT_RelatedCustomerName = oCustomerList.RelatedProvider.Where(x => x.RelatedProvider.CompanyPublicId == ADT_RelatedCustomer).
                 Select(x => x.RelatedProvider.CompanyName).
                 DefaultIfEmpty(string.Empty).
                 FirstOrDefault();

            ADT_RelatedUserId = RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDataInfoType.ADT_RelatedUser).
                Select(x => x.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            ADT_RelatedUser = RelatedAditionalDocument.ItemInfo.Where(x => x.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumAditionalDataInfoType.ADT_RelatedUser).
                Select(x => x.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            ADT_CreateDate = RelatedAditionalDocument.LastModify.ToString();

            ADT_Title = RelatedAditionalDocument.ItemName;

            #endregion
        }
    }
}
