using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.ThirdKnowledge.Models
{
    public class TreeItemModel
    {
        public int TreeItemId { get; set; }
        public int TreeId { get; set; }
        public TDCatalogModel ParentItem { get; set; }
        public TDCatalogModel ChildItem { get; set; }
        public bool Enable { get; set; }
        public DateTime LastModify { get; set; }
        public DateTime CreateDate { get; set; }  
    }
}
