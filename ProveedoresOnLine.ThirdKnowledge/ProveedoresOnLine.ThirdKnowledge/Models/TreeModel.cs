﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.ThirdKnowledge.Models
{
    public class TreeModel
    {
        public int TreeId { get; set; }
        public string TreeName { get; set; }
        public TDCatalogModel TreeType { get; set; }
        public bool Enable { get; set; }
        public DateTime LastModify { get; set; }
        public DateTime CreateDate { get; set; }
        public List<TreeItemModel> TreeItem  { get; set; }
    }
}
