using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;
using Caliburn.Micro;

namespace BioModule.ViewModels
{
    public class SettingsViewModel : PropertyChangedBase
    {

        public BitmapSource SaveIconSource
        {
            get { return ResourceLoader.SaveIconSource; }
        } 


    }
}
