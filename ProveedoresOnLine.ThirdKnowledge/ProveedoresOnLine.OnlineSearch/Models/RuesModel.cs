using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.OnlineSearch.Models
{
    public class RuesModel
    {
        public List<RuesModel> rows { get; set; }
        public string codigo_error { get; set; }
        public string tipoID { get; set; }
        public string nit { get; set; }
        public string razSol { get; set; }
        public string desc_camara { get; set; }

        public string desc_categoria_matricula { get; set; }
        public string detalleRM { get; set; }

        public string detalleESAL { get; set; }

        public string detalleRUP { get; set; }

        public string detalleRNT { get; set; }

    } 
}
