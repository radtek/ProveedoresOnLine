using ProveedoresOnLine.Company.Models.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPlace.Models.Survey
{
    public class SurveyUtil
    {
        #region Generic Catalog

        public static List<CatalogModel> GetSurveyName
        {
            get
            {
                if (oGetSurveyName == null)
                {
                    oGetSurveyName =
                        ProveedoresOnLine.SurveyModule.Controller.SurveyModule.CatalogGetSurveyName();
                }
                return oGetSurveyName;
            }
        }

        private static List<ProveedoresOnLine.Company.Models.Util.CatalogModel> oGetSurveyName;

        #endregion

        #region Survey filter name

        public static string GetSurveyOptionName(string vSurveyOptionId)
        {
            return GetSurveyName.
                        Where(po => po.ItemId.ToString() == vSurveyOptionId).
                        Select(po => po.ItemName).
                        DefaultIfEmpty(string.Empty).
                        FirstOrDefault();
        }

        #endregion
    }
}
