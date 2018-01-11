using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.ThirdKnowledgeBatch.Models
{
    public class ExcelModel
    {
        public string SEARCHPARAM { get; set; }

        public string SEARCHCRITERY { get; set; }

        
        public ExcelModel()
        {

        }

        public ExcelModel(DataRow Row)
        {
            this.SEARCHPARAM = Row[ProveedoresOnLine.ThirdKnowledgeBatch.Models.InternalSettings.Instance[ProveedoresOnLine.ThirdKnowledgeBacth.Models.Constants.MP_CP_ColSearchParam].Value].ToString();
            this.SEARCHCRITERY = Row[ProveedoresOnLine.ThirdKnowledgeBatch.Models.InternalSettings.Instance[ProveedoresOnLine.ThirdKnowledgeBacth.Models.Constants.MP_CP_ColSearchCritery].Value].ToString();
        }
    }
}
