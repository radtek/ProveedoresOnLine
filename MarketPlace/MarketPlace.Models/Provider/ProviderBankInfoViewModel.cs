using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPlace.Models.Provider
{
    public class ProviderBankInfoViewModel
    {
        #region BankInfo
        public ProviderViewModel RelatedViewProvider { get; set; }

        public ProviderBankInfoViewModel()
        {

        }

        public ProviderBankInfoViewModel(ProveedoresOnLine.Company.Models.Util.GenericItemModel oRelatedInfo)
        {
            RelatedBankInfo = oRelatedInfo;
        }

        public ProveedoresOnLine.Company.Models.Util.GenericItemModel RelatedBankInfo { get; private set; }
        public string oIB_AccountType { get; set; }
        public string IB_AccountType
        {
            get
            {
                if (string.IsNullOrEmpty(oIB_AccountType))
                {
                    oIB_AccountType = RelatedBankInfo.ItemInfo.
                        Where(y => y.ItemInfoType.ItemId == (int)MarketPlace.Models.General.enumFinancialInfoType.IB_AccountType).
                        Select(y => y.Value).
                        DefaultIfEmpty(string.Empty).
                        FirstOrDefault();
                }

                return oIB_AccountType;
            }
        }

        public string oIB_AccountNumber { get; set; }
        public string IB_AccountNumber
        {
            get
            {
                if (string.IsNullOrEmpty(oIB_AccountNumber))
                {
                    oIB_AccountNumber = RelatedBankInfo.ItemInfo.
                        Where(y => y.ItemInfoType.ItemId == (int)MarketPlace.Models.General.enumFinancialInfoType.IB_AccountNumber).
                        Select(y => y.Value).
                        DefaultIfEmpty(string.Empty).
                        FirstOrDefault();
                }

                return oIB_AccountNumber;
            }
        }

        public string oIB_AccountHolder { get; set; }
        public string IB_AccountHolder
        {
            get
            {
                if (string.IsNullOrEmpty(oIB_AccountHolder))
                {
                    oIB_AccountHolder = RelatedBankInfo.ItemInfo.
                        Where(y => y.ItemInfoType.ItemId == (int)MarketPlace.Models.General.enumFinancialInfoType.IB_AccountHolder).
                        Select(y => y.Value).
                        DefaultIfEmpty(string.Empty).
                        FirstOrDefault();
                }

                return oIB_AccountHolder;
            }
        }
        #endregion

    }
}
