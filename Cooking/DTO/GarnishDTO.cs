using Cooking.ServiceLayer;
using PropertyChanged;
using System.ComponentModel;

namespace Cooking.DTO
{
    [AddINotifyPropertyChangedInterface]
    public class GarnishMain : GarnishDTO, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}