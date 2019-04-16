using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SQLite;

namespace myBacklog.Models
{
    public class StateModel
    {
        [PrimaryKey, AutoIncrement]
        public int StateID { get; set; }

        public string StateName { get; set; }

        [Ignore]
        public Color Color { get; set; }

        public string ColorName
        {
            get
            {
                return Color.Name;
            }
        }

        public int CategoryID { get; set; }
    }
}
