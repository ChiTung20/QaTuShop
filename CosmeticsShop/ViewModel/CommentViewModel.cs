using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.ViewModel
{
    public class CommentViewModel
    {
        public int ID { get; set; }
        public string CommentMsg { get; set; }
        public Nullable<System.DateTime> CommentDate { get; set; }
        public int ProductID { get; set; }
        public int UserID { get; set; }
        public string Name { get; set; }
        public int ParentID { get; set; }
        public int Rate { get; set; }
    }
}
