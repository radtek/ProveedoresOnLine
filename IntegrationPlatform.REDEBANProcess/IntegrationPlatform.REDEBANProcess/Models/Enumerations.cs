using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPlatform.REDEBANProcess.Models
{
    public class Enumerations
    {
        public enum enumNotificationType
        {
            RedebanNotification = 1801002
        }

        //LegalInfoType
        public enum enumLegalInfoType
        {
            ChaimberOfCommerceFile = 602006,
            RUTFile = 603012
        }

        //FinancialInfoType
        public enum enumFinancialInfoType
        {
            FinancialStatsFile = 502002,
            BankCertificationFile = 505010,
            IS_FileIncomeStatement = 504006
        }

        //CommercialInfoType
        public enum enumCommercialInfoType
        {
            ExpereienceCertificationFile = 302011
        }

        //CertificationInfoType
        public enum enumCertificationInfoType
        {
            //Certifications                        
            C_CertificationFile = 702006,

            //CompanyHealtyPolitic            
            CH_PoliticsSecurity = 703002,
            CH_PoliticsNoAlcohol = 703003,
            CH_ProgramOccupationalHealth = 703004,
            CH_RuleIndustrialSecurity = 703005,
            CH_MatrixRiskControl = 703006,
            CH_CorporateSocialResponsability = 703007,
            CH_ProgramEnterpriseSecurity = 703008,
            CH_PoliticsRecruiment = 703009,
            CH_CertificationsForm = 703010,
            CH_PoliticIntegral = 703011,

            //CompanyRiskPolicies            
            CR_CertificateAffiliateARL = 704003,

            //CertficatesAccident                      
            CA_CertificateAccidentARL = 705007,

        }

        public enum enumAditionalDocumentInfoType
        {
            AD_File = 1702001
        }
    }
}
