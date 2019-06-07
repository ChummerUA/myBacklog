using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Prism.Mvvm;
using Plugin.CloudFirestore.Attributes;

namespace myBacklog.Models
{
    public class StateModel : BindableBase
    {
        #region Variables
        private string stateID;
        private string stateName;
        private NamedColor namedColor;
        private string categoryID;
        private int id;
        #endregion

        #region ID
        public string StateID
        {
            get => stateID;
            set => SetProperty(ref stateID, value);
        }

        public string CategoryID
        {
            get => categoryID;
            set => SetProperty(ref categoryID, value);
        }

        public int ID
        {
            get => id;
            set => SetProperty(ref id, value);
        }
        #endregion

        public string StateName
        {
            get => stateName;
            set => SetProperty(ref stateName, value);
        }

        public string ColorName
        {
            get
            {
                return NamedColor.Name;
            }
            set
            {
                NamedColor = NamedColor.All.FirstOrDefault(x => x.Name == value);
            }
        }

        #region Ignored
        [Ignored]
        public NamedColor NamedColor
        {
            get => namedColor;
            set
            {
                if(value != namedColor)
                {
                    namedColor = value;
                    OnPropertyChanged("NamedColor");
                    OnPropertyChanged("Color");
                }
            }
        }

        [Ignored]
        public Color Color
        {
            get => NamedColor.Color;
        }
        #endregion
    }
}
