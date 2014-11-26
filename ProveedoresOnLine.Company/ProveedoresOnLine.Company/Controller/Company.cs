﻿using ProveedoresOnLine.Company.Models.Company;
using ProveedoresOnLine.Company.Models.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.Company.Controller
{
    public class Company
    {
        #region Util

        public static TreeModel TreeUpsert(TreeModel TreeToUpsert)
        {
            LogManager.Models.LogModel oLog = GetGenericLogModel();
            try
            {
                TreeToUpsert.TreeId = DAL.Controller.CompanyDataController.Instance.TreeUpsert
                    (TreeToUpsert.TreeId > 0 ? (int?)TreeToUpsert.TreeId : null,
                    TreeToUpsert.TreeName,
                    TreeToUpsert.Enable);

                oLog.IsSuccess = true;

                return TreeToUpsert;
            }
            catch (Exception err)
            {
                oLog.IsSuccess = false;
                oLog.Message = err.Message + " - " + err.StackTrace;

                throw err;
            }
            finally
            {
                oLog.LogObject = TreeToUpsert;
                LogManager.ClientLog.AddLog(oLog);
            }
        }

        public static GenericItemModel CategoryUpsert(int? TreeId, GenericItemModel CategoryToUpsert)
        {
            LogManager.Models.LogModel oLog = GetGenericLogModel();

            try
            {
                CategoryToUpsert.ItemId = DAL.Controller.CompanyDataController.Instance.CategoryUpsert
                    (CategoryToUpsert.ItemId,
                    CategoryToUpsert.ItemName,
                    CategoryToUpsert.Enable);

                CategoryInfoUpsert(CategoryToUpsert);

                if (TreeId != null && TreeId > 0)
                {
                    TreeCategoryUpsert((int)TreeId, CategoryToUpsert);
                }

                oLog.IsSuccess = true;

                return CategoryToUpsert;
            }
            catch (Exception err)
            {
                oLog.IsSuccess = false;
                oLog.Message = err.Message + " - " + err.StackTrace;

                throw err;
            }
            finally
            {
                oLog.LogObject = CategoryToUpsert;

                if (TreeId != null)
                {
                    oLog.RelatedLogInfo.Add(new LogManager.Models.LogInfoModel()
                    {
                        LogInfoType = "TreeId",
                        Value = TreeId.ToString(),
                    });
                }

                LogManager.ClientLog.AddLog(oLog);
            }
        }

        public static GenericItemModel CategoryInfoUpsert(GenericItemModel CategoryToUpsert)
        {
            if (CategoryToUpsert.ItemId > 0 &&
                CategoryToUpsert.ItemInfo != null &&
                CategoryToUpsert.ItemInfo.Count > 0)
            {
                CategoryToUpsert.ItemInfo.All(catinfo =>
                {
                    LogManager.Models.LogModel oLog = GetGenericLogModel();
                    try
                    {
                        catinfo.ItemInfoId = DAL.Controller.CompanyDataController.Instance.CategoryInfoUpsert
                            (CategoryToUpsert.ItemId,
                            catinfo.ItemInfoId > 0 ? (int?)catinfo.ItemInfoId : null,
                            catinfo.ItemInfoType.ItemId,
                            catinfo.Value,
                            catinfo.LargeValue,
                            catinfo.Enable);

                        oLog.IsSuccess = true;
                    }
                    catch (Exception err)
                    {
                        oLog.IsSuccess = false;
                        oLog.Message = err.Message + " - " + err.StackTrace;

                        throw err;
                    }
                    finally
                    {
                        oLog.LogObject = CategoryToUpsert;

                        oLog.RelatedLogInfo.Add(new LogManager.Models.LogInfoModel()
                        {
                            LogInfoType = "CategoryId",
                            Value = CategoryToUpsert.ItemId.ToString(),
                        });

                        LogManager.ClientLog.AddLog(oLog);
                    }

                    return true;
                });
            }

            return CategoryToUpsert;
        }

        public static void TreeCategoryUpsert(int TreeId, GenericItemModel CategoryToUpsert)
        {
            LogManager.Models.LogModel oLog = GetGenericLogModel();
            try
            {

                DAL.Controller.CompanyDataController.Instance.TreeCategoryUpsert
                    (TreeId,
                    CategoryToUpsert.ParentItem != null && CategoryToUpsert.ParentItem.ItemId > 0 ? (int?)CategoryToUpsert.ParentItem.ItemId : null,
                    CategoryToUpsert.ItemId,
                    CategoryToUpsert.Enable);

                oLog.IsSuccess = true;
            }
            catch (Exception err)
            {
                oLog.IsSuccess = false;
                oLog.Message = err.Message + " - " + err.StackTrace;

                throw err;
            }
            finally
            {
                oLog.LogObject = CategoryToUpsert;

                oLog.RelatedLogInfo.Add(new LogManager.Models.LogInfoModel()
                {
                    LogInfoType = "TreeId",
                    Value = TreeId.ToString(),
                });

                LogManager.ClientLog.AddLog(oLog);
            }
        }

        public static CatalogModel CatalogItemUpsert(CatalogModel CatalogItemToUpsert)
        {
            LogManager.Models.LogModel oLog = GetGenericLogModel();
            try
            {

                CatalogItemToUpsert.ItemId = DAL.Controller.CompanyDataController.Instance.CatalogItemUpsert
                    (CatalogItemToUpsert.CatalogId,
                    CatalogItemToUpsert.ItemId > 0 ? (int?)CatalogItemToUpsert.ItemId : null,
                    CatalogItemToUpsert.ItemName,
                    CatalogItemToUpsert.ItemEnable);

                oLog.IsSuccess = true;

                return CatalogItemToUpsert;
            }
            catch (Exception err)
            {
                oLog.IsSuccess = false;
                oLog.Message = err.Message + " - " + err.StackTrace;

                throw err;
            }
            finally
            {
                oLog.LogObject = CatalogItemToUpsert;
                LogManager.ClientLog.AddLog(oLog);
            }
        }

        #endregion

        #region Company

        #region Crud Methods

        public static CompanyModel CompanyUpsert(CompanyModel CompanyToUpsert)
        {
            LogManager.Models.LogModel oLog = GetGenericLogModel();
            try
            {

                CompanyToUpsert.CompanyPublicId = DAL.Controller.CompanyDataController.Instance.CompanyUpsert
                    (CompanyToUpsert.CompanyPublicId,
                    CompanyToUpsert.CompanyName,
                    CompanyToUpsert.IdentificationType.ItemId,
                    CompanyToUpsert.IdentificationNumber,
                    CompanyToUpsert.CompanyType.ItemId,
                    CompanyToUpsert.Enable);

                CompanyInfoUpsert(CompanyToUpsert);
                ContactUpsert(CompanyToUpsert);
                RoleCompanyUpsert(CompanyToUpsert);

                oLog.IsSuccess = true;

                return CompanyToUpsert;
            }
            catch (Exception err)
            {
                oLog.IsSuccess = false;
                oLog.Message = err.Message + " - " + err.StackTrace;

                throw err;
            }
            finally
            {
                oLog.LogObject = CompanyToUpsert;
                LogManager.ClientLog.AddLog(oLog);
            }
        }

        public static CompanyModel CompanyInfoUpsert(CompanyModel CompanyToUpsert)
        {
            if (!string.IsNullOrEmpty(CompanyToUpsert.CompanyPublicId) &&
                CompanyToUpsert.CompanyInfo != null &&
                CompanyToUpsert.CompanyInfo.Count > 0)
            {
                CompanyToUpsert.CompanyInfo.All(cmpinf =>
                {
                    LogManager.Models.LogModel oLog = GetGenericLogModel();
                    try
                    {
                        cmpinf.ItemInfoId = DAL.Controller.CompanyDataController.Instance.CompanyInfoUpsert
                            (CompanyToUpsert.CompanyPublicId,
                            cmpinf.ItemInfoId > 0 ? (int?)cmpinf.ItemInfoId : null,
                            cmpinf.ItemInfoType.ItemId,
                            cmpinf.Value,
                            cmpinf.LargeValue,
                            cmpinf.Enable);

                        oLog.IsSuccess = true;
                    }
                    catch (Exception err)
                    {
                        oLog.IsSuccess = false;
                        oLog.Message = err.Message + " - " + err.StackTrace;

                        throw err;
                    }
                    finally
                    {
                        oLog.LogObject = cmpinf;

                        oLog.RelatedLogInfo.Add(new LogManager.Models.LogInfoModel()
                        {
                            LogInfoType = "CompanyPublicId",
                            Value = CompanyToUpsert.CompanyPublicId,
                        });

                        LogManager.ClientLog.AddLog(oLog);
                    }

                    return true;
                });
            }

            return CompanyToUpsert;
        }

        public static CompanyModel ContactUpsert(CompanyModel CompanyToUpsert)
        {
            if (!string.IsNullOrEmpty(CompanyToUpsert.CompanyPublicId) &&
                CompanyToUpsert.RelatedContact != null &&
                CompanyToUpsert.RelatedContact.Count > 0)
            {
                CompanyToUpsert.RelatedContact.All(cmpinf =>
                {
                    LogManager.Models.LogModel oLog = GetGenericLogModel();
                    try
                    {
                        cmpinf.ItemId = DAL.Controller.CompanyDataController.Instance.ContactUpsert
                            (CompanyToUpsert.CompanyPublicId,
                            cmpinf.ItemId > 0 ? (int?)cmpinf.ItemId : null,
                            cmpinf.ItemType.ItemId,
                            cmpinf.ItemName,
                            cmpinf.Enable);

                        ContactInfoUpsert(cmpinf);

                        oLog.IsSuccess = true;

                    }
                    catch (Exception err)
                    {
                        oLog.IsSuccess = false;
                        oLog.Message = err.Message + " - " + err.StackTrace;

                        throw err;
                    }
                    finally
                    {
                        oLog.LogObject = cmpinf;

                        oLog.RelatedLogInfo.Add(new LogManager.Models.LogInfoModel()
                        {
                            LogInfoType = "CompanyPublicId",
                            Value = CompanyToUpsert.CompanyPublicId,
                        });

                        LogManager.ClientLog.AddLog(oLog);
                    }

                    return true;
                });
            }

            return CompanyToUpsert;
        }

        public static GenericItemModel ContactInfoUpsert(GenericItemModel ContactToUpsert)
        {
            if (ContactToUpsert.ItemId > 0 &&
                ContactToUpsert.ItemInfo != null &&
                ContactToUpsert.ItemInfo.Count > 0)
            {
                ContactToUpsert.ItemInfo.All(ctinf =>
                {
                    LogManager.Models.LogModel oLog = GetGenericLogModel();
                    try
                    {
                        ctinf.ItemInfoId = DAL.Controller.CompanyDataController.Instance.ContactInfoUpsert
                            (ContactToUpsert.ItemId,
                            ctinf.ItemInfoId > 0 ? (int?)ctinf.ItemInfoId : null,
                            ctinf.ItemInfoType.ItemId,
                            ctinf.Value,
                            ctinf.LargeValue,
                            ctinf.Enable);

                        oLog.IsSuccess = true;
                    }
                    catch (Exception err)
                    {
                        oLog.IsSuccess = false;
                        oLog.Message = err.Message + " - " + err.StackTrace;

                        throw err;
                    }
                    finally
                    {
                        oLog.LogObject = ctinf;

                        oLog.RelatedLogInfo.Add(new LogManager.Models.LogInfoModel()
                        {
                            LogInfoType = "ContactId",
                            Value = ContactToUpsert.ItemId.ToString(),
                        });

                        LogManager.ClientLog.AddLog(oLog);
                    }

                    return true;
                });
            }

            return ContactToUpsert;
        }

        public static CompanyModel RoleCompanyUpsert(CompanyModel CompanyToUpsert)
        {
            if (!string.IsNullOrEmpty(CompanyToUpsert.CompanyPublicId) &&
                CompanyToUpsert.RelatedRole != null &&
                CompanyToUpsert.RelatedRole.Count > 0)
            {
                CompanyToUpsert.RelatedRole.All(cmpinf =>
                {
                    LogManager.Models.LogModel oLog = GetGenericLogModel();
                    try
                    {
                        cmpinf.ItemId = DAL.Controller.CompanyDataController.Instance.RoleCompanyUpsert
                            (CompanyToUpsert.CompanyPublicId,
                            cmpinf.ItemId > 0 ? (int?)cmpinf.ItemId : null,
                            cmpinf.ItemName,
                            cmpinf.ParentItem != null && cmpinf.ParentItem.ItemId > 0 ? (int?)cmpinf.ParentItem.ItemId : null,
                            cmpinf.Enable);

                        RoleCompanyInfoUpsert(cmpinf);
                        oLog.IsSuccess = true;

                    }
                    catch (Exception err)
                    {
                        oLog.IsSuccess = false;
                        oLog.Message = err.Message + " - " + err.StackTrace;

                        throw err;
                    }
                    finally
                    {
                        oLog.LogObject = cmpinf;

                        oLog.RelatedLogInfo.Add(new LogManager.Models.LogInfoModel()
                        {
                            LogInfoType = "CompanyPublicId",
                            Value = CompanyToUpsert.CompanyPublicId,
                        });

                        LogManager.ClientLog.AddLog(oLog);
                    }

                    return true;
                });
            }
            return CompanyToUpsert;

        }

        public static GenericItemModel RoleCompanyInfoUpsert(GenericItemModel RoleCompanyToUpsert)
        {
            if (RoleCompanyToUpsert.ItemId > 0 &&
                RoleCompanyToUpsert.ItemInfo != null &&
                RoleCompanyToUpsert.ItemInfo.Count > 0)
            {
                RoleCompanyToUpsert.ItemInfo.All(ctinf =>
                {
                    LogManager.Models.LogModel oLog = GetGenericLogModel();
                    try
                    {
                        ctinf.ItemInfoId = DAL.Controller.CompanyDataController.Instance.RoleCompanyInfoUpsert
                            (RoleCompanyToUpsert.ItemId,
                            ctinf.ItemInfoId > 0 ? (int?)ctinf.ItemInfoId : null,
                            ctinf.ItemInfoType.ItemId,
                            ctinf.Value,
                            ctinf.LargeValue,
                            ctinf.Enable);

                        oLog.IsSuccess = true;

                    }
                    catch (Exception err)
                    {
                        oLog.IsSuccess = false;
                        oLog.Message = err.Message + " - " + err.StackTrace;

                        throw err;
                    }
                    finally
                    {
                        oLog.LogObject = ctinf;

                        oLog.RelatedLogInfo.Add(new LogManager.Models.LogInfoModel()
                        {
                            LogInfoType = "RoleCompanyId",
                            Value = RoleCompanyToUpsert.ItemId.ToString(),
                        });

                        LogManager.ClientLog.AddLog(oLog);
                    }

                    return true;
                });
            }

            return RoleCompanyToUpsert;
        }

        public static UserCompany UserCompanyUpsert(UserCompany UserCompanyToUpsert)
        {
            LogManager.Models.LogModel oLog = GetGenericLogModel();
            try
            {

                UserCompanyToUpsert.UserCompanyId = DAL.Controller.CompanyDataController.Instance.UserCompanyUpsert
                    (UserCompanyToUpsert.UserCompanyId > 0 ? (int?)UserCompanyToUpsert.UserCompanyId : null,
                    UserCompanyToUpsert.User,
                    UserCompanyToUpsert.RelatedRole.ItemId,
                    UserCompanyToUpsert.Enable);

                return UserCompanyToUpsert;
            }
            catch (Exception err)
            {
                oLog.IsSuccess = false;
                oLog.Message = err.Message + " - " + err.StackTrace;

                throw err;
            }
            finally
            {
                oLog.LogObject = UserCompanyToUpsert;
                LogManager.ClientLog.AddLog(oLog);
            }
        }

        #endregion

        #region Get Methods

        public static CompanyModel CompanyGetBasicInfo(string CompanyPublicId)
        {
            return DAL.Controller.CompanyDataController.Instance.CompanyGetBasicInfo(CompanyPublicId);
        }

        #endregion

        #endregion

        #region Generic Log

        public static LogManager.Models.LogModel GetGenericLogModel()
        {
            LogManager.Models.LogModel oReturn = new LogManager.Models.LogModel()
            {
                RelatedLogInfo = new List<LogManager.Models.LogInfoModel>(),
            };

            try
            {
                System.Diagnostics.StackTrace oStack = new System.Diagnostics.StackTrace();

                if (System.Web.HttpContext.Current != null)
                {
                    //get user info
                    if (SessionManager.SessionController.Auth_UserLogin != null)
                    {
                        oReturn.User = SessionManager.SessionController.Auth_UserLogin.Email;
                    }
                    else
                    {
                        oReturn.User = System.Web.HttpContext.Current.Request.UserHostAddress;
                    }

                    //get source invocation
                    oReturn.Source = System.Web.HttpContext.Current.Request.Url.ToString();
                }
                else if (oStack.FrameCount > 0)
                {
                    oReturn.Source = oStack.GetFrame(oStack.FrameCount - 1).GetMethod().Module.Assembly.Location;
                }

                //get appname
                if (oStack.FrameCount > 1)
                {
                    oReturn.Application = oStack.GetFrame(1).GetMethod().Module.Name + " - " + oStack.GetFrame(1).GetMethod().Name;
                }
                else
                {
                    oReturn.Application = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                }
            }
            catch { }

            return oReturn;
        }

        #endregion
    }
}
