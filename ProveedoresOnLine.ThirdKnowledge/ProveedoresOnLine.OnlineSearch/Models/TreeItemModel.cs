using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.OnlineSearch.Models
{
    public class TreeItemModel
    {
        public int TreeItemId { get; set; }
        public int TreeId { get; set; }
        public CatalogModel ParentItem { get; set; }
        public CatalogModel ChildItem { get; set; }
        public bool Enable { get; set; }
        public DateTime LastModify { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
