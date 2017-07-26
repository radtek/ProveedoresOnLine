﻿using ProveedoresOnLine.CalificationBatch.Models.CalificationProjectBatch;
using ProveedoresOnLine.CalificationProject.Models.CalificationProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.CalificationBatch.CalificationProjectModule
{
    public class AditionalDocumentModule
    {
        public static CalificationProjectItemBatchModel AditionalDocumentationRule(string CompanyPublicId, ConfigItemModel oCalificationProjectItemModel, CalificationProjectItemBatchModel oRelatedCalificationProjectItemModel)
        {
            CalificationProcess.LogFile("Aditional Document Module in Process::");

            CalificationProjectItemBatchModel oReturn = new CalificationProjectItemBatchModel()
            {
                CalificationProjectItemId = oRelatedCalificationProjectItemModel != null && oRelatedCalificationProjectItemModel.CalificationProjectItemId > 0 ? oRelatedCalificationProjectItemModel.CalificationProjectItemId : 0,
                CalificationProjectConfigItem = new ConfigItemModel()
                {
                    CalificationProjectConfigItemId = oRelatedCalificationProjectItemModel != null && oRelatedCalificationProjectItemModel.CalificationProjectConfigItem != null ? oRelatedCalificationProjectItemModel.CalificationProjectConfigItem.CalificationProjectConfigItemId : oCalificationProjectItemModel.CalificationProjectConfigItemId,
                },
                CalificatioProjectItemInfoModel = new List<CalificationProjectItemInfoBatchModel>(),
                ItemScore = oRelatedCalificationProjectItemModel != null ? oRelatedCalificationProjectItemModel.ItemScore : 0,
                Enable = true,
            };

            List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> oAdDocProviderInfo;

            #region Variables

            Int32 oTotalModuleScore = 0;
            Int32 AdDocScore = 0;
            Int32 RuleScore = 0;
            Int32 oIntValue = 0;
            decimal oDecimalValue = 0;
            double oPercentValue = 0;
            DateTime oDateValue = new DateTime();
            string oTextValue = "";
            bool oBooleanValue = false;

            #endregion

            try
            {
                if (oRelatedCalificationProjectItemModel != null &&
                oRelatedCalificationProjectItemModel.CalificatioProjectItemInfoModel != null &&
                oRelatedCalificationProjectItemModel.CalificatioProjectItemInfoModel.Count > 0)
                {
                    oReturn.CalificatioProjectItemInfoModel = oRelatedCalificationProjectItemModel.CalificatioProjectItemInfoModel;
                }

                if (oReturn.CalificatioProjectItemInfoModel != null &&
                    oReturn.CalificatioProjectItemInfoModel.Count > 0)
                {
                    //Validate if module is not enable
                    if (oRelatedCalificationProjectItemModel.CalificationProjectConfigItem.Enable)
                    {
                        //Validate rule config with mp rule
                        oRelatedCalificationProjectItemModel.CalificatioProjectItemInfoModel.All(mprule =>
                        {
                            bool mpEnable = false;

                            oCalificationProjectItemModel.CalificationProjectConfigItemInfoModel.Where(cnfrule => cnfrule.Enable == true).All(cnfrule =>
                            {
                                if (!mpEnable && mprule.CalificationProjectConfigItemInfoModel.CalificationProjectConfigItemInfoId == cnfrule.CalificationProjectConfigItemInfoId)
                                {
                                    mpEnable = true;
                                }

                                return true;
                            });

                            mprule.Enable = mpEnable;

                            return true;
                        });
                    }
                    else
                    {
                        //disable all params
                        oRelatedCalificationProjectItemModel.CalificatioProjectItemInfoModel.All(mprule =>
                        {
                            mprule.Enable = false;
                            mprule.CalificationProjectConfigItemInfoModel.Enable = false;
                            return true;
                        });
                    }

                    oCalificationProjectItemModel.CalificationProjectConfigItemInfoModel.Where(rule => rule.Enable == true).All(rule =>
                    {
                        ProveedoresOnLine.CalificationBatch.CalificationProcess.LogFile("Update validate to Aditonal Document module ::: Provider public id ::: " + CompanyPublicId + " ::: RuleId ::: " + rule.CalificationProjectConfigItemInfoId);

                        if (oRelatedCalificationProjectItemModel.CalificatioProjectItemInfoModel.Any(mprule => mprule.CalificationProjectConfigItemInfoModel.CalificationProjectConfigItemInfoId == rule.CalificationProjectConfigItemInfoId))
                        {
                            oRelatedCalificationProjectItemModel.CalificatioProjectItemInfoModel.Where(mprule => mprule.CalificationProjectConfigItemInfoModel.CalificationProjectConfigItemInfoId == rule.CalificationProjectConfigItemInfoId).All(mprule =>
                            {
                                //update mp rule
                                var AditionalDocumentName = ProveedoresOnLine.CalificationBatch.Controller.CalificationProjectBatch.GetAditionalDocumentName(rule.Question.ItemId);
                                oAdDocProviderInfo = ProveedoresOnLine.CalificationBatch.Controller.CalificationProjectBatch.AditionalDocumentModuleInfo(CompanyPublicId, AditionalDocumentName);

                                oAdDocProviderInfo.Where(pinf => pinf != null).All(pinf =>
                                {
                                    if (RuleScore <= 0)
                                    {
                                        switch (rule.Rule.ItemId)
                                        {
                                            #region Positivo

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.Positivo:

                                                oDecimalValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDecimal(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oDecimalValue > 0)
                                                {
                                                    AdDocScore = int.Parse(rule.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += AdDocScore;

                                                    mprule.ItemInfoScore = AdDocScore;
                                                }
                                                else
                                                {
                                                    mprule.ItemInfoScore = 0;
                                                }

                                                break;

                                            #endregion

                                            #region Negativo

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.Negativo:

                                                oDecimalValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDecimal(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oDecimalValue < 0)
                                                {
                                                    AdDocScore = int.Parse(rule.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += AdDocScore;

                                                    mprule.ItemInfoScore = AdDocScore;
                                                }
                                                else
                                                {
                                                    mprule.ItemInfoScore = 0;
                                                }

                                                break;

                                            #endregion

                                            #region MayorQue

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.MayorQue:

                                                switch (rule.ValueType.ItemId)
                                                {
                                                    #region Tipo valor: numérico

                                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                        oDecimalValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDecimal(pinf.ItemInfo.FirstOrDefault().Value);

                                                        if (oDecimalValue > int.Parse(rule.Value))
                                                        {
                                                            AdDocScore = int.Parse(rule.Score);

                                                            RuleScore++;

                                                            oTotalModuleScore += AdDocScore;

                                                            mprule.ItemInfoScore = AdDocScore;
                                                        }
                                                        else
                                                        {
                                                            mprule.ItemInfoScore = 0;
                                                        }

                                                        break;

                                                    #endregion

                                                    #region Tipo valor: fecha

                                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                        oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value);

                                                        if (oDateValue > Convert.ToDateTime(rule.Value))
                                                        {
                                                            AdDocScore = int.Parse(rule.Score);

                                                            RuleScore++;

                                                            oTotalModuleScore += AdDocScore;

                                                            mprule.ItemInfoScore = AdDocScore;
                                                        }
                                                        else
                                                        {
                                                            mprule.ItemInfoScore = 0;
                                                        }

                                                        break;

                                                    #endregion

                                                    #region Tipo valor: porcentaje

                                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                        oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value);

                                                        if (oPercentValue > Convert.ToDouble(rule.Value))
                                                        {
                                                            AdDocScore = int.Parse(rule.Score);

                                                            RuleScore++;

                                                            oTotalModuleScore += AdDocScore;

                                                            mprule.ItemInfoScore = AdDocScore;
                                                        }
                                                        else
                                                        {
                                                            mprule.ItemInfoScore = 0;
                                                        }

                                                        break;

                                                        #endregion
                                                }

                                                break;

                                            #endregion

                                            #region MenorQue

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.MenorQue:

                                                switch (rule.ValueType.ItemId)
                                                {
                                                    #region Tipo valor: numérico

                                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                        oDecimalValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDecimal(pinf.ItemInfo.FirstOrDefault().Value);

                                                        if (oDecimalValue < int.Parse(rule.Value))
                                                        {
                                                            AdDocScore = int.Parse(rule.Score);

                                                            RuleScore++;

                                                            oTotalModuleScore += AdDocScore;

                                                            mprule.ItemInfoScore = AdDocScore;
                                                        }
                                                        else
                                                        {
                                                            mprule.ItemInfoScore = 0;
                                                        }

                                                        break;

                                                    #endregion

                                                    #region Tipo valor: fecha

                                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                        oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value);

                                                        if (oDateValue < Convert.ToDateTime(rule.Value))
                                                        {
                                                            AdDocScore = int.Parse(rule.Score);

                                                            RuleScore++;

                                                            oTotalModuleScore += AdDocScore;

                                                            mprule.ItemInfoScore = AdDocScore;
                                                        }
                                                        else
                                                        {
                                                            mprule.ItemInfoScore = 0;
                                                        }

                                                        break;

                                                    #endregion

                                                    #region Tipo valor: porcentaje

                                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                        oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value);

                                                        if (oPercentValue < Convert.ToDouble(rule.Value))
                                                        {
                                                            AdDocScore = int.Parse(rule.Score);

                                                            RuleScore++;

                                                            oTotalModuleScore += AdDocScore;

                                                            mprule.ItemInfoScore = AdDocScore;
                                                        }
                                                        else
                                                        {
                                                            mprule.ItemInfoScore = 0;
                                                        }

                                                        break;

                                                        #endregion
                                                }

                                                break;

                                            #endregion

                                            #region MayorOIgual

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.MayorOIgual:

                                                switch (rule.ValueType.ItemId)
                                                {
                                                    #region Tipo valor: numérico

                                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                        oDecimalValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDecimal(pinf.ItemInfo.FirstOrDefault().Value);

                                                        if (oDecimalValue >= int.Parse(rule.Value))
                                                        {
                                                            AdDocScore = int.Parse(rule.Score);

                                                            RuleScore++;

                                                            oTotalModuleScore += AdDocScore;

                                                            mprule.ItemInfoScore = AdDocScore;
                                                        }
                                                        else
                                                        {
                                                            mprule.ItemInfoScore = 0;
                                                        }

                                                        break;

                                                    #endregion

                                                    #region Tipo valor: fecha

                                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                        oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value);

                                                        if (oDateValue >= Convert.ToDateTime(rule.Value))
                                                        {
                                                            AdDocScore = int.Parse(rule.Score);

                                                            RuleScore++;

                                                            oTotalModuleScore += AdDocScore;

                                                            mprule.ItemInfoScore = AdDocScore;
                                                        }
                                                        else
                                                        {
                                                            mprule.ItemInfoScore = 0;
                                                        }

                                                        break;

                                                    #endregion

                                                    #region Tipo valor: porcentaje

                                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                        oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value);

                                                        if (oPercentValue >= Convert.ToDouble(rule.Value))
                                                        {
                                                            AdDocScore = int.Parse(rule.Score);

                                                            RuleScore++;

                                                            oTotalModuleScore += AdDocScore;

                                                            mprule.ItemInfoScore = AdDocScore;
                                                        }
                                                        else
                                                        {
                                                            mprule.ItemInfoScore = 0;
                                                        }

                                                        break;

                                                        #endregion
                                                }

                                                break;

                                            #endregion

                                            #region MenorOIgual

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.MenorOIgual:

                                                switch (rule.ValueType.ItemId)
                                                {
                                                    #region Tipo valor: numérico

                                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                        oDecimalValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDecimal(pinf.ItemInfo.FirstOrDefault().Value);

                                                        if (oDecimalValue <= int.Parse(rule.Value))
                                                        {
                                                            AdDocScore = int.Parse(rule.Score);

                                                            RuleScore++;

                                                            oTotalModuleScore += AdDocScore;

                                                            mprule.ItemInfoScore = AdDocScore;
                                                        }
                                                        else
                                                        {
                                                            mprule.ItemInfoScore = 0;
                                                        }

                                                        break;

                                                    #endregion

                                                    #region Tipo valor: fecha

                                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                        oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value);

                                                        if (oDateValue <= Convert.ToDateTime(rule.Value))
                                                        {
                                                            AdDocScore = int.Parse(rule.Score);

                                                            RuleScore++;

                                                            oTotalModuleScore += AdDocScore;

                                                            mprule.ItemInfoScore = AdDocScore;
                                                        }
                                                        else
                                                        {
                                                            mprule.ItemInfoScore = 0;
                                                        }

                                                        break;

                                                    #endregion

                                                    #region Tipo valor: porcentaje

                                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                        oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value);

                                                        if (oPercentValue <= Convert.ToDouble(rule.Value))
                                                        {
                                                            AdDocScore = int.Parse(rule.Score);

                                                            RuleScore++;

                                                            oTotalModuleScore += AdDocScore;

                                                            mprule.ItemInfoScore = AdDocScore;
                                                        }
                                                        else
                                                        {
                                                            mprule.ItemInfoScore = 0;
                                                        }

                                                        break;

                                                        #endregion
                                                }

                                                break;

                                            #endregion

                                            #region IgualQue

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.IgualQue:

                                                switch (rule.ValueType.ItemId)
                                                {
                                                    #region Tipo valor: numérico

                                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                        oDecimalValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDecimal(pinf.ItemInfo.FirstOrDefault().Value);

                                                        if (oDecimalValue == int.Parse(rule.Value))
                                                        {
                                                            AdDocScore = int.Parse(rule.Score);

                                                            RuleScore++;

                                                            oTotalModuleScore += AdDocScore;

                                                            mprule.ItemInfoScore = AdDocScore;
                                                        }
                                                        else
                                                        {
                                                            mprule.ItemInfoScore = 0;
                                                        }

                                                        break;

                                                    #endregion

                                                    #region Tipo valor: fecha

                                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                        oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value);

                                                        if (oDateValue == Convert.ToDateTime(rule.Value))
                                                        {
                                                            AdDocScore = int.Parse(rule.Score);

                                                            RuleScore++;

                                                            oTotalModuleScore += AdDocScore;

                                                            mprule.ItemInfoScore = AdDocScore;
                                                        }
                                                        else
                                                        {
                                                            mprule.ItemInfoScore = 0;
                                                        }

                                                        break;

                                                    #endregion

                                                    #region Tipo valor: porcentaje

                                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                        oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value);

                                                        if (oPercentValue == Convert.ToDouble(rule.Value))
                                                        {
                                                            AdDocScore = int.Parse(rule.Score);

                                                            RuleScore++;

                                                            oTotalModuleScore += AdDocScore;

                                                            mprule.ItemInfoScore = AdDocScore;
                                                        }
                                                        else
                                                        {
                                                            mprule.ItemInfoScore = 0;
                                                        }

                                                        break;

                                                    #endregion

                                                    #region Tipo valor: texto

                                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Text:

                                                        oTextValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeText(pinf.ItemInfo.FirstOrDefault().Value);

                                                        if (oTextValue == rule.Value)
                                                        {
                                                            AdDocScore = int.Parse(rule.Score);

                                                            RuleScore++;

                                                            oTotalModuleScore += AdDocScore;

                                                            mprule.ItemInfoScore = AdDocScore;
                                                        }
                                                        else
                                                        {
                                                            mprule.ItemInfoScore = 0;
                                                        }

                                                        break;

                                                        #endregion
                                                }

                                                break;

                                            #endregion

                                            #region Entre

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.Entre:

                                                switch (rule.ValueType.ItemId)
                                                {
                                                    #region Tipo valor: numérico

                                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                        int minValue = 0;
                                                        int maxValue = 0;

                                                        string[] oValue = rule.Value.Split(',');

                                                        minValue = int.Parse(oValue[0]);
                                                        maxValue = int.Parse(oValue[1]);

                                                        oDecimalValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDecimal(pinf.ItemInfo.FirstOrDefault().Value);

                                                        if (oDecimalValue < maxValue && oDecimalValue > minValue)
                                                        {
                                                            AdDocScore = int.Parse(rule.Score);

                                                            RuleScore++;

                                                            oTotalModuleScore += AdDocScore;

                                                            mprule.ItemInfoScore = AdDocScore;
                                                        }
                                                        else
                                                        {
                                                            mprule.ItemInfoScore = 0;
                                                        }

                                                        break;

                                                    #endregion

                                                    #region Tipo valor: fecha

                                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                        oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value);

                                                        DateTime oMinValue;
                                                        DateTime oMaxValue;

                                                        oValue = rule.Value.Split(',');

                                                        System.Globalization.CultureInfo cultureinfo =
                                                            new System.Globalization.CultureInfo("nl-NL");
                                                        DateTime min = DateTime.Parse(oValue[0].Trim(), cultureinfo);
                                                        DateTime max = DateTime.Parse(oValue[1].Trim(), cultureinfo);

                                                        oMinValue = min;
                                                        oMaxValue = max;

                                                        if (oDateValue < oMaxValue && oDateValue > oMinValue)
                                                        {
                                                            AdDocScore = int.Parse(rule.Score);

                                                            RuleScore++;

                                                            oTotalModuleScore += AdDocScore;

                                                            mprule.ItemInfoScore = AdDocScore;
                                                        }
                                                        else
                                                        {
                                                            mprule.ItemInfoScore = 0;
                                                        }

                                                        break;

                                                    #endregion

                                                    #region Tipo valor: porcentaje

                                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                        oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value);

                                                        double oMiniValue;
                                                        double oMaxiValue;

                                                        oValue = rule.Value.Split(',');

                                                        oMiniValue = Convert.ToDouble(oValue[0].Trim());
                                                        oMaxiValue = Convert.ToDouble(oValue[1].Trim());

                                                        if (oPercentValue < oMaxiValue && oPercentValue > oMiniValue)
                                                        {
                                                            AdDocScore = int.Parse(rule.Score);

                                                            RuleScore++;

                                                            oTotalModuleScore += AdDocScore;

                                                            mprule.ItemInfoScore = AdDocScore;
                                                        }
                                                        else
                                                        {
                                                            mprule.ItemInfoScore = 0;
                                                        }

                                                        break;

                                                        #endregion
                                                }

                                                break;

                                            #endregion

                                            #region Pasa / No pasa

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.PasaNoPasa:

                                                switch (rule.ValueType.ItemId)
                                                {
                                                    #region Tipo valor: archivo

                                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.File:

                                                        bool oRelatedFile = !string.IsNullOrEmpty(pinf.ItemInfo.FirstOrDefault().Value) || !string.IsNullOrEmpty(pinf.ItemInfo.FirstOrDefault().LargeValue) ? true : false;

                                                        if (oRelatedFile)
                                                        {
                                                            AdDocScore = int.Parse(rule.Score);

                                                            RuleScore++;

                                                            oTotalModuleScore += AdDocScore;

                                                            mprule.ItemInfoScore = AdDocScore;
                                                        }
                                                        else
                                                        {
                                                            mprule.ItemInfoScore = 0;
                                                        }

                                                        break;

                                                    #endregion

                                                    #region Tipo valor: texto

                                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Text:

                                                        if (!string.IsNullOrEmpty(pinf.ItemInfo.FirstOrDefault().Value) || !string.IsNullOrEmpty(pinf.ItemInfo.FirstOrDefault().LargeValue))
                                                        {
                                                            AdDocScore = int.Parse(rule.Score);

                                                            RuleScore++;

                                                            oTotalModuleScore += AdDocScore;

                                                            mprule.ItemInfoScore = AdDocScore;
                                                        }
                                                        else
                                                        {
                                                            mprule.ItemInfoScore = 0;
                                                        }

                                                        break;

                                                    #endregion

                                                    #region Tipo valor: booleano

                                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Boolean:

                                                        oTextValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeText(pinf.ItemInfo.FirstOrDefault().Value);

                                                        oBooleanValue = !string.IsNullOrEmpty(oTextValue) && (oTextValue == "1" || oTextValue == "True" || oTextValue == "true") ? true : false;

                                                        if (oBooleanValue)
                                                        {
                                                            AdDocScore = int.Parse(rule.Score);

                                                            RuleScore++;

                                                            oTotalModuleScore += AdDocScore;

                                                            mprule.ItemInfoScore = AdDocScore;
                                                        }
                                                        else
                                                        {
                                                            mprule.ItemInfoScore = 0;
                                                        }

                                                        break;

                                                        #endregion
                                                }

                                                break;

                                                #endregion
                                        }
                                    }
                                    return true;
                                });

                                RuleScore = 0;

                                return true;
                            });
                        }
                        else
                        {
                            var AditionalDocumentName = ProveedoresOnLine.CalificationBatch.Controller.CalificationProjectBatch.GetAditionalDocumentName(rule.Question.ItemId);
                            //update new rule
                            oAdDocProviderInfo = ProveedoresOnLine.CalificationBatch.Controller.CalificationProjectBatch.AditionalDocumentModuleInfo(CompanyPublicId, AditionalDocumentName);

                            oAdDocProviderInfo.Where(pinf => pinf != null).All(pinf =>
                            {
                                if (RuleScore <= 0)
                                {
                                    switch (rule.Rule.ItemId)
                                    {
                                        #region Positivo

                                        case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.Positivo:

                                            oDecimalValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDecimal(pinf.ItemInfo.FirstOrDefault().Value);

                                            if (oDecimalValue >= 0)
                                            {
                                                AdDocScore = int.Parse(rule.Score);

                                                RuleScore++;

                                                oTotalModuleScore += AdDocScore;

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = AdDocScore,
                                                    Enable = true,
                                                });
                                            }
                                            else
                                            {
                                                AdDocScore = 0;
                                            }

                                            break;

                                        #endregion

                                        #region Negativo

                                        case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.Negativo:

                                            oDecimalValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDecimal(pinf.ItemInfo.FirstOrDefault().Value);

                                            if (oDecimalValue < 0)
                                            {
                                                AdDocScore = int.Parse(rule.Score);

                                                RuleScore++;

                                                oTotalModuleScore += AdDocScore;

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = AdDocScore,
                                                    Enable = true,
                                                });
                                            }
                                            else
                                            {
                                                AdDocScore = 0;
                                            }

                                            break;

                                        #endregion

                                        #region MayorQue

                                        case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.MayorQue:

                                            switch (rule.ValueType.ItemId)
                                            {
                                                #region Tipo valor: numérico

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                    oDecimalValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDecimal(pinf.ItemInfo.FirstOrDefault().Value);

                                                    if (oDecimalValue > int.Parse(rule.Value))
                                                    {
                                                        AdDocScore = int.Parse(rule.Score);

                                                        RuleScore++;

                                                        oTotalModuleScore += AdDocScore;

                                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                        {
                                                            CalificationProjectItemInfoId = 0,
                                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                            {
                                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                            },
                                                            ItemInfoScore = AdDocScore,
                                                            Enable = true,
                                                        });
                                                    }
                                                    else
                                                    {
                                                        AdDocScore = 0;
                                                    }

                                                    break;

                                                #endregion

                                                #region Tipo valor: fecha

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                    oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value.ToString());

                                                    if (oDateValue > Convert.ToDateTime(rule.Value))
                                                    {
                                                        AdDocScore = int.Parse(rule.Score);

                                                        RuleScore++;

                                                        oTotalModuleScore += AdDocScore;

                                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                        {
                                                            CalificationProjectItemInfoId = 0,
                                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                            {
                                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                            },
                                                            ItemInfoScore = AdDocScore,
                                                            Enable = true,
                                                        });
                                                    }
                                                    else
                                                    {
                                                        AdDocScore = 0;
                                                    }

                                                    break;

                                                #endregion

                                                #region Tipo valor: porcentaje

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                    oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value);

                                                    if (oPercentValue > Convert.ToDouble(rule.Value))
                                                    {
                                                        AdDocScore = int.Parse(rule.Score);

                                                        RuleScore++;

                                                        oTotalModuleScore += AdDocScore;

                                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                        {
                                                            CalificationProjectItemInfoId = 0,
                                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                            {
                                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                            },
                                                            ItemInfoScore = AdDocScore,
                                                            Enable = true,
                                                        });
                                                    }
                                                    else
                                                    {
                                                        AdDocScore = 0;
                                                    }

                                                    break;

                                                    #endregion
                                            }

                                            break;

                                        #endregion

                                        #region MenorQue

                                        case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.MenorQue:

                                            switch (rule.ValueType.ItemId)
                                            {
                                                #region Tipo valor: numérico

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                    oDecimalValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDecimal(pinf.ItemInfo.FirstOrDefault().Value);

                                                    if (oDecimalValue < int.Parse(rule.Value))
                                                    {
                                                        AdDocScore = int.Parse(rule.Score);

                                                        RuleScore++;

                                                        oTotalModuleScore += AdDocScore;

                                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                        {
                                                            CalificationProjectItemInfoId = 0,
                                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                            {
                                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                            },
                                                            ItemInfoScore = AdDocScore,
                                                            Enable = true,
                                                        });
                                                    }
                                                    else
                                                    {
                                                        AdDocScore = 0;
                                                    }

                                                    break;

                                                #endregion

                                                #region Tipo valor: fecha

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                    oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value.ToString());

                                                    if (oDateValue < Convert.ToDateTime(rule.Value))
                                                    {
                                                        AdDocScore = int.Parse(rule.Score);

                                                        RuleScore++;

                                                        oTotalModuleScore += AdDocScore;

                                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                        {
                                                            CalificationProjectItemInfoId = 0,
                                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                            {
                                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                            },
                                                            ItemInfoScore = AdDocScore,
                                                            Enable = true,
                                                        });
                                                    }
                                                    else
                                                    {
                                                        AdDocScore = 0;
                                                    }

                                                    break;

                                                #endregion

                                                #region Tipo valor: porcentaje

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                    oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value);

                                                    if (oPercentValue < Convert.ToDouble(rule.Value))
                                                    {
                                                        AdDocScore = int.Parse(rule.Score);

                                                        RuleScore++;

                                                        oTotalModuleScore += AdDocScore;

                                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                        {
                                                            CalificationProjectItemInfoId = 0,
                                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                            {
                                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                            },
                                                            ItemInfoScore = AdDocScore,
                                                            Enable = true,
                                                        });
                                                    }
                                                    else
                                                    {
                                                        AdDocScore = 0;
                                                    }

                                                    break;

                                                    #endregion
                                            }

                                            break;

                                        #endregion

                                        #region MayorOIgual

                                        case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.MayorOIgual:

                                            switch (rule.ValueType.ItemId)
                                            {
                                                #region Tipo valor: numérico

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                    oDecimalValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDecimal(pinf.ItemInfo.FirstOrDefault().Value);

                                                    if (oDecimalValue >= int.Parse(rule.Value))
                                                    {
                                                        AdDocScore = int.Parse(rule.Score);

                                                        RuleScore++;

                                                        oTotalModuleScore += AdDocScore;

                                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                        {
                                                            CalificationProjectItemInfoId = 0,
                                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                            {
                                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                            },
                                                            ItemInfoScore = AdDocScore,
                                                            Enable = true,
                                                        });
                                                    }
                                                    else
                                                    {
                                                        AdDocScore = 0;
                                                    }

                                                    break;

                                                #endregion

                                                #region Tipo valor: fecha

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                    oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value.ToString());

                                                    if (oDateValue >= Convert.ToDateTime(rule.Value))
                                                    {
                                                        AdDocScore = int.Parse(rule.Score);

                                                        RuleScore++;

                                                        oTotalModuleScore += AdDocScore;

                                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                        {
                                                            CalificationProjectItemInfoId = 0,
                                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                            {
                                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                            },
                                                            ItemInfoScore = AdDocScore,
                                                            Enable = true,
                                                        });
                                                    }
                                                    else
                                                    {
                                                        AdDocScore = 0;
                                                    }

                                                    break;

                                                #endregion

                                                #region Tipo valor: porcentaje

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                    oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value);

                                                    if (oPercentValue >= Convert.ToDouble(rule.Value))
                                                    {
                                                        AdDocScore = int.Parse(rule.Score);

                                                        RuleScore++;

                                                        oTotalModuleScore += AdDocScore;

                                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                        {
                                                            CalificationProjectItemInfoId = 0,
                                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                            {
                                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                            },
                                                            ItemInfoScore = AdDocScore,
                                                            Enable = true,
                                                        });
                                                    }
                                                    else
                                                    {
                                                        AdDocScore = 0;
                                                    }

                                                    break;

                                                    #endregion
                                            }

                                            break;

                                        #endregion

                                        #region MenorOIgual

                                        case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.MenorOIgual:

                                            switch (rule.ValueType.ItemId)
                                            {
                                                #region Tipo valor: numérico

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                    oDecimalValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDecimal(pinf.ItemInfo.FirstOrDefault().Value);

                                                    if (oDecimalValue <= int.Parse(rule.Value))
                                                    {
                                                        AdDocScore = int.Parse(rule.Score);

                                                        RuleScore++;

                                                        oTotalModuleScore += AdDocScore;

                                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                        {
                                                            CalificationProjectItemInfoId = 0,
                                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                            {
                                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                            },
                                                            ItemInfoScore = AdDocScore,
                                                            Enable = true,
                                                        });
                                                    }
                                                    else
                                                    {
                                                        AdDocScore = 0;
                                                    }

                                                    break;

                                                #endregion

                                                #region Tipo valor: fecha

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                    oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value.ToString());

                                                    if (oDateValue <= Convert.ToDateTime(rule.Value))
                                                    {
                                                        AdDocScore = int.Parse(rule.Score);

                                                        RuleScore++;

                                                        oTotalModuleScore += AdDocScore;

                                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                        {
                                                            CalificationProjectItemInfoId = 0,
                                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                            {
                                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                            },
                                                            ItemInfoScore = AdDocScore,
                                                            Enable = true,
                                                        });
                                                    }
                                                    else
                                                    {
                                                        AdDocScore = 0;
                                                    }

                                                    break;

                                                #endregion

                                                #region Tipo valor: porcentaje

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                    oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value);

                                                    if (oPercentValue <= Convert.ToDouble(rule.Value))
                                                    {
                                                        AdDocScore = int.Parse(rule.Score);

                                                        RuleScore++;

                                                        oTotalModuleScore += AdDocScore;

                                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                        {
                                                            CalificationProjectItemInfoId = 0,
                                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                            {
                                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                            },
                                                            ItemInfoScore = AdDocScore,
                                                            Enable = true,
                                                        });
                                                    }
                                                    else
                                                    {
                                                        AdDocScore = 0;
                                                    }

                                                    break;

                                                    #endregion
                                            }

                                            break;

                                        #endregion

                                        #region IgualQue

                                        case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.IgualQue:

                                            switch (rule.ValueType.ItemId)
                                            {
                                                #region Tipo valor: numérico

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                    oDecimalValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDecimal(pinf.ItemInfo.FirstOrDefault().Value);

                                                    if (oDecimalValue == int.Parse(rule.Value))
                                                    {
                                                        AdDocScore = int.Parse(rule.Score);

                                                        RuleScore++;

                                                        oTotalModuleScore += AdDocScore;

                                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                        {
                                                            CalificationProjectItemInfoId = 0,
                                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                            {
                                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                            },
                                                            ItemInfoScore = AdDocScore,
                                                            Enable = true,
                                                        });
                                                    }
                                                    else
                                                    {
                                                        AdDocScore = 0;
                                                    }

                                                    break;

                                                #endregion

                                                #region Tipo valor: fecha

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                    oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value.ToString());

                                                    if (oDateValue == Convert.ToDateTime(rule.Value))
                                                    {
                                                        AdDocScore = int.Parse(rule.Score);

                                                        RuleScore++;

                                                        oTotalModuleScore += AdDocScore;

                                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                        {
                                                            CalificationProjectItemInfoId = 0,
                                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                            {
                                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                            },
                                                            ItemInfoScore = AdDocScore,
                                                            Enable = true,
                                                        });
                                                    }
                                                    else
                                                    {
                                                        AdDocScore = 0;
                                                    }

                                                    break;

                                                #endregion

                                                #region Tipo valor: porcentaje

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                    oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value);

                                                    if (oPercentValue == Convert.ToDouble(rule.Value))
                                                    {
                                                        AdDocScore = int.Parse(rule.Score);

                                                        RuleScore++;

                                                        oTotalModuleScore += AdDocScore;

                                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                        {
                                                            CalificationProjectItemInfoId = 0,
                                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                            {
                                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                            },
                                                            ItemInfoScore = AdDocScore,
                                                            Enable = true,
                                                        });
                                                    }
                                                    else
                                                    {
                                                        AdDocScore = 0;
                                                    }

                                                    break;

                                                #endregion

                                                #region Tipo valor: texto

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Text:

                                                    oTextValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeText(pinf.ItemInfo.FirstOrDefault().Value);

                                                    if (oTextValue == rule.Value)
                                                    {
                                                        AdDocScore = int.Parse(rule.Score);
                                                        RuleScore++;
                                                        oTotalModuleScore += AdDocScore;

                                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                        {
                                                            CalificationProjectItemInfoId = 0,
                                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                            {
                                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                            },
                                                            ItemInfoScore = AdDocScore,
                                                            Enable = true,
                                                        });
                                                    }
                                                    else
                                                    {
                                                        AdDocScore = 0;
                                                    }

                                                    break;

                                                    #endregion
                                            }

                                            break;

                                        #endregion

                                        #region Entre

                                        case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.Entre:

                                            switch (rule.ValueType.ItemId)
                                            {
                                                #region Tipo valor: numérico

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                    int minValue = 0;
                                                    int maxValue = 0;

                                                    string[] oValue = rule.Value.Split(',');

                                                    minValue = int.Parse(oValue[0]);
                                                    maxValue = int.Parse(oValue[1]);

                                                    oDecimalValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDecimal(pinf.ItemInfo.FirstOrDefault().Value);

                                                    if (oDecimalValue < maxValue && oDecimalValue > minValue)
                                                    {
                                                        AdDocScore = int.Parse(rule.Score);

                                                        RuleScore++;

                                                        oTotalModuleScore += AdDocScore;

                                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                        {
                                                            CalificationProjectItemInfoId = 0,
                                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                            {
                                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                            },
                                                            ItemInfoScore = AdDocScore,
                                                            Enable = true,
                                                        });
                                                    }
                                                    else
                                                    {
                                                        AdDocScore = 0;
                                                    }

                                                    break;

                                                #endregion

                                                #region Tipo valor: fecha

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                    oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value);

                                                    DateTime oMinValue;
                                                    DateTime oMaxValue;

                                                    oValue = rule.Value.Split(',');
                                                    System.Globalization.CultureInfo cultureinfo =
                                                            new System.Globalization.CultureInfo("nl-NL");
                                                    DateTime min = DateTime.Parse(oValue[0].Trim(), cultureinfo);
                                                    DateTime max = DateTime.Parse(oValue[1].Trim(), cultureinfo);

                                                    oMinValue = min;
                                                    oMaxValue = max;

                                                    if (oDateValue < oMaxValue && oDateValue > oMinValue)
                                                    {
                                                        AdDocScore = int.Parse(rule.Score);

                                                        RuleScore++;

                                                        oTotalModuleScore += AdDocScore;

                                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                        {
                                                            CalificationProjectItemInfoId = 0,
                                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                            {
                                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                            },
                                                            ItemInfoScore = AdDocScore,
                                                            Enable = true,
                                                        });
                                                    }
                                                    else
                                                    {
                                                        AdDocScore = 0;
                                                    }

                                                    break;

                                                #endregion

                                                #region Tipo valor: porcentaje

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                    oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value);

                                                    double oMiniValue;
                                                    double oMaxiValue;

                                                    oValue = rule.Value.Split(',');

                                                    oMiniValue = Convert.ToDouble(oValue[0].Trim());
                                                    oMaxiValue = Convert.ToDouble(oValue[1].Trim());

                                                    if (oPercentValue < oMaxiValue && oPercentValue > oMiniValue)
                                                    {
                                                        AdDocScore = int.Parse(rule.Score);

                                                        RuleScore++;

                                                        oTotalModuleScore += AdDocScore;

                                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                        {
                                                            CalificationProjectItemInfoId = 0,
                                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                            {
                                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                            },
                                                            ItemInfoScore = AdDocScore,
                                                            Enable = true,
                                                        });
                                                    }
                                                    else
                                                    {
                                                        AdDocScore = 0;
                                                    }

                                                    break;

                                                    #endregion
                                            }

                                            break;

                                        #endregion

                                        #region Pasa / No pasa

                                        case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.PasaNoPasa:

                                            switch (rule.ValueType.ItemId)
                                            {
                                                #region Tipo valor: archivo

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.File:

                                                    bool oRelatedFile = !string.IsNullOrEmpty(pinf.ItemInfo.FirstOrDefault().Value) || !string.IsNullOrEmpty(pinf.ItemInfo.FirstOrDefault().LargeValue) ? true : false;

                                                    if (oRelatedFile)
                                                    {
                                                        AdDocScore = int.Parse(rule.Score);

                                                        RuleScore++;

                                                        oTotalModuleScore += AdDocScore;

                                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                        {
                                                            CalificationProjectItemInfoId = 0,
                                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                            {
                                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                            },
                                                            ItemInfoScore = AdDocScore,
                                                            Enable = true,
                                                        });
                                                    }
                                                    else
                                                    {
                                                        AdDocScore = 0;
                                                    }

                                                    break;

                                                #endregion

                                                #region Tipo valor: texto

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Text:

                                                    if (!string.IsNullOrEmpty(pinf.ItemInfo.FirstOrDefault().Value) || !string.IsNullOrEmpty(pinf.ItemInfo.FirstOrDefault().LargeValue))
                                                    {
                                                        AdDocScore = int.Parse(rule.Score);

                                                        RuleScore++;

                                                        oTotalModuleScore += AdDocScore;

                                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                        {
                                                            CalificationProjectItemInfoId = 0,
                                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                            {
                                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                            },
                                                            ItemInfoScore = AdDocScore,
                                                            Enable = true,
                                                        });
                                                    }
                                                    else
                                                    {
                                                        AdDocScore = 0;
                                                    }

                                                    break;

                                                #endregion

                                                #region Tipo valor: booleano

                                                case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Boolean:

                                                    oTextValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeText(pinf.ItemInfo.FirstOrDefault().Value);

                                                    oBooleanValue = !string.IsNullOrEmpty(oTextValue) && (oTextValue == "1" || oTextValue == "True" || oTextValue == "true") ? true : false;

                                                    if (oBooleanValue)
                                                    {
                                                        AdDocScore = int.Parse(rule.Score);

                                                        RuleScore++;

                                                        oTotalModuleScore += AdDocScore;

                                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                        {
                                                            CalificationProjectItemInfoId = 0,
                                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                            {
                                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                            },
                                                            ItemInfoScore = AdDocScore,
                                                            Enable = true,
                                                        });
                                                    }
                                                    else
                                                    {
                                                        AdDocScore = 0;
                                                    }

                                                    break;

                                                    #endregion
                                            }

                                            break;

                                            #endregion
                                    }
                                }

                                return true;
                            });
                        }

                        RuleScore = 0;

                        return true;
                    });
                }
                else
                {
                    //add new rulw
                    oReturn.CalificatioProjectItemInfoModel = new List<CalificationProjectItemInfoBatchModel>();

                    oCalificationProjectItemModel.CalificationProjectConfigItemInfoModel.Where(cpitinf => cpitinf.Enable == true).All(cpitinf =>
                    {
                        ProveedoresOnLine.CalificationBatch.CalificationProcess.LogFile("Create validate to Aditional Document module ::: Provider public id ::: " + CompanyPublicId + " ::: RuleId ::: " + cpitinf.CalificationProjectConfigItemInfoId);

                        var AditionalDocumentName = ProveedoresOnLine.CalificationBatch.Controller.CalificationProjectBatch.GetAditionalDocumentName(cpitinf.Question.ItemId);
                        //update new rule
                        oAdDocProviderInfo = ProveedoresOnLine.CalificationBatch.Controller.CalificationProjectBatch.AditionalDocumentModuleInfo(CompanyPublicId, AditionalDocumentName);

                        oAdDocProviderInfo.Where(pinf => pinf != null).All(pinf =>
                        {
                            if (RuleScore <= 0)
                            {
                                switch (cpitinf.Rule.ItemId)
                                {
                                    #region Positivo

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.Positivo:

                                        oDecimalValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDecimal(pinf.ItemInfo.FirstOrDefault().Value);

                                        if (oDecimalValue >= 0)
                                        {
                                            AdDocScore = int.Parse(cpitinf.Score);

                                            RuleScore++;

                                            oTotalModuleScore += AdDocScore;

                                            oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                            {
                                                CalificationProjectItemInfoId = 0,
                                                CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                {
                                                    CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                },
                                                ItemInfoScore = AdDocScore,
                                                Enable = true,
                                            });
                                        }
                                        else
                                        {
                                            AdDocScore = 0;
                                        }

                                        break;

                                    #endregion

                                    #region Negativo

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.Negativo:

                                        oDecimalValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDecimal(pinf.ItemInfo.FirstOrDefault().Value);

                                        if (oDecimalValue < 0)
                                        {
                                            AdDocScore = int.Parse(cpitinf.Score);

                                            RuleScore++;

                                            oTotalModuleScore += AdDocScore;

                                            oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                            {
                                                CalificationProjectItemInfoId = 0,
                                                CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                {
                                                    CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                },
                                                ItemInfoScore = AdDocScore,
                                                Enable = true,
                                            });
                                        }
                                        else
                                        {
                                            AdDocScore = 0;
                                        }

                                        break;

                                    #endregion

                                    #region MayorQue

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.MayorQue:

                                        switch (cpitinf.ValueType.ItemId)
                                        {
                                            #region Tipo valor: numérico

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                oDecimalValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDecimal(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oDecimalValue > int.Parse(cpitinf.Value))
                                                {
                                                    AdDocScore = int.Parse(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += AdDocScore;

                                                    oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                    {
                                                        CalificationProjectItemInfoId = 0,
                                                        CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                        {
                                                            CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                        },
                                                        ItemInfoScore = AdDocScore,
                                                        Enable = true,
                                                    });
                                                }
                                                else
                                                {
                                                    AdDocScore = 0;
                                                }

                                                break;

                                            #endregion

                                            #region Tipo valor: fecha

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value.ToString());

                                                if (oDateValue > Convert.ToDateTime(cpitinf.Value))
                                                {
                                                    AdDocScore = int.Parse(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += AdDocScore;

                                                    oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                    {
                                                        CalificationProjectItemInfoId = 0,
                                                        CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                        {
                                                            CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                        },
                                                        ItemInfoScore = AdDocScore,
                                                        Enable = true,
                                                    });
                                                }
                                                else
                                                {
                                                    AdDocScore = 0;
                                                }

                                                break;

                                            #endregion

                                            #region Tipo valor: porcentaje

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oPercentValue > Convert.ToDouble(cpitinf.Value))
                                                {
                                                    AdDocScore = int.Parse(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += AdDocScore;

                                                    oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                    {
                                                        CalificationProjectItemInfoId = 0,
                                                        CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                        {
                                                            CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                        },
                                                        ItemInfoScore = AdDocScore,
                                                        Enable = true,
                                                    });
                                                }
                                                else
                                                {
                                                    AdDocScore = 0;
                                                }

                                                break;

                                                #endregion
                                        }

                                        break;

                                    #endregion

                                    #region MenorQue

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.MenorQue:

                                        switch (cpitinf.ValueType.ItemId)
                                        {
                                            #region Tipo valor: numérico

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                oDecimalValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDecimal(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oDecimalValue < int.Parse(cpitinf.Value))
                                                {
                                                    AdDocScore = int.Parse(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += AdDocScore;

                                                    oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                    {
                                                        CalificationProjectItemInfoId = 0,
                                                        CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                        {
                                                            CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                        },
                                                        ItemInfoScore = AdDocScore,
                                                        Enable = true,
                                                    });
                                                }
                                                else
                                                {
                                                    AdDocScore = 0;
                                                }

                                                break;

                                            #endregion

                                            #region Tipo valor: fecha

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value.ToString());

                                                if (oDateValue < Convert.ToDateTime(cpitinf.Value))
                                                {
                                                    AdDocScore = int.Parse(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += AdDocScore;

                                                    oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                    {
                                                        CalificationProjectItemInfoId = 0,
                                                        CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                        {
                                                            CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                        },
                                                        ItemInfoScore = AdDocScore,
                                                        Enable = true,
                                                    });
                                                }
                                                else
                                                {
                                                    AdDocScore = 0;
                                                }

                                                break;

                                            #endregion

                                            #region Tipo valor: porcentaje

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oPercentValue < Convert.ToDouble(cpitinf.Value))
                                                {
                                                    AdDocScore = int.Parse(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += AdDocScore;

                                                    oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                    {
                                                        CalificationProjectItemInfoId = 0,
                                                        CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                        {
                                                            CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                        },
                                                        ItemInfoScore = AdDocScore,
                                                        Enable = true,
                                                    });
                                                }
                                                else
                                                {
                                                    AdDocScore = 0;
                                                }

                                                break;

                                                #endregion
                                        }

                                        break;

                                    #endregion

                                    #region MayorOIgual

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.MayorOIgual:

                                        switch (cpitinf.ValueType.ItemId)
                                        {
                                            #region Tipo valor: numérico

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                oDecimalValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDecimal(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oDecimalValue >= int.Parse(cpitinf.Value))
                                                {
                                                    AdDocScore = int.Parse(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += AdDocScore;

                                                    oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                    {
                                                        CalificationProjectItemInfoId = 0,
                                                        CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                        {
                                                            CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                        },
                                                        ItemInfoScore = AdDocScore,
                                                        Enable = true,
                                                    });
                                                }
                                                else
                                                {
                                                    AdDocScore = 0;
                                                }

                                                break;

                                            #endregion

                                            #region Tipo valor: fecha

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value.ToString());

                                                if (oDateValue >= Convert.ToDateTime(cpitinf.Value))
                                                {
                                                    AdDocScore = int.Parse(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += AdDocScore;

                                                    oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                    {
                                                        CalificationProjectItemInfoId = 0,
                                                        CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                        {
                                                            CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                        },
                                                        ItemInfoScore = AdDocScore,
                                                        Enable = true,
                                                    });
                                                }
                                                else
                                                {
                                                    AdDocScore = 0;
                                                }

                                                break;

                                            #endregion

                                            #region Tipo valor: porcentaje

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oPercentValue >= Convert.ToDouble(cpitinf.Value))
                                                {
                                                    AdDocScore = int.Parse(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += AdDocScore;

                                                    oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                    {
                                                        CalificationProjectItemInfoId = 0,
                                                        CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                        {
                                                            CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                        },
                                                        ItemInfoScore = AdDocScore,
                                                        Enable = true,
                                                    });
                                                }
                                                else
                                                {
                                                    AdDocScore = 0;
                                                }

                                                break;

                                                #endregion
                                        }

                                        break;

                                    #endregion

                                    #region MenorOIgual

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.MenorOIgual:

                                        switch (cpitinf.ValueType.ItemId)
                                        {
                                            #region Tipo valor: numérico

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                oDecimalValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDecimal(pinf.ItemInfo.FirstOrDefault().Value);


                                                if (oDecimalValue <= int.Parse(cpitinf.Value))
                                                {
                                                    AdDocScore = int.Parse(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += AdDocScore;

                                                    oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                    {
                                                        CalificationProjectItemInfoId = 0,
                                                        CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                        {
                                                            CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                        },
                                                        ItemInfoScore = AdDocScore,
                                                        Enable = true,
                                                    });
                                                }
                                                else
                                                {
                                                    AdDocScore = 0;
                                                }

                                                break;

                                            #endregion

                                            #region Tipo valor: fecha

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value.ToString());

                                                if (oDateValue <= Convert.ToDateTime(cpitinf.Value))
                                                {
                                                    AdDocScore = int.Parse(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += AdDocScore;

                                                    oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                    {
                                                        CalificationProjectItemInfoId = 0,
                                                        CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                        {
                                                            CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                        },
                                                        ItemInfoScore = AdDocScore,
                                                        Enable = true,
                                                    });
                                                }
                                                else
                                                {
                                                    AdDocScore = 0;
                                                }

                                                break;

                                            #endregion

                                            #region Tipo valor: porcentaje

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oPercentValue <= Convert.ToDouble(cpitinf.Value))
                                                {
                                                    AdDocScore = int.Parse(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += AdDocScore;

                                                    oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                    {
                                                        CalificationProjectItemInfoId = 0,
                                                        CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                        {
                                                            CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                        },
                                                        ItemInfoScore = AdDocScore,
                                                        Enable = true,
                                                    });
                                                }
                                                else
                                                {
                                                    AdDocScore = 0;
                                                }

                                                break;

                                                #endregion
                                        }

                                        break;

                                    #endregion

                                    #region IgualQue

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.IgualQue:

                                        switch (cpitinf.ValueType.ItemId)
                                        {
                                            #region Tipo valor: numérico

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                oDecimalValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDecimal(pinf.ItemInfo.FirstOrDefault().Value);


                                                if (oDecimalValue == int.Parse(cpitinf.Value))
                                                {
                                                    AdDocScore = int.Parse(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += AdDocScore;

                                                    oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                    {
                                                        CalificationProjectItemInfoId = 0,
                                                        CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                        {
                                                            CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                        },
                                                        ItemInfoScore = AdDocScore,
                                                        Enable = true,
                                                    });
                                                }
                                                else
                                                {
                                                    AdDocScore = 0;
                                                }

                                                break;

                                            #endregion

                                            #region Tipo valor: fecha

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value.ToString());

                                                if (oDateValue == Convert.ToDateTime(cpitinf.Value))
                                                {
                                                    AdDocScore = int.Parse(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += AdDocScore;

                                                    oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                    {
                                                        CalificationProjectItemInfoId = 0,
                                                        CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                        {
                                                            CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                        },
                                                        ItemInfoScore = AdDocScore,
                                                        Enable = true,
                                                    });
                                                }
                                                else
                                                {
                                                    AdDocScore = 0;
                                                }

                                                break;

                                            #endregion

                                            #region Tipo valor: porcentaje

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oPercentValue == Convert.ToDouble(cpitinf.Value))
                                                {
                                                    AdDocScore = int.Parse(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += AdDocScore;

                                                    oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                    {
                                                        CalificationProjectItemInfoId = 0,
                                                        CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                        {
                                                            CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                        },
                                                        ItemInfoScore = AdDocScore,
                                                        Enable = true,
                                                    });
                                                }
                                                else
                                                {
                                                    AdDocScore = 0;
                                                }

                                                break;

                                            #endregion

                                            #region Tipo valor: texto

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Text:

                                                oTextValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeText(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oTextValue == cpitinf.Value)
                                                {
                                                    AdDocScore = int.Parse(cpitinf.Score);
                                                    RuleScore++;
                                                    oTotalModuleScore += AdDocScore;
                                                }
                                                else
                                                {
                                                    AdDocScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = AdDocScore,
                                                    Enable = true,
                                                });
                                                break;

                                                #endregion
                                        }

                                        break;

                                    #endregion

                                    #region Entre

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.Entre:

                                        switch (cpitinf.ValueType.ItemId)
                                        {
                                            #region Tipo valor: numérico

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                int minValue = 0;
                                                int maxValue = 0;

                                                string[] oValue = cpitinf.Value.Split(',');

                                                minValue = int.Parse(oValue[0]);
                                                maxValue = int.Parse(oValue[1]);

                                                oDecimalValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDecimal(pinf.ItemInfo.FirstOrDefault().Value);


                                                if (oDecimalValue < maxValue && oDecimalValue > minValue)
                                                {
                                                    AdDocScore = int.Parse(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += AdDocScore;

                                                    oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                    {
                                                        CalificationProjectItemInfoId = 0,
                                                        CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                        {
                                                            CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                        },
                                                        ItemInfoScore = AdDocScore,
                                                        Enable = true,
                                                    });
                                                }
                                                else
                                                {
                                                    AdDocScore = 0;
                                                }

                                                break;

                                            #endregion

                                            #region Tipo valor: fecha

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value);

                                                DateTime oMinValue;
                                                DateTime oMaxValue;

                                                oValue = cpitinf.Value.Split(',');
                                                System.Globalization.CultureInfo cultureinfo =
                                                            new System.Globalization.CultureInfo("nl-NL");
                                                DateTime min = DateTime.Parse(oValue[0].Trim(), cultureinfo);
                                                DateTime max = DateTime.Parse(oValue[1].Trim(), cultureinfo);

                                                oMinValue = min;
                                                oMaxValue = max;

                                                if (oDateValue < oMaxValue && oDateValue > oMinValue)
                                                {
                                                    AdDocScore = int.Parse(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += AdDocScore;

                                                    oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                    {
                                                        CalificationProjectItemInfoId = 0,
                                                        CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                        {
                                                            CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                        },
                                                        ItemInfoScore = AdDocScore,
                                                        Enable = true,
                                                    });
                                                }
                                                else
                                                {
                                                    AdDocScore = 0;
                                                }

                                                break;

                                            #endregion

                                            #region Tipo valor: porcentaje

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value);

                                                double oMiniValue;
                                                double oMaxiValue;

                                                oValue = cpitinf.Value.Split(',');

                                                oMiniValue = Convert.ToDouble(oValue[0].Trim());
                                                oMaxiValue = Convert.ToDouble(oValue[1].Trim());

                                                if (oPercentValue < oMaxiValue && oPercentValue > oMiniValue)
                                                {
                                                    AdDocScore = int.Parse(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += AdDocScore;

                                                    oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                    {
                                                        CalificationProjectItemInfoId = 0,
                                                        CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                        {
                                                            CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                        },
                                                        ItemInfoScore = AdDocScore,
                                                        Enable = true,
                                                    });
                                                }
                                                else
                                                {
                                                    AdDocScore = 0;
                                                }

                                                break;

                                                #endregion
                                        }

                                        break;

                                    #endregion

                                    #region Pasa / No pasa

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.PasaNoPasa:

                                        switch (cpitinf.ValueType.ItemId)
                                        {
                                            #region Tipo valor: archivo

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.File:

                                                bool oRelatedFile = !string.IsNullOrEmpty(pinf.ItemInfo.FirstOrDefault().Value) || !string.IsNullOrEmpty(pinf.ItemInfo.FirstOrDefault().LargeValue) ? true : false;

                                                if (oRelatedFile)
                                                {
                                                    AdDocScore = int.Parse(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += AdDocScore;

                                                    oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                    {
                                                        CalificationProjectItemInfoId = 0,
                                                        CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                        {
                                                            CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                        },
                                                        ItemInfoScore = AdDocScore,
                                                        Enable = true,
                                                    });
                                                }
                                                else
                                                {
                                                    AdDocScore = 0;
                                                }

                                                break;

                                            #endregion

                                            #region Tipo valor: texto

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Text:

                                                if (!string.IsNullOrEmpty(pinf.ItemInfo.FirstOrDefault().Value) || !string.IsNullOrEmpty(pinf.ItemInfo.FirstOrDefault().LargeValue))
                                                {
                                                    AdDocScore = int.Parse(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += AdDocScore;

                                                    oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                    {
                                                        CalificationProjectItemInfoId = 0,
                                                        CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                        {
                                                            CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                        },
                                                        ItemInfoScore = AdDocScore,
                                                        Enable = true,
                                                    });
                                                }
                                                else
                                                {
                                                    AdDocScore = 0;
                                                }

                                                break;

                                            #endregion

                                            #region Tipo valor: booleano

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Boolean:

                                                oTextValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeText(pinf.ItemInfo.FirstOrDefault().Value);

                                                oBooleanValue = !string.IsNullOrEmpty(oTextValue) && (oTextValue == "1" || oTextValue == "True" || oTextValue == "true") ? true : false;

                                                if (oBooleanValue)
                                                {
                                                    AdDocScore = int.Parse(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += AdDocScore;

                                                    oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                    {
                                                        CalificationProjectItemInfoId = 0,
                                                        CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                        {
                                                            CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                        },
                                                        ItemInfoScore = AdDocScore,
                                                        Enable = true,
                                                    });
                                                }
                                                else
                                                {
                                                    AdDocScore = 0;
                                                }

                                                break;

                                                #endregion
                                        }

                                        break;

                                        #endregion
                                }
                            }

                            return true;
                        });

                        RuleScore = 0;

                        return true;
                    });
                }

                ProveedoresOnLine.CalificationBatch.CalificationProcess.LogFile("End Aditional Document module process::: Provider public id::: " + CompanyPublicId);
            }
            catch (Exception err)
            {
                ProveedoresOnLine.CalificationBatch.CalificationProcess.LogFile("Fatal error:: Aditional Document Module :: " + err.Message + " - " + err.StackTrace + " InnerException: " + err.InnerException);
            }

            //Get new score
            oReturn.ItemScore = oTotalModuleScore;

            return oReturn;
        }
    }
}