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
        public string categoria_matricula { get; set; }
        public string clase_identificacion { get; set; }
        public int codigo_camara { get; set; }
        public int codigo_estado { get; set; }
        public string identificacion { get; set; }
        public string estado { get; set; }
        public string matricula { get; set; }
        public string nombre_camara { get; set; }
        public string organizacion_juridica { get; set; }
        public string razon_social { get; set; }
        public string tipo { get; set; }
        public string enlaceVer { get; set; }
    } 
}
