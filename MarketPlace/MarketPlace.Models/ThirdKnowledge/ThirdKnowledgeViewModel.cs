using MarketPlace.Models.General;
using MarketPlace.Models.ThirdKnowledge;
using ProveedoresOnLine.IndexSearch.Models;
using ProveedoresOnLine.ThirdKnowledge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPlace.Models.ThirdKnowledge
{
    public class ThirdKnowledgeViewModel
    {
        public bool RenderScripts { get; set; }

        public List<ProveedoresOnLine.ThirdKnowledge.Models.PlanModel> RelatedPlanModel { get; set; }

        public ProveedoresOnLine.ThirdKnowledge.Models.PlanModel CurrentPlanModel { get; set; }

        public ProveedoresOnLine.ThirdKnowledge.Models.PeriodModel CurrentPeriodModel { get; set; }

        public List<MarketPlace.Models.General.GenericMenu> ThirdKnowledgeMenu { get; set; }

        public ThirdKnowledgeSearchViewModel RelatedThidKnowledgePager { get; set; }

        public bool ShowAlerForQueries { get; set; }

        public int CurrentSale { get; set; }

        public bool HasPlan { get; set; }

        //Params to Filter
        public List<string> oUsers { get; set; }
        public List<string> Users
        {
            get
            {
                if (oUsers == null)
                {
                    oUsers = ProveedoresOnLine.ThirdKnowledge.Controller.ThirdKnowledgeModule.GetUsersBycompanyPublicId(SessionModel.CurrentCompany.CompanyPublicId);
                    if (SessionModel.CurrentCompanyLoginUser.RelatedCompany.FirstOrDefault().RelatedUser.FirstOrDefault().RelatedCompanyRole.ParentRoleCompany != null)
                    {
                        oUsers = new List<string>();
                        oUsers.Add(SessionModel.CurrentCompanyLoginUser.RelatedUser.Email);
                        
                    }
                   
                }
                return oUsers;
            }
        }
        
        //Params to Research
        public string SearchNameParam { get; set; }

        public string SearchIdNumberParam { get; set; }
        public string IdType { get; set; }

        public bool ReSearch { get; set; }

        //Temp Cols
        public TDQueryModel CollumnsResult { get; set; }

        public string ReturnUrl { get; set; }

        public string QueryBasicPublicId { get; set; }

        //Cosl
        public string RequestName { get; set; }
        public string IdNumberRequest { get; set; }
        public string QueryPublicId { get; set; }
        public string IdGroup { get; set; }
        public string GroupName { get; set; }
        public string Priority { get; set; }
        public string TypeDocument { get; set; }
        public string IdentificationNumberResult { get; set; }
        public string NameResult { get; set; }
        public string IdList { get; set; }
        public string ListName { get; set; }
        public string Alias { get; set; }
        public string Offense { get; set; }
        public string Peps { get; set; }
        public string Status { get; set; }
        public string Zone { get; set; }
        public string Link { get; set; }
        public string MoreInfo { get; set; }
        public string RegisterDate { get; set; }
        public string LastModifyDate { get; set; }
        public string Message { get; set; } 
        public string FileURL { get; set; }

        public int ElasticId { get; set; }

        #region Third Knowledge Search

        public List<ProveedoresOnLine.ThirdKnowledge.Models.TDQueryModel> ThirdKnowledgeResult { get; set; }

        public DateTime InitDate { get; set; }

        public DateTime EndDate { get; set; }      
        #endregion

        public ThirdKnowledgeViewModel(ProveedoresOnLine.ThirdKnowledge.Models.PlanModel oCurrenPlan)
        {
            CurrentSale = oCurrenPlan.RelatedPeriodModel.FirstOrDefault().TotalQueries;
        }

        public ThirdKnowledgeViewModel(TDQueryInfoModel oDetail)
        {
            QueryBasicPublicId = oDetail.QueryInfoPublicId;
            RequestName = oDetail.QueryName;
            IdNumberRequest = oDetail.QueryIdentification;
            QueryPublicId = oDetail.QueryPublicId;            
            GroupName = oDetail.GroupName;
            Priority = oDetail.Priority;
            TypeDocument = oDetail.DocumentType;
            IdentificationNumberResult = oDetail.IdentificationResult;
            NameResult = oDetail.NameResult;
            IdList = oDetail.IdList;
            ListName = oDetail.ListName;
            Alias = oDetail.AKA;
            Offense = oDetail.Offense;
            Peps = oDetail.Peps;
            Status = oDetail.Status;
            Zone = oDetail.Zone;
            Link = oDetail.Link;
            MoreInfo = oDetail.MoreInfo;
            RegisterDate = oDetail.CreateDate.ToString();
            LastModifyDate = oDetail.CreateDate.ToString();
            Message = oDetail.Message;
            FileURL = oDetail.UrlFile;
            Status = oDetail.Status;
            ElasticId = oDetail.ElasticId;
        }

        public ThirdKnowledgeViewModel()
        {

        }

        #region Indexation properties

        public Nest.ISearchResponse<TK_QueryIndexModel> ElasticQueryModel { get; set; }

        public List<ProveedoresOnLine.Company.Models.Util.ElasticSearchFilter> QueryStatusFilter { get; set; }
        public List<ProveedoresOnLine.Company.Models.Util.ElasticSearchFilter> InitDateFilter { get; set; }
        public List<ProveedoresOnLine.Company.Models.Util.ElasticSearchFilter> EndDateFilter { get; set; }
        public List<ProveedoresOnLine.Company.Models.Util.ElasticSearchFilter> DomainFilter { get; set; }
        public List<ProveedoresOnLine.Company.Models.Util.ElasticSearchFilter> UserFilter { get; set; }
        public List<ProveedoresOnLine.Company.Models.Util.ElasticSearchFilter> QueryTypeFilter { get; set; }
        #endregion

        public string SearchFilter { get; set; }

        public List<Tuple<string, string, string>> GetlstSearchFilter()
        {
            List<Tuple<string, string, string>> oReturn = new List<Tuple<string, string, string>>();

            if (!string.IsNullOrEmpty(SearchFilter))
            {
                oReturn = SearchFilter.Replace(" ", "").
                    Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).
                    Where(x => x.IndexOf(';') >= 0).
                    Select(x => new Tuple<string, string, string>(x.Split(';')[0], x.Split(';')[1], x.Split(';')[2])).
                    Where(x => !string.IsNullOrEmpty(x.Item1) && !string.IsNullOrEmpty(x.Item2) && !string.IsNullOrEmpty(x.Item3)).
                    ToList();
            }

            return oReturn;
        }

    }
}
