using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.ThirdKnowledge.Models
{
    public class TDQueryInfoModel
    {
        public int QueryInfoId { get; set; }

        public string QueryInfoPublicId { get; set; }

        public string QueryPublicId { get; set; }

        public string NameResult { get; set; }

        public string IdentificationResult { get; set; }

        public string Priority { get; set; }

        public string Peps { get; set; }

        public string Status { get; set; }

        public string Offense { get; set; }

        public string DocumentType { get; set; }

        public string IdentificationNumber { get; set; }

        public string FullName { get; set; }

        public string IdList { get; set; }

        public string ListName { get; set; }

        public string AKA { get; set; }

        public string ChargeOffense { get; set; }

        public string Message { get; set; }

        public string QueryIdentification { get; set; }

        public string QueryName { get; set; }

        public int ElasticId { get; set; }

        public string GroupName{ get; set; }

        public string Link { get; set; }

        public string MoreInfo { get; set; }

        public string Zone { get; set; }

        public string UrlFile { get; set; }

        public string GroupId { get; set; }
                
        public bool Enable { get; set; }

        public DateTime LastModify { get; set; }

        public DateTime CreateDate { get; set; }
        public TDQueryInfoModel() { }

        public TDQueryInfoModel(TDQueryInfoModel oTDQueryInfoModel)
        {
            QueryInfoId = oTDQueryInfoModel.QueryInfoId;
            QueryInfoPublicId = oTDQueryInfoModel.QueryInfoPublicId;
            QueryPublicId = oTDQueryInfoModel.QueryPublicId;
            NameResult = oTDQueryInfoModel.NameResult;
            IdentificationResult = oTDQueryInfoModel.IdentificationResult;
            Priority = oTDQueryInfoModel.Priority;
            Peps = oTDQueryInfoModel.Peps;
            Status = oTDQueryInfoModel.Status;
            Offense = oTDQueryInfoModel.Offense;
            DocumentType = Offense = oTDQueryInfoModel.DocumentType;
            IdentificationNumber = Offense = oTDQueryInfoModel.IdentificationNumber;
            FullName = Offense = oTDQueryInfoModel.FullName;
            AKA = oTDQueryInfoModel.AKA;
            ChargeOffense = Offense = oTDQueryInfoModel.ChargeOffense;
            Message = Offense = oTDQueryInfoModel.Message;
            QueryIdentification = Offense = oTDQueryInfoModel.QueryIdentification;
            QueryName = oTDQueryInfoModel.QueryName;
            ElasticId = oTDQueryInfoModel.ElasticId;
            GroupName = oTDQueryInfoModel.GroupName;
            Link = oTDQueryInfoModel.Link;
            MoreInfo = oTDQueryInfoModel.MoreInfo;
            Zone = oTDQueryInfoModel.Zone;
            UrlFile = oTDQueryInfoModel.UrlFile;
            GroupId = oTDQueryInfoModel.GroupId;
            Enable = oTDQueryInfoModel.Enable;            
            LastModify = oTDQueryInfoModel.LastModify;
            CreateDate = oTDQueryInfoModel.CreateDate;
        }
    }
}
