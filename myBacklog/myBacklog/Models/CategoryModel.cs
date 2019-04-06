using System;
using System.Collections.Generic;
using System.Text;

namespace myBacklog.Models
{
    public class CategoryModel
    {
        public int ID { get; set; }

        public string CategoryName { get; set; }
        
        public List<StateModel> States { get; set; }
    }
}
