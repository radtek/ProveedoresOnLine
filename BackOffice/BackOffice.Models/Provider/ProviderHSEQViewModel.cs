using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Models.Provider
{
    public class ProviderHSEQViewModel
    {
        public ProveedoresOnLine.Company.Models.Util.GenericItemModel RelatedCertification { get; private set; }

        public string CertificationId { get; set; }
        public string CertificationInfoId { get; set; }

        public string CertificationName { get; set; }

        public bool Enable { get; set; }

        #region Certifications

        public string C_CertificationCompany { get; set; }
        public string C_CertificationCompanyId { get; set; }
        public string C_CertificationCompanyName { get; set; }

        public string C_Rule { get; set; }
        public string C_RuleId { get; set; }
        public string C_RuleName { get; set; }

        public string C_StartDateCertification { get; set; }
        public string C_StartDateCertificationId { get; set; }

        public string C_EndDateCertification { get; set; }
        public string C_EndDateCertificationId { get; set; }

        public string C_CCS { get; set; }
        public string C_CCSId { get; set; }

        public string C_CertificationFile { get; set; }
        public string C_CertificationFileId { get; set; }

        public string C_Scope { get; set; }
        public string C_ScopeId { get; set; }

        #endregion

        #region HealtyPolitics

        public string CH_Year { get; set; }
        public string CH_YearId { get; set; }

        public string CH_Document { get; set; }
        public string CH_DocumentId { get; set; }

        public string CH_TypeDocument { get; set; }
        public string CH_TypeDocumentId { get; set; }

        public string CH_Other { get; set; }
        public string CH_OtherId { get; set; }

        public string CH_CertificationsForm { get; set; }
        public string CH_CertificationsFormId { get; set; }

        #endregion

        #region RiskPolicies

        public string CR_SystemOccupationalHazards { get; set; }
        public string CR_SystemOccupationalHazardsId { get; set; }
        public string CR_SystemOccupationalHazardsName { get; set; }

        public string CR_RateARL { get; set; }
        public string CR_RateARLId { get; set; }

        public string CR_CertificateAffiliateARL { get; set; }
        public string CR_CertificateAffiliateARLId { get; set; }

        public string CR_LTIFResult { get; set; }

        #endregion

        #region CertificatesAccident

        public string CA_Year { get; set; }
        public string CA_YearId { get; set; }

        public string CA_ManHoursWorked { get; set; }
        public string CA_ManHoursWorkedId { get; set; }

        public string CA_Fatalities { get; set; }
        public string CA_FatalitiesId { get; set; }

        public string CA_NumberAccident { get; set; }
        public string CA_NumberAccidentId { get; set; }

        public string CA_NumberAccidentDisabling { get; set; }
        public string CA_NumberAccidentDisablingId { get; set; }

        public string CA_DaysIncapacity { get; set; }
        public string CA_DaysIncapacityId { get; set; }

        public string CA_CertificateAccidentARL { get; set; }
        public string CA_CertificateAccidentARLId { get; set; }

        #endregion

        public ProviderHSEQViewModel() { }

        public ProviderHSEQViewModel(ProveedoresOnLine.Company.Models.Util.GenericItemModel oRelatedCertification,
                                     List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> oRule,
                                     List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> oCompanyRule,
                                     List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> oARL, int pTypeDocumentId)
        {
            RelatedCertification = oRelatedCertification;
            CertificationId = RelatedCertification.ItemId.ToString();
            CertificationInfoId = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == pTypeDocumentId).
                Select(y => y.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            CertificationName = RelatedCertification.ItemName;
            Enable = pTypeDocumentId == 0 ? RelatedCertification.Enable : (
                    RelatedCertification.ItemInfo.
                        Where(y => y.ItemInfoType.ItemId == pTypeDocumentId).
                        Select(y => y.Enable).
                        FirstOrDefault()
                );

            #region Certifications

            C_CertificationCompany = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.C_CertificationCompany).
                Select(y => y.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            C_CertificationCompanyId = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.C_CertificationCompany).
                Select(y => y.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            if (oCompanyRule != null && oCompanyRule.Count > 0)
            {
                C_CertificationCompanyName = oCompanyRule.
                    Where(x => x.ItemId.ToString() == C_CertificationCompany).
                    Select(x => x.ItemName).
                    DefaultIfEmpty(string.Empty).
                    FirstOrDefault();
            }

            C_Rule = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.C_Rule).
                Select(y => y.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            C_RuleId = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.C_Rule).
                Select(y => y.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            if (oRule != null && oRule.Count > 0)
            {
                C_RuleName = oRule.
                    Where(x => x.ItemId.ToString() == C_Rule).
                    Select(x => x.ItemName).
                    DefaultIfEmpty(string.Empty).
                    FirstOrDefault();
            }

            C_StartDateCertification = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.C_StartDateCertification).
                Select(y => y.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            C_StartDateCertificationId = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.C_StartDateCertification).
                Select(y => y.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            C_EndDateCertification = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.C_EndDateCertification).
                Select(y => y.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            C_EndDateCertificationId = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.C_EndDateCertification).
                Select(y => y.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

           /* C_CCS = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.C_CCS).
                Select(y => y.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            C_CCSId = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.C_CCS).
                Select(y => y.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();*/

            C_CertificationFile = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.C_CertificationFile).
                Select(y => y.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            C_CertificationFileId = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.C_CertificationFile).
                Select(y => y.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            C_Scope = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.C_Scope).
                Select(y => y.LargeValue).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            C_ScopeId = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.C_Scope).
                Select(y => y.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            #endregion

            #region HealtyPolitics

            CH_Year = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CH_Year).
                Select(y => y.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            CH_YearId = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CH_Year).
                Select(y => y.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            CH_Document = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == pTypeDocumentId).
                Select(y => y.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            CH_DocumentId = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == pTypeDocumentId).
                Select(y => y.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            CH_TypeDocumentId = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == pTypeDocumentId).
                Select(y => y.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            CH_TypeDocument = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == pTypeDocumentId).
                Select(y => y.ItemInfoType.ItemId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            CH_Other = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == pTypeDocumentId).
                Select(y => (y.LargeValue == null ? "" : y.LargeValue)).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            CH_OtherId = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == pTypeDocumentId).
                Select(y => y.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();


            CH_CertificationsForm = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CH_CertificationsForm).
                Select(y => y.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            CH_CertificationsFormId = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CH_CertificationsForm).
                Select(y => y.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            #endregion

            #region RiskPolicies

            CR_SystemOccupationalHazards = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CR_SystemOccupationalHazards).
                Select(y => y.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            CR_SystemOccupationalHazardsId = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CR_SystemOccupationalHazards).
                Select(y => y.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            if (oARL != null && oARL.Count > 0)
            {
                CR_SystemOccupationalHazardsName = oARL.
                    Where(x => x.ItemId.ToString() == CR_SystemOccupationalHazards).
                    Select(x => x.ItemName).
                    DefaultIfEmpty(string.Empty).
                    FirstOrDefault();
            }

            CR_RateARL = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CR_RateARL).
                Select(y => y.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            CR_RateARLId = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CR_RateARL).
                Select(y => y.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            CR_CertificateAffiliateARL = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CR_CertificateAffiliateARL).
                Select(y => y.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            CR_CertificateAffiliateARLId = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CR_CertificateAffiliateARL).
                Select(y => y.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            CR_LTIFResult = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CR_LTIFResult).
                Select(y => y.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            #endregion

            #region CertificatesAcccident

            CA_Year = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CA_Year).
                Select(y => y.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            CA_YearId = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CA_Year).
                Select(y => y.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            CA_ManHoursWorked = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CA_ManHoursWorked).
                Select(y => y.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            CA_ManHoursWorkedId = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CA_ManHoursWorked).
                Select(y => y.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            CA_Fatalities = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CA_Fatalities).
                Select(y => y.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            CA_FatalitiesId = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CA_Fatalities).
                Select(y => y.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            CA_NumberAccident = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CA_NumberAccident).
                Select(y => y.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            CA_NumberAccidentId = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CA_NumberAccident).
                Select(y => y.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            CA_NumberAccidentDisabling = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CA_NumberAccidentDisabling).
                Select(y => y.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            CA_NumberAccidentDisablingId = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CA_NumberAccidentDisabling).
                Select(y => y.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            CA_DaysIncapacity = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CA_DaysIncapacity).
                Select(y => y.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            CA_DaysIncapacityId = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CA_DaysIncapacity).
                Select(y => y.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();

            CA_CertificateAccidentARL = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CA_CertificateAccidentARL).
                Select(y => y.Value).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            CA_CertificateAccidentARLId = RelatedCertification.ItemInfo.
                Where(y => y.ItemInfoType.ItemId == (int)BackOffice.Models.General.enumHSEQInfoType.CA_CertificateAccidentARL).
                Select(y => y.ItemInfoId.ToString()).
                DefaultIfEmpty(string.Empty).
                FirstOrDefault();
            #endregion
        }
    }
}
