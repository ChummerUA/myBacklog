using myBacklog.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace myBacklog.ViewModels
{
    public class SetColorViewModel 
    {
        public ObservableCollection<NamedColor> Colors { get; set; }

        public NamedColor SelectedColor { get; set; }

        public SetColorViewModel(ObservableCollection<NamedColor> usedColors, NamedColor selectedColor)
        {
            SelectedColor = selectedColor;
            Colors = new ObservableCollection<NamedColor>();
            var colors = NamedColor.All;

            foreach(var color in colors)
            {
                Colors.Add(color);
            }

            
            foreach(var color in usedColors)
            {
                Colors.Remove(color);
            }
            if (!Colors.Contains(SelectedColor))
            {
                Colors.Add(SelectedColor);
            }
        }
    }
}
