﻿using System;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Manager.General;

namespace WebCrawler.Manager.CrawlerInfo
{
    public class LegalInfo
    {
        public static List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> GetLegalInfo(string ParId, string PublicId)
        {
            List<ProveedoresOnLine.Company.Models.Util.GenericItemModel> oLegal = new List<ProveedoresOnLine.Company.Models.Util.GenericItemModel>();

            HtmlDocument HtmlDoc = WebCrawler.Manager.WebCrawlerManager.GetHtmlDocumnet(ParId, enumMenu.LegalInfo.ToString());

            if (HtmlDoc.DocumentNode.SelectNodes("//table[@class='administrador_tabla_generales']") != null)
            {
                HtmlNodeCollection table = HtmlDoc.DocumentNode.SelectNodes("//table[@class='administrador_tabla_generales']");

                HtmlNodeCollection rowsTable1 = table[1].SelectNodes(".//tr"); //Socios

                #region Designations

                if (rowsTable1 != null)
                {
                    ProveedoresOnLine.Company.Models.Util.GenericItemModel oDesignationsInfo = null;

                    Console.WriteLine("\nIncome Statement\n");

                    for (int i = 1; i < rowsTable1.Count; i++)
                    {
                        HtmlNodeCollection cols = rowsTable1[i].SelectNodes(".//td");

                        oDesignationsInfo = new ProveedoresOnLine.Company.Models.Util.GenericItemModel()
                        {
                            ItemId = 0,
                            ItemName = string.Empty,
                            ItemType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumLegalType.Designations,
                            },
                            Enable = true,
                            ItemInfo = new List<ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel>(),
                        };

                        oDesignationsInfo.ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumLegalInfoType.CD_PartnerName,
                            },
                            Value = cols[0].InnerText.ToString() != "&nbsp;" ? cols[0].InnerText.ToString() : string.Empty,
                            Enable = true
                        });

                        oDesignationsInfo.ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumLegalInfoType.CD_PartnerIdentificationNumber,
                            },
                            Value = cols[1].InnerText.ToString() != "&nbsp;" ? cols[1].InnerText.ToString() : string.Empty,
                            Enable = true,
                        });
                                                
                        //Get partner rank info
                        ProveedoresOnLine.Company.Models.Util.CatalogModel oPartnerRankInfo = Util.ProviderOptions_GetByName(219, cols[2].InnerText.ToString());

                        oDesignationsInfo.ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumLegalInfoType.CD_PartnerRank,
                            },
                            Value = oPartnerRankInfo == null ? string.Empty : oPartnerRankInfo.ItemId.ToString(),
                            Enable = true,
                        });

                        if (oDesignationsInfo != null)
                        {
                            oLegal.Add(oDesignationsInfo);
                        }
                    }

                    if (oDesignationsInfo == null)
                    {
                        Console.WriteLine("\nDesignatiosn no tiene datos disponibles.\n");
                    }
                }

                #endregion

                HtmlNodeCollection rowsTable2 = table[2].SelectNodes(".//tr"); //RUT

                #region RUT

                if (rowsTable2 != null)
                {
                    ProveedoresOnLine.Company.Models.Util.GenericItemModel oRUTInfo = null;

                    Console.WriteLine("\nRUT\n");

                    for (int i = 1; i < rowsTable2.Count; i++)
                    {
                        HtmlNodeCollection cols = rowsTable2[i].SelectNodes(".//td");

                        oRUTInfo = new ProveedoresOnLine.Company.Models.Util.GenericItemModel()
                        {
                            ItemId = 0,
                            ItemName = string.Empty,
                            ItemType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumLegalType.RUT,
                            },
                            Enable = true,
                            ItemInfo = new List<ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel>(),
                        };
                        
                        //get RUT person type
                        ProveedoresOnLine.Company.Models.Util.CatalogModel oPersonType = Util.ProviderOptions_GetByName(213, cols[0].InnerText.ToString());

                        oRUTInfo.ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumLegalInfoType.R_PersonType,
                            },
                            Value = oPersonType == null ? string.Empty : oPersonType.ItemId.ToString(),
                            Enable = true,
                        });

                        oRUTInfo.ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumLegalInfoType.R_LargeContributor,
                            },
                            Value = cols[1].InnerText.ToString() == "SI" ? "1" : "0",
                            Enable = true,
                        });

                        oRUTInfo.ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumLegalInfoType.R_LargeContributorReceipt,
                            },
                            Value = cols[2].InnerText.ToString(),
                            Enable = true,
                        });

                        oRUTInfo.ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumLegalInfoType.R_LargeContributorDate,
                            },
                            Value = cols[3].InnerText != string.Empty && cols[3].InnerText.Length > 1 ? Convert.ToDateTime(cols[3].InnerText).ToString("yyyy-MM-dd") : string.Empty,
                            Enable = true,
                        });

                        oRUTInfo.ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumLegalInfoType.R_SelfRetainer,
                            },
                            Value = cols[4].InnerText.ToString() == "SI" ? "1" : "0",
                            Enable = true,
                        });

                        oRUTInfo.ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumLegalInfoType.R_SelfRetainerReciept,
                            },
                            Value = cols[5].InnerText != "&nbsp;" ? cols[5].InnerText.ToString() : string.Empty,
                            Enable = true,
                        });

                        oRUTInfo.ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumLegalInfoType.R_SelfRetainerDate,
                            },
                            Value = cols[6].InnerText != string.Empty && cols[6].InnerText.Length > 1 ? Convert.ToDateTime(cols[6].InnerText).ToString("yyyy-MM-dd") : string.Empty,
                            Enable = true,
                        });

                        //get entity type
                        ProveedoresOnLine.Company.Models.Util.CatalogModel oEntityType = Util.ProviderOptions_GetByName(214, cols[7].InnerText.ToString());

                        oRUTInfo.ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumLegalInfoType.R_EntityType,
                            },
                            Value = oEntityType == null ? string.Empty : oEntityType.ItemId.ToString(),
                            Enable = true,
                        });

                        oRUTInfo.ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumLegalInfoType.R_IVA,
                            },
                            Value = cols[8].InnerText == "SI" ? "1" : "0",
                            Enable = true,
                        });

                        //get tax payer type
                        ProveedoresOnLine.Company.Models.Util.CatalogModel oTaxPayerType = Util.ProviderOptions_GetByName(215, cols[9].InnerText.ToString());

                        oRUTInfo.ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumLegalInfoType.R_TaxPayerType,
                            },
                            Value = oTaxPayerType == null ? string.Empty : oTaxPayerType.ItemId.ToString(),
                            Enable = true,
                        });

                        //get ICA
                        ProveedoresOnLine.Company.Models.Util.GenericItemModel oICA = Util.ICA_GetByName(cols[10].InnerText.ToString());

                        oRUTInfo.ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumLegalInfoType.R_ICA,
                            },
                            Value = oICA == null ? string.Empty : oICA.ItemName,
                            Enable = true,
                        });

                        if (cols[11].InnerHtml.Contains("href"))
                        {
                            string urlDownload = WebCrawler.Manager.General.InternalSettings.Instance[Constants.C_Settings_UrlDownload].Value;
                            string urlS3 = string.Empty;

                            if (cols[11].ChildNodes["a"].Attributes["href"].Value.Contains("../"))
                            {
                                urlDownload = cols[11].ChildNodes["a"].Attributes["href"].Value.Replace("..", urlDownload);
                            }

                            urlS3 = WebCrawler.Manager.WebCrawlerManager.UploadFile(urlDownload, enumLegalType.RUT.ToString(), PublicId);

                            oRUTInfo.ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                            {
                                ItemInfoId = 0,
                                ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                {
                                    ItemId = (int)enumLegalInfoType.R_RUTFile,
                                },
                                Value = urlS3,
                                Enable = true,
                            });
                        }

                        if (cols[12].InnerHtml.Contains("href"))
                        {
                            string urlDownload = WebCrawler.Manager.General.InternalSettings.Instance[Constants.C_Settings_UrlDownload].Value;
                            string urlS3 = string.Empty;

                            if (cols[12].ChildNodes["a"].Attributes["href"].Value.Contains("../"))
                            {
                                urlDownload = cols[12].ChildNodes["a"].Attributes["href"].Value.Replace("..", urlDownload);
                            }

                            urlS3 = WebCrawler.Manager.WebCrawlerManager.UploadFile(urlDownload, enumLegalType.RUT.ToString(), PublicId);

                            oRUTInfo.ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                            {
                                ItemInfoId = 0,
                                ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                {
                                    ItemId = (int)enumLegalInfoType.R_LargeContributorFile,
                                },
                                Value = urlS3,
                                Enable = true,
                            });
                        }

                        if (cols[13].InnerHtml.Contains("href"))
                        {
                            string urlDownload = WebCrawler.Manager.General.InternalSettings.Instance[Constants.C_Settings_UrlDownload].Value;
                            string urlS3 = string.Empty;

                            if (cols[13].ChildNodes["a"].Attributes["href"].Value.Contains("../"))
                            {
                                urlDownload = cols[13].ChildNodes["a"].Attributes["href"].Value.Replace("..", urlDownload);
                            }

                            urlS3 = WebCrawler.Manager.WebCrawlerManager.UploadFile(urlDownload, enumLegalType.RUT.ToString(), PublicId);

                            oRUTInfo.ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                            {
                                ItemInfoId = 0,
                                ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                {
                                    ItemId = (int)enumLegalInfoType.R_SelfRetainerFile,
                                },
                                Value = urlS3,
                                Enable = true,
                            });
                        }

                        if (oRUTInfo != null)
                        {
                            oLegal.Add(oRUTInfo);
                        }
                    }
                    if (oRUTInfo == null)
                    {
                        Console.WriteLine("\nRUT no tiene datos disponibles.\n");
                    }
                }

                #endregion

                HtmlNodeCollection rowsTable3 = table[8].SelectNodes(".//tr"); //SARLAFT

                #region SARLAFT

                if (rowsTable1 != null)
                {
                    ProveedoresOnLine.Company.Models.Util.GenericItemModel oSARLAFTInfo = null;

                    Console.WriteLine("\nSARLAFT\n");

                    for (int i = 1; i < rowsTable3.Count; i++)
                    {
                        HtmlNodeCollection cols = rowsTable3[i].SelectNodes(".//td");

                        oSARLAFTInfo = new ProveedoresOnLine.Company.Models.Util.GenericItemModel()
                        {
                            ItemId = 0,
                            ItemName = string.Empty,
                            ItemType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumLegalType.SARLAFT,
                            },
                            Enable = true,
                            ItemInfo = new List<ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel>(),
                        };

                        oSARLAFTInfo.ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumLegalInfoType.SF_ProcessDate,
                            },
                            Value = cols[0].InnerText != string.Empty ? Convert.ToDateTime(cols[0].InnerText).ToString("yyyy-MM-dd") : string.Empty,
                            Enable = true,
                        });

                        //get person type
                        ProveedoresOnLine.Company.Models.Util.CatalogModel oPersonType = Util.ProviderOptions_GetByName(213, cols[1].InnerText.ToString());

                        oSARLAFTInfo.ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumLegalInfoType.SF_PersonType,
                            },
                            Value = oPersonType == null ? string.Empty : oPersonType.ItemId.ToString(),
                            Enable = true,
                        });

                        if (cols[2].InnerHtml.Contains("href"))
                        {
                            string urlDownload = WebCrawler.Manager.General.InternalSettings.Instance[Constants.C_Settings_UrlDownload].Value;
                            string urlS3 = string.Empty;

                            if (cols[2].ChildNodes["a"].Attributes["href"].Value.Contains("../"))
                            {
                                urlDownload = cols[2].ChildNodes["a"].Attributes["href"].Value.Replace("..", urlDownload);
                            }

                            urlS3 = WebCrawler.Manager.WebCrawlerManager.UploadFile(urlDownload, enumLegalType.SARLAFT.ToString(), PublicId);

                            oSARLAFTInfo.ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                            {
                                ItemInfoId = 0,
                                ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                                {
                                    ItemId = (int)enumLegalInfoType.SF_SARLAFTFile,
                                },
                                Value = urlS3,
                                Enable = true,
                            });
                        }

                        if (oSARLAFTInfo != null)
                        {
                            oLegal.Add(oSARLAFTInfo);
                        }
                    }

                    if (oSARLAFTInfo == null)
                    {
                        Console.WriteLine("\nSARLAFT no tiene datos disponibles.\n");
                    }
                }

                #endregion

                HtmlNodeCollection rowsTable4 = table[4].SelectNodes(".//tr"); //CIFIN

                #region CIFIN

                if (rowsTable4 != null)
                {
                    ProveedoresOnLine.Company.Models.Util.GenericItemModel oCIFINInfo = null;

                    Console.WriteLine("\nCIFIN\n");

                    for (int i = 1; i < rowsTable4.Count; i++)
                    {
                        HtmlNodeCollection cols = rowsTable4[i].SelectNodes(".//td");

                        oCIFINInfo = new ProveedoresOnLine.Company.Models.Util.GenericItemModel()
                        {
                            ItemId = 0,
                            ItemName = string.Empty,
                            ItemType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumLegalType.CIFIN,
                            },
                            Enable = true,
                            ItemInfo = new List<ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel>(),
                        };

                        oCIFINInfo.ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumLegalInfoType.CF_QueryDate,
                            },
                            Value = cols[0].InnerText != string.Empty ? Convert.ToDateTime(cols[0].InnerText).ToString("yyyy-MM-dd") : string.Empty,
                            Enable = true,
                        });

                        oCIFINInfo.ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumLegalInfoType.CF_ResultQuery,
                            },
                            Value = cols[1].InnerText.ToString(),
                            Enable = true,
                        });

                        oCIFINInfo.ItemInfo.Add(new ProveedoresOnLine.Company.Models.Util.GenericItemInfoModel()
                        {
                            ItemInfoId = 0,
                            ItemInfoType = new ProveedoresOnLine.Company.Models.Util.CatalogModel()
                            {
                                ItemId = (int)enumLegalInfoType.CF_AutorizationFile,
                            },
                            Value = string.Empty,
                            Enable = true,
                        });

                        if (oCIFINInfo != null)
                        {
                            oLegal.Add(oCIFINInfo);
                        }
                    }

                    if (oCIFINInfo == null)
                    {
                        Console.WriteLine("\nCIFIN no tiene datos disponibles.\n");
                    }
                }

                #endregion
            }

            return oLegal;
        }
    }
}
