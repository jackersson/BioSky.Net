﻿using BioData;
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
//using static BioFaceService.Person.Types;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;
using BioContracts;
using BioFaceService;

namespace BioModule.Utils
{
  public class ConvertPhotoIdToImage : IValueConverter
  {
    public ConvertPhotoIdToImage( IBioSkyNetRepository database )
    {
      _database = database;
    }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        Photo photo = _database.GetPhotoByID((long)value);

        string addr = Directory.GetCurrentDirectory();

        if (photo != null && File.Exists(photo.FileLocation))
        {
          BitmapSource img = new BitmapImage(new Uri(addr + "\\" + photo.FileLocation, UriKind.RelativeOrAbsolute));
          return img;
        }

      }
      return ResourceLoader.UserDefaultImageIconSource;
    }
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    private readonly IBioSkyNetRepository _database;
  }

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

  public class StringToGenderConverter : StringToEnumConverter<BioFaceService.Person.Types.Gender>
  { }

  public class StringToRightsConverter : StringToEnumConverter<BioFaceService.Person.Types.Rights>
  { }

  public class StringToEnumConverter<TEnum> : IValueConverter
  {
    public object Convert( object value, Type targetType,
                           object parameter, CultureInfo culture)
    {
      return Enum.Parse(typeof(TEnum), value.ToString());
    }    
    public object ConvertBack(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
      return Enum.Parse(targetType, value.ToString());
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

  public class ConvertToFormatedNumber : IValueConverter
  {
    public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      int chunkSize = 4;
      string number = (string)(values);
      string result = "";
      int stringLength = number.Length;
      for (int i = 0; i < stringLength; i += chunkSize)
      {
        if (i + chunkSize > stringLength) chunkSize = stringLength - i;

        result += number.Substring(i, chunkSize) + " ";
      }

      return result;
    }

    public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
  public class NullImageConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        return DependencyProperty.UnsetValue;
      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
