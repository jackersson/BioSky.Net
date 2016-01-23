using BioData;
using BioModule.ResourcesLoader;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

namespace BioModule.Utils
{
    public class ConvertPhotoPathToImage : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {

        if (File.Exists(value.ToString()))
        {
          BitmapSource img = new BitmapImage(new Uri(value.ToString(), UriKind.RelativeOrAbsolute));
          return img;
        }

      }
      return ResourceLoader.UserDefaultImageIconSource;
    }
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class StringToGenderConverter : IValueConverter
  {
    public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
      Gender gender;
      Enum.TryParse(value.ToString(), out gender);
      return gender;
    }

    public object ConvertBack(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
      return Enum.GetName(value.GetType(), value);
    }
  }

  public class StringToRightsConverter : IValueConverter
  {
    public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
      Rights right;
      Enum.TryParse(value.ToString(), out right);
      return right;
    }

    public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
    {
      return Enum.GetName(value.GetType(), value); ;
    }
  }


  public class ConvertStatusToImage : IValueConverter
  {
    private BioStatusResource _resource = new BioStatusResource();
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        return _resource.GetBitmapSource(value.ToString());
      }
      return null;
    }
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class ActivatedDevicesConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      string name = (string)value;
      ObservableCollection<string> ActivatedDevices = (parameter as ObjectDataProvider).ObjectInstance as ObservableCollection<string>;
      return !ActivatedDevices.Contains(name);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
