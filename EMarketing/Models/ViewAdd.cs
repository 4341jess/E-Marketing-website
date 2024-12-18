using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMarketing.Models
{
    public class ViewAdd
    {

        public int pro_id { get; set; }
        public string pro_name { get; set; }
        public string pro_image { get; set; }
        public string pro_des { get; set; }
        public Nullable<int> pro_price { get; set; }
        public Nullable<int> pro { get; set; }
        public Nullable<int> prod { get; set; }
        public int cat_id { get; set; }
        public string cat_name { get; set; }
        public string u_name { get; set; }
        public string u_image { get; set; }
        public string u_contact { get; set; }
    }
}