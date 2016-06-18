﻿using ProveedoresOnLine.CalificationBatch.Models.CalificationProjectBatch;
using ProveedoresOnLine.CalificationProject.Models.CalificationProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.CalificationBatch.CalificationProjectModule
{
    public class LegalModule
    {
        public static ProveedoresOnLine.CalificationBatch.Models.CalificationProjectBatch.CalificationProjectItemBatchModel LegalRule(string CompanyPublicId, ConfigItemModel oCalificationProjectItemModel, CalificationProjectItemBatchModel oRelatedCalificationProjectItemModel)
        {
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

            if (oRelatedCalificationProjectItemModel != null &&
                oRelatedCalificationProjectItemModel.CalificatioProjectItemInfoModel != null &&
                oRelatedCalificationProjectItemModel.CalificatioProjectItemInfoModel.Count > 0)
            {
                oReturn.CalificatioProjectItemInfoModel = oRelatedCalificationProjectItemModel.CalificatioProjectItemInfoModel;
            }

            List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> oLegalProviderInfo;

            #region Variables

            int oTotalModuleScore = 0;
            int LegalScore = 0;
            int RuleScore = 0;
            int oIntValue = 0;
            bool oBooleanValue = true;
            double oPercentValue = 0;
            DateTime oDateValue = new DateTime();

            #endregion

            try
            {
                if (oReturn.CalificatioProjectItemInfoModel != null &&
                    oReturn.CalificatioProjectItemInfoModel.Count > 0)
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

                    //Get last calification
                    oReturn.CalificatioProjectItemInfoModel.Where(cpitinf => cpitinf.CalificationProjectConfigItemInfoModel.LastModify <= cpitinf.LastModify && cpitinf.Enable == true).All(cpitinf =>
                    {
                        oTotalModuleScore += cpitinf.ItemInfoScore;
                        return true;
                    });

                    oCalificationProjectItemModel.CalificationProjectConfigItemInfoModel.All(rule =>
                    {
                        if (oRelatedCalificationProjectItemModel.CalificatioProjectItemInfoModel.Any(mprule => mprule.CalificationProjectConfigItemInfoModel.CalificationProjectConfigItemInfoId == rule.CalificationProjectConfigItemInfoId))
                        {
                            oRelatedCalificationProjectItemModel.CalificatioProjectItemInfoModel.Where(mprule => mprule.CalificationProjectConfigItemInfoModel.CalificationProjectConfigItemInfoId == rule.CalificationProjectConfigItemInfoId).All(mprule =>
                            {
                                //add mp rule

                                return true;
                            });
                        }
                        else
                        {
                            //run rule
                            oLegalProviderInfo = ProveedoresOnLine.CalificationBatch.Controller.CalificationProjectBatch.LegalModuleInfo(CompanyPublicId, rule.Question);

                            oLegalProviderInfo.Where(pinf => pinf != null).All(pinf =>
                            {
                                switch (rule.Rule.ItemId)
                                {
                                    #region Positivo

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.Positivo:

                                        oIntValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeNumeric(pinf.ItemInfo.FirstOrDefault().Value);

                                        if (oIntValue >= 0)
                                        {
                                            LegalScore = Convert.ToInt32(rule.Score);

                                            RuleScore++;

                                            oTotalModuleScore += LegalScore;
                                        }
                                        else
                                        {
                                            LegalScore = 0;
                                        }

                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                        {
                                            CalificationProjectItemInfoId = 0,
                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                            {
                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                            },
                                            ItemInfoScore = LegalScore,
                                            Enable = true,
                                        });

                                        break;

                                    #endregion

                                    #region Negativo

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.Negativo:

                                        oIntValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeNumeric(pinf.ItemInfo.FirstOrDefault().Value);

                                        if (oIntValue < 0)
                                        {
                                            LegalScore = Convert.ToInt32(rule.Score);

                                            RuleScore++;

                                            oTotalModuleScore += LegalScore;
                                        }
                                        else
                                        {
                                            LegalScore = 0;
                                        }

                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                        {
                                            CalificationProjectItemInfoId = 0,
                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                            {
                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                            },
                                            ItemInfoScore = LegalScore,
                                            Enable = true,
                                        });

                                        break;

                                    #endregion

                                    #region MayorQue

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.MayorQue:

                                        switch (rule.ValueType.ItemId)
                                        {
                                            #region Tipo valor: numérico

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                oIntValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeNumeric(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oIntValue > Convert.ToInt32(rule.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(rule.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

                                                break;

                                            #endregion

                                            #region Tipo valor: fecha

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value.ToString());

                                                if (oDateValue > Convert.ToDateTime(rule.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(rule.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

                                                break;

                                            #endregion

                                            #region Tipo valor: porcentaje

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value.Trim());

                                                if (oPercentValue > Convert.ToDouble(rule.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(rule.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

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

                                                oIntValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeNumeric(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oIntValue < Convert.ToInt32(rule.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(rule.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

                                                break;

                                            #endregion

                                            #region Tipo valor: fecha

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value.ToString());

                                                if (oDateValue < Convert.ToDateTime(rule.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(rule.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

                                                break;

                                            #endregion

                                            #region Tipo valor: porcentaje

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value.Trim());

                                                if (oPercentValue < Convert.ToDouble(rule.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(rule.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

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

                                                oIntValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeNumeric(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oIntValue >= Convert.ToInt32(rule.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(rule.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

                                                break;

                                            #endregion

                                            #region Tipo valor: fecha

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value.ToString());

                                                if (oDateValue >= Convert.ToDateTime(rule.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(rule.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

                                                break;

                                            #endregion

                                            #region Tipo valor: porcentaje

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value.Trim());

                                                if (oPercentValue >= Convert.ToDouble(rule.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(rule.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

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

                                                oIntValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeNumeric(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oIntValue <= Convert.ToInt32(rule.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(rule.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

                                                break;

                                            #endregion

                                            #region Tipo valor: fecha

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value.ToString());

                                                if (oDateValue <= Convert.ToDateTime(rule.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(rule.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

                                                break;

                                            #endregion

                                            #region Tipo valor: porcentaje

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value.Trim());

                                                if (oPercentValue <= Convert.ToDouble(rule.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(rule.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

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

                                                oIntValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeNumeric(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oIntValue == Convert.ToInt32(rule.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(rule.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

                                                break;

                                            #endregion

                                            #region Tipo valor: fecha

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value.ToString());

                                                if (oDateValue == Convert.ToDateTime(rule.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(rule.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

                                                break;

                                            #endregion

                                            #region Tipo valor: porcentaje

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value.Trim());

                                                if (oPercentValue == Convert.ToDouble(rule.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(rule.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

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

                                                minValue = Convert.ToInt32(oValue[0]);
                                                maxValue = Convert.ToInt32(oValue[1]);

                                                oIntValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeNumeric(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oIntValue < maxValue && oIntValue > minValue)
                                                {
                                                    LegalScore = Convert.ToInt32(rule.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                break;

                                            #endregion

                                            #region Tipo valor: fecha

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value.Trim());

                                                DateTime oMinValue;
                                                DateTime oMaxValue;

                                                oValue = rule.Value.Split(',');

                                                oMinValue = Convert.ToDateTime(oValue[0].Trim());
                                                oMaxValue = Convert.ToDateTime(oValue[1].Trim());

                                                if (oDateValue < oMaxValue && oDateValue > oMinValue)
                                                {
                                                    LegalScore = Convert.ToInt32(rule.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                break;

                                            #endregion

                                            #region Tipo valor: porcentaje

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value.Trim());

                                                double oMiniValue;
                                                double oMaxiValue;

                                                oValue = rule.Value.Split(',');

                                                oMiniValue = Convert.ToDouble(oValue[0].Trim());
                                                oMaxiValue = Convert.ToDouble(oValue[1].Trim());

                                                if (oPercentValue < oMaxiValue && oPercentValue > oMiniValue)
                                                {
                                                    LegalScore = Convert.ToInt32(rule.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                break;

                                            #endregion
                                        }

                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                        {
                                            CalificationProjectItemInfoId = 0,
                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                            {
                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                            },
                                            ItemInfoScore = LegalScore,
                                            Enable = true,
                                        });

                                        break;

                                    #endregion

                                    #region Pasa / No pasa

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.PasaNoPasa:

                                        //Validación por archivo
                                        if (rule.ValueType.ItemId == (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.File)
                                        {
                                            bool oRelatedFile = !string.IsNullOrEmpty(pinf.ItemInfo.FirstOrDefault().LargeValue) || !string.IsNullOrEmpty(pinf.ItemInfo.FirstOrDefault().Value) ? true : false;

                                            if (oRelatedFile)
                                            {
                                                LegalScore = Convert.ToInt32(rule.Score);

                                                RuleScore++;

                                                oTotalModuleScore += LegalScore;
                                            }
                                            else
                                            {
                                                LegalScore = 0;
                                            }
                                        }

                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                        {
                                            CalificationProjectItemInfoId = 0,
                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                            {
                                                CalificationProjectConfigItemInfoId = rule.CalificationProjectConfigItemInfoId,
                                            },
                                            ItemInfoScore = LegalScore,
                                            Enable = true,
                                        });

                                        break;

                                    #endregion
                                }

                                return true;
                            });
                        }

                        return true;
                    });


                    oReturn.CalificatioProjectItemInfoModel.Where(cpitinf => cpitinf.CalificationProjectConfigItemInfoModel.LastModify > cpitinf.LastModify && cpitinf.Enable == true).All(cpitinf =>
                    {
                        oLegalProviderInfo = ProveedoresOnLine.CalificationBatch.Controller.CalificationProjectBatch.LegalModuleInfo(CompanyPublicId, cpitinf.CalificationProjectConfigItemInfoModel.Question);

                        oLegalProviderInfo.Where(pinf => pinf != null).All(pinf =>
                        {
                            if (RuleScore <= 0)
                            {
                                switch (cpitinf.CalificationProjectConfigItemInfoModel.Rule.ItemId)
                                {
                                    #region Positivo

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.Positivo:

                                        oIntValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeNumeric(pinf.ItemInfo.FirstOrDefault().Value);

                                        if (oIntValue > 0)
                                        {
                                            LegalScore = Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Score);

                                            RuleScore++;

                                            oTotalModuleScore += LegalScore;
                                        }
                                        else
                                        {
                                            LegalScore = 0;
                                        }

                                        cpitinf.ItemInfoScore = LegalScore;

                                        break;

                                    #endregion

                                    #region Negativo

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.Negativo:

                                        oIntValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeNumeric(pinf.ItemInfo.FirstOrDefault().Value);

                                        if (oIntValue < 0)
                                        {
                                            LegalScore = Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Score);

                                            RuleScore++;

                                            oTotalModuleScore += LegalScore;
                                        }
                                        else
                                        {
                                            LegalScore = 0;
                                        }

                                        cpitinf.ItemInfoScore = LegalScore;

                                        break;

                                    #endregion

                                    #region MayorQue

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.MayorQue:

                                        switch (cpitinf.CalificationProjectConfigItemInfoModel.ValueType.ItemId)
                                        {
                                            #region Tipo valor: numérico

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                oIntValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeNumeric(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oIntValue > Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                cpitinf.ItemInfoScore = LegalScore;

                                                break;

                                            #endregion

                                            #region Tipo valor: fecha

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oDateValue > Convert.ToDateTime(cpitinf.CalificationProjectConfigItemInfoModel.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                cpitinf.ItemInfoScore = LegalScore;

                                                break;

                                            #endregion

                                            #region Tipo valor: porcentaje

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oPercentValue > Convert.ToDouble(cpitinf.CalificationProjectConfigItemInfoModel.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                cpitinf.ItemInfoScore = LegalScore;

                                                break;

                                            #endregion
                                        }

                                        break;

                                    #endregion

                                    #region MenorQue

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.MenorQue:

                                        switch (cpitinf.CalificationProjectConfigItemInfoModel.ValueType.ItemId)
                                        {
                                            #region Tipo valor: numérico

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                oIntValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeNumeric(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oIntValue < Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                cpitinf.ItemInfoScore = LegalScore;

                                                break;

                                            #endregion

                                            #region Tipo valor: fecha

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oDateValue < Convert.ToDateTime(cpitinf.CalificationProjectConfigItemInfoModel.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                cpitinf.ItemInfoScore = LegalScore;

                                                break;

                                            #endregion

                                            #region Tipo valor: porcentaje

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oPercentValue < Convert.ToDouble(cpitinf.CalificationProjectConfigItemInfoModel.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                cpitinf.ItemInfoScore = LegalScore;

                                                break;

                                            #endregion
                                        }

                                        break;

                                    #endregion

                                    #region MayorOIgual

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.MayorOIgual:

                                        switch (cpitinf.CalificationProjectConfigItemInfoModel.ValueType.ItemId)
                                        {
                                            #region Tipo valor: numérico

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                oIntValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeNumeric(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oIntValue >= Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                cpitinf.ItemInfoScore = LegalScore;

                                                break;

                                            #endregion

                                            #region Tipo valor: fecha

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oDateValue >= Convert.ToDateTime(cpitinf.CalificationProjectConfigItemInfoModel.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                cpitinf.ItemInfoScore = LegalScore;

                                                break;

                                            #endregion

                                            #region Tipo valor: porcentaje

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oPercentValue >= Convert.ToDouble(cpitinf.CalificationProjectConfigItemInfoModel.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                cpitinf.ItemInfoScore = LegalScore;

                                                break;

                                            #endregion
                                        }

                                        break;

                                    #endregion

                                    #region MenorOIgual

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.MenorOIgual:

                                        switch (cpitinf.CalificationProjectConfigItemInfoModel.ValueType.ItemId)
                                        {
                                            #region Tipo valor: numérico

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                oIntValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeNumeric(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oIntValue <= Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                cpitinf.ItemInfoScore = LegalScore;

                                                break;

                                            #endregion

                                            #region Tipo valor: fecha

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oDateValue <= Convert.ToDateTime(cpitinf.CalificationProjectConfigItemInfoModel.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                cpitinf.ItemInfoScore = LegalScore;

                                                break;

                                            #endregion

                                            #region Tipo valor: porcentaje

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oPercentValue <= Convert.ToDouble(cpitinf.CalificationProjectConfigItemInfoModel.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                cpitinf.ItemInfoScore = LegalScore;

                                                break;

                                            #endregion
                                        }

                                        break;

                                    #endregion

                                    #region IgualQue

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.IgualQue:

                                        switch (cpitinf.CalificationProjectConfigItemInfoModel.ValueType.ItemId)
                                        {
                                            #region Tipo valor: numérico

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                oIntValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeNumeric(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oIntValue == Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                cpitinf.ItemInfoScore = LegalScore;

                                                break;

                                            #endregion

                                            #region Tipo valor: fecha

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oDateValue == Convert.ToDateTime(cpitinf.CalificationProjectConfigItemInfoModel.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                cpitinf.ItemInfoScore = LegalScore;

                                                break;

                                            #endregion

                                            #region Tipo valor: porcentaje

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oPercentValue == Convert.ToDouble(cpitinf.CalificationProjectConfigItemInfoModel.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                cpitinf.ItemInfoScore = LegalScore;

                                                break;

                                            #endregion
                                        }

                                        break;

                                    #endregion

                                    #region Entre

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.Entre:

                                        switch (cpitinf.CalificationProjectConfigItemInfoModel.ValueType.ItemId)
                                        {
                                            #region Tipo valor: numérico

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                int minValue = 0;
                                                int maxValue = 0;

                                                string[] oValue = cpitinf.CalificationProjectConfigItemInfoModel.Value.Split(',');

                                                minValue = Convert.ToInt32(oValue[0]);
                                                maxValue = Convert.ToInt32(oValue[1]);

                                                oIntValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeNumeric(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oIntValue < maxValue && oIntValue > minValue)
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                cpitinf.ItemInfoScore = LegalScore;

                                                break;

                                            #endregion

                                            #region Tipo valor: fecha

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value);

                                                DateTime oMinValue;
                                                DateTime oMaxValue;

                                                oValue = cpitinf.CalificationProjectConfigItemInfoModel.Value.Split(',');

                                                oMinValue = Convert.ToDateTime(oValue[0].Trim());
                                                oMaxValue = Convert.ToDateTime(oValue[1].Trim());

                                                if (oDateValue < oMaxValue && oDateValue > oMinValue)
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                cpitinf.ItemInfoScore = LegalScore;

                                                break;

                                            #endregion

                                            #region Tipo valor: porcentaje

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value);

                                                double oMiniValue;
                                                double oMaxiValue;

                                                oValue = cpitinf.CalificationProjectConfigItemInfoModel.Value.Split(',');

                                                oMiniValue = Convert.ToDouble(oValue[0].Trim());
                                                oMaxiValue = Convert.ToDouble(oValue[1].Trim());

                                                if (oPercentValue < oMaxiValue && oPercentValue > oMiniValue)
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                cpitinf.ItemInfoScore = LegalScore;

                                                break;

                                            #endregion
                                        }

                                        break;

                                    #endregion

                                    #region Pasa / No pasa

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.PasaNoPasa:

                                        //Validación por archivo
                                        if (cpitinf.CalificationProjectConfigItemInfoModel.ValueType.ItemId == (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.File)
                                        {
                                            bool oRelatedFile = !string.IsNullOrEmpty(pinf.ItemInfo.FirstOrDefault().Value) || !string.IsNullOrEmpty(pinf.ItemInfo.FirstOrDefault().LargeValue) ? true : false;

                                            if (oRelatedFile)
                                            {
                                                LegalScore = Convert.ToInt32(cpitinf.CalificationProjectConfigItemInfoModel.Score);

                                                RuleScore++;

                                                oTotalModuleScore += LegalScore;
                                            }
                                            else
                                            {
                                                LegalScore = 0;
                                            }
                                        }

                                        cpitinf.ItemInfoScore = LegalScore;

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
                    oReturn.CalificatioProjectItemInfoModel = new List<CalificationProjectItemInfoBatchModel>();

                    oCalificationProjectItemModel.CalificationProjectConfigItemInfoModel.All(cpitinf =>
                    {
                        oLegalProviderInfo = ProveedoresOnLine.CalificationBatch.Controller.CalificationProjectBatch.LegalModuleInfo(CompanyPublicId, cpitinf.Question);

                        oLegalProviderInfo.Where(pinf => pinf != null).All(pinf =>
                        {
                            if (RuleScore <= 0)
                            {
                                switch (cpitinf.Rule.ItemId)
                                {
                                    #region Positivo

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.Positivo:

                                        oIntValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeNumeric(pinf.ItemInfo.FirstOrDefault().Value);

                                        if (oIntValue >= 0)
                                        {
                                            LegalScore = Convert.ToInt32(cpitinf.Score);

                                            RuleScore++;

                                            oTotalModuleScore += LegalScore;
                                        }
                                        else
                                        {
                                            LegalScore = 0;
                                        }

                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                        {
                                            CalificationProjectItemInfoId = 0,
                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                            {
                                                CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                            },
                                            ItemInfoScore = LegalScore,
                                            Enable = true,
                                        });

                                        break;

                                    #endregion

                                    #region Negativo

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.Negativo:

                                        oIntValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeNumeric(pinf.ItemInfo.FirstOrDefault().Value);

                                        if (oIntValue < 0)
                                        {
                                            LegalScore = Convert.ToInt32(cpitinf.Score);

                                            RuleScore++;

                                            oTotalModuleScore += LegalScore;
                                        }
                                        else
                                        {
                                            LegalScore = 0;
                                        }

                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                        {
                                            CalificationProjectItemInfoId = 0,
                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                            {
                                                CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                            },
                                            ItemInfoScore = LegalScore,
                                            Enable = true,
                                        });

                                        break;

                                    #endregion

                                    #region MayorQue

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.MayorQue:

                                        switch (cpitinf.ValueType.ItemId)
                                        {
                                            #region Tipo valor: numérico

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Numeric:

                                                oIntValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeNumeric(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oIntValue > Convert.ToInt32(cpitinf.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

                                                break;

                                            #endregion

                                            #region Tipo valor: fecha

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value.ToString());

                                                if (oDateValue > Convert.ToDateTime(cpitinf.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

                                                break;

                                            #endregion

                                            #region Tipo valor: porcentaje

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value.Trim());

                                                if (oPercentValue > Convert.ToDouble(cpitinf.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

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

                                                oIntValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeNumeric(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oIntValue < Convert.ToInt32(cpitinf.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

                                                break;

                                            #endregion

                                            #region Tipo valor: fecha

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value.ToString());

                                                if (oDateValue < Convert.ToDateTime(cpitinf.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

                                                break;

                                            #endregion

                                            #region Tipo valor: porcentaje

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value.Trim());

                                                if (oPercentValue < Convert.ToDouble(cpitinf.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

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

                                                oIntValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeNumeric(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oIntValue >= Convert.ToInt32(cpitinf.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

                                                break;

                                            #endregion

                                            #region Tipo valor: fecha

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value.ToString());

                                                if (oDateValue >= Convert.ToDateTime(cpitinf.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

                                                break;

                                            #endregion

                                            #region Tipo valor: porcentaje

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value.Trim());

                                                if (oPercentValue >= Convert.ToDouble(cpitinf.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

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

                                                oIntValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeNumeric(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oIntValue <= Convert.ToInt32(cpitinf.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

                                                break;

                                            #endregion

                                            #region Tipo valor: fecha

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value.ToString());

                                                if (oDateValue <= Convert.ToDateTime(cpitinf.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

                                                break;

                                            #endregion

                                            #region Tipo valor: porcentaje

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value.Trim());

                                                if (oPercentValue <= Convert.ToDouble(cpitinf.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

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

                                                oIntValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeNumeric(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oIntValue == Convert.ToInt32(cpitinf.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

                                                break;

                                            #endregion

                                            #region Tipo valor: fecha

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value.ToString());

                                                if (oDateValue == Convert.ToDateTime(cpitinf.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
                                                    Enable = true,
                                                });

                                                break;

                                            #endregion

                                            #region Tipo valor: porcentaje

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value.Trim());

                                                if (oPercentValue == Convert.ToDouble(cpitinf.Value))
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                                {
                                                    CalificationProjectItemInfoId = 0,
                                                    CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                                    {
                                                        CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                                    },
                                                    ItemInfoScore = LegalScore,
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

                                                minValue = Convert.ToInt32(oValue[0]);
                                                maxValue = Convert.ToInt32(oValue[1]);

                                                oIntValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeNumeric(pinf.ItemInfo.FirstOrDefault().Value);

                                                if (oIntValue < maxValue && oIntValue > minValue)
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                break;

                                            #endregion

                                            #region Tipo valor: fecha

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Date:

                                                oDateValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypeDate(pinf.ItemInfo.FirstOrDefault().Value.Trim());

                                                DateTime oMinValue;
                                                DateTime oMaxValue;

                                                oValue = cpitinf.Value.Split(',');

                                                oMinValue = Convert.ToDateTime(oValue[0].Trim());
                                                oMaxValue = Convert.ToDateTime(oValue[1].Trim());

                                                if (oDateValue < oMaxValue && oDateValue > oMinValue)
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                break;

                                            #endregion

                                            #region Tipo valor: porcentaje

                                            case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.Percent:

                                                oPercentValue = ProveedoresOnLine.CalificationBatch.Util.UtilModule.ValueTypePercent(pinf.ItemInfo.FirstOrDefault().Value.Trim());

                                                double oMiniValue;
                                                double oMaxiValue;

                                                oValue = cpitinf.Value.Split(',');

                                                oMiniValue = Convert.ToDouble(oValue[0].Trim());
                                                oMaxiValue = Convert.ToDouble(oValue[1].Trim());

                                                if (oPercentValue < oMaxiValue && oPercentValue > oMiniValue)
                                                {
                                                    LegalScore = Convert.ToInt32(cpitinf.Score);

                                                    RuleScore++;

                                                    oTotalModuleScore += LegalScore;
                                                }
                                                else
                                                {
                                                    LegalScore = 0;
                                                }

                                                break;

                                            #endregion
                                        }

                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                        {
                                            CalificationProjectItemInfoId = 0,
                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                            {
                                                CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                            },
                                            ItemInfoScore = LegalScore,
                                            Enable = true,
                                        });

                                        break;

                                    #endregion

                                    #region Pasa / No pasa

                                    case (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumOperatorType.PasaNoPasa:

                                        //Validación por archivo
                                        if (cpitinf.ValueType.ItemId == (int)ProveedoresOnLine.CalificationBatch.Models.Enumerations.enumValueType.File)
                                        {
                                            bool oRelatedFile = !string.IsNullOrEmpty(pinf.ItemInfo.FirstOrDefault().LargeValue) || !string.IsNullOrEmpty(pinf.ItemInfo.FirstOrDefault().Value) ? true : false;

                                            if (oRelatedFile)
                                            {
                                                LegalScore = Convert.ToInt32(cpitinf.Score);

                                                RuleScore++;

                                                oTotalModuleScore += LegalScore;
                                            }
                                            else
                                            {
                                                LegalScore = 0;
                                            }
                                        }

                                        oReturn.CalificatioProjectItemInfoModel.Add(new CalificationProjectItemInfoBatchModel()
                                        {
                                            CalificationProjectItemInfoId = 0,
                                            CalificationProjectConfigItemInfoModel = new ConfigItemInfoModel()
                                            {
                                                CalificationProjectConfigItemInfoId = cpitinf.CalificationProjectConfigItemInfoId,
                                            },
                                            ItemInfoScore = LegalScore,
                                            Enable = true,
                                        });

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

                ProveedoresOnLine.CalificationBatch.CalificationProcess.LogFile("Se validaron las reglas legales del proveedor " + CompanyPublicId);
            }
            catch (Exception)
            {
                //ProveedoresOnLine.CalificationBatch.CalificationProcess.LogFile("Fatal error::" + err.Message + " - " + err.StackTrace);
            }

            //Get new score
            oReturn.ItemScore = oTotalModuleScore;

            return oReturn;
        }

        public static void NewRule()
        {
            CalificationProjectBatchModel oReturn = new CalificationProjectBatchModel()
            {

            };
        }

        public static void LastRule()
        {
            CalificationProjectBatchModel oReturn = new CalificationProjectBatchModel()
            {

            };
        }
    }
}