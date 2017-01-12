using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Models.Provider
{
    public class CalificationProjectConfigInfoViewModel
    {
        public ProveedoresOnLine.CalificationProject.Models.CalificationProject.ConfigInfoModel RelatedCalificationProjectConfigInfoModel { get; set; }
        public string CalificationProjectConfigInfoId { get; set; }    
        public string CalificationProjectConfigId { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CalificationProjectConfigName { get; set; }
        public ProveedoresOnLine.CalificationProject.Models.CalificationProject.CalificationProjectConfigModel CalificationProjectConfig { get; set; }
        public bool Status { get; set; }
        public bool Enable { get; set; }
        public CalificationProjectConfigInfoViewModel() { }
        public CalificationProjectConfigInfoViewModel(ProveedoresOnLine.CalificationProject.Models.CalificationProject.ConfigInfoModel oRelatedCalificationProjectConfigInfoModel)
        {
            CalificationProjectConfigInfoId = oRelatedCalificationProjectConfigInfoModel.CalificationProjectConfigInfoId.ToString();

            CalificationProjectConfigId = oRelatedCalificationProjectConfigInfoModel.RelatedCalificationProjectConfig.CalificationProjectConfigId.ToString();

            CompanyId = oRelatedCalificationProjectConfigInfoModel.CompanyId.ToString();

            CompanyName = oRelatedCalificationProjectConfigInfoModel.RelatedCustomer.CompanyName;

            CalificationProjectConfigName = oRelatedCalificationProjectConfigInfoModel.RelatedCalificationProjectConfig.CalificationProjectConfigName;

            RelatedCalificationProjectConfigInfoModel = oRelatedCalificationProjectConfigInfoModel;

            CalificationProjectConfig = oRelatedCalificationProjectConfigInfoModel.RelatedCalificationProjectConfig;
            
            Enable = oRelatedCalificationProjectConfigInfoModel.Enable;

            Status = oRelatedCalificationProjectConfigInfoModel.Status;
        }

    }
}
