using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.RestrictiveListProcessBatch.Models
{
    public class ExcelModel
    {
        public string TIPOPERSONA { get; set; }

        public string NUMEIDEN { get; set; }

        public string NOMBRES { get; set; }

        public ExcelModel()
        {

        }

        public ExcelModel(DataRow Row)
        {            
            this.NUMEIDEN = Row["NUMERO DE IDENTIFICACION"].ToString();
            this.NOMBRES = Row["NOMBRES Y APELLIDOS"].ToString();
        }

    }
}
