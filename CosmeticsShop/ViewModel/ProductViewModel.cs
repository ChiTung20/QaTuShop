using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.ViewModel
{
    public class ProductViewModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public Nullable<int> Price { get; set; }

        public Nullable<int> CreatedBy { get; set; }

        public Nullable<int> ViewCount { get; set; }

        public string Image1 { get; set; }

        public string Image2 { get; set; }

        public string Image3 { get; set; }

        public Nullable<int> Quantity { get; set; }

        public Nullable<int> PurchasedCount { get; set; }

        public string Description { get; set; }

        public Nullable<bool> IsActive { get; set; }

        public Nullable<int> CategoryID { get; set; }

        public string Type { get; set; }
        public List<CommentViewModel> Comments  { get; set; }
    }
}