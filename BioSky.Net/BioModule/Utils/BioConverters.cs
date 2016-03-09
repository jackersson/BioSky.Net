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
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;
using BioContracts;
using BioService;

namespace BioModule.Utils
{

  public class ConverterInitializer
  {

    #region UI 
    private static ConvertPhotoIdToImage _photoIDConverter;
    public static ConvertPhotoIdToImage PhotoIDConverter
    {
      get { return _photoIDConverter; }
      set
      {
        if (_photoIDConverter != value)
        {
          _photoIDConverter = value;
        }
      }
    }

    private static ConvertPersonIdToFirstname _personIdToFirstnameConverter;
    public static ConvertPersonIdToFirstname PersonIdToFirstnameConverter
    {
      get { return _personIdToFirstnameConverter; }
      set
      {
        if (_personIdToFirstnameConverter != value)
        {
          _personIdToFirstnameConverter = value;
        }
      }
    }

    private static ConvertPersonIdToLastname _personIdToLastnameConverter;
    public static ConvertPersonIdToLastname PersonIdToLastnameConverter
    {
      get { return _personIdToLastnameConverter; }
      set
      {
        if (_personIdToLastnameConverter != value)
        {
          _personIdToLastnameConverter = value;
        }
      }
    }

    private static ConvertLocationIdToLocationname _locationIdToLocationnameConverter;
    public static ConvertLocationIdToLocationname LocationIdToLocationnameConverter
    {
      get { return _locationIdToLocationnameConverter; }
      set
      {
        if (_locationIdToLocationnameConverter != value)
        {
          _locationIdToLocationnameConverter = value;
        }
      }
    }

    private static ConvertFileLocationToImage _fileLocationToImageConverter;
    public static ConvertFileLocationToImage FileLocationToImageConverter
    {
      get { return _fileLocationToImageConverter; }
      set
      {
        if (_fileLocationToImageConverter != value)
        {
          _fileLocationToImageConverter = value;
        }
      }
    }
    #endregion

    
    public ConverterInitializer( IBioSkyNetRepository database )
    {
      PhotoIDConverter                  = new ConvertPhotoIdToImage          (database );
      FileLocationToImageConverter      = new ConvertFileLocationToImage     (database);
      PersonIdToFirstnameConverter      = new ConvertPersonIdToFirstname     (database.Persons);
      PersonIdToLastnameConverter       = new ConvertPersonIdToLastname      (database.Persons);
      LocationIdToLocationnameConverter = new ConvertLocationIdToLocationname(database.Locations);
    }
  }

  #region InverseBooleanToVisibilitConvertor  
  public class InverseBooleanConverter : IValueConverter
  {
    private bool _invert = true;
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    { 
       var flag = (value is bool && (bool)value); 
       return (flag ^ _invert) ? Visibility.Visible : Visibility.Collapsed; 
    }


    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
   { 
     var result = value is Visibility && (Visibility)value == Visibility.Visible; 
     return result ^ _invert; 
   } 

  }
  #endregion


  #region ConvertLongToDateTime
  public class ConvertLongToDateTime : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        long newvalue = (long)value;
        if (newvalue < 1000)
          return DateTime.Now.Ticks.ToString("hh:mm:ss dd.MM.yy");
        return new DateTime((long)value).ToString("hh:mm:ss dd.MM.yy");
      }
      return null;
    }
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        DateTime time = (DateTime)value;
        return time.Ticks;
      }
      return null;
    }
  }

  #endregion

  #region ConvertPhotoIdToImage
  public class ConvertPhotoIdToImage : IValueConverter
  {
    public ConvertPhotoIdToImage(IBioSkyNetRepository database)
    {
      _database = database;
      //_photoHolder = _database.PhotoHolder;
      _personHolder = _database.Persons;
    }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        Person person = _personHolder.GetValue((long)value);
        
        if(person != null)
        {

          Photo photo = person.Photos.FirstOrDefault();//.GetValue(person.Thumbnailid);

          if (photo != null)
          {
            string fullFilePathway = _database.LocalStorage.LocalStoragePath + photo.PhotoUrl;
            if (File.Exists(fullFilePathway))
            {
              BitmapSource img = new BitmapImage(new Uri(fullFilePathway, UriKind.RelativeOrAbsolute));
              return img;
            }
          }
          
        }
      }
      return ResourceLoader.UserDefaultImageIconSource;
    }
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
    private readonly IBioSkyNetRepository  _database    ;
    private readonly IHolder<Photo, long>  _photoHolder ;
    private readonly IFullPersonHolder _personHolder;
  }
  #endregion

  #region ConvertFileLocationToImage
  public class ConvertFileLocationToImage : IValueConverter
  {
    public ConvertFileLocationToImage(IBioSkyNetRepository database)
    {
      _database = database;
    }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        string fullFilePathway = _database.LocalStorage.LocalStoragePath + "\\" + value;

        if (File.Exists(fullFilePathway))
        {
          BitmapSource img = new BitmapImage(new Uri(fullFilePathway, UriKind.RelativeOrAbsolute));
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
  #endregion

  #region ConvertLocationIdToLocationname
  public class ConvertLocationIdToLocationname : IValueConverter
  {
    public ConvertLocationIdToLocationname(IFullHolder<Location> locationHolder)
    {
      _locationHolder = locationHolder;
    }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        Location location = _locationHolder.GetValue((long)value);
        return (location != null) ? location.LocationName : "";
      }
      return null;
    }
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    private readonly IFullHolder<Location> _locationHolder;
  }

  #endregion

  #region ConvertPersonIdToFirstname
  public class ConvertPersonIdToFirstname : IValueConverter
  {
    public ConvertPersonIdToFirstname(IFullPersonHolder personHolder)
    {
      _personHolder = personHolder;
    }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        Person person = _personHolder.GetValue((long)value);
        if (person != null)
          return person.Firstname;
      }
      return "Undefined";
    }
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    private readonly IFullPersonHolder _personHolder;
  }
  #endregion

  #region ConvertPersonIdToLastname
  public class ConvertPersonIdToLastname : IValueConverter
  {
    public ConvertPersonIdToLastname(IFullPersonHolder personHolder)
    {
      _personHolder = personHolder;
    }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        Person person = _personHolder.GetValue((long)value);
        if (person != null)
          return person.Lastname;
      }
      return null;
    }
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    private readonly IFullPersonHolder _personHolder;
  }
  #endregion

  #region ConvertPhotoPathToImage
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

  #endregion

  #region ConvertStateToImage
  public class ConvertStateToImage : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        if ((bool)value)        
          return ResourceLoader.OkIconSource;
      }
      return ResourceLoader.CancelIconSource;
    }
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  #endregion

  #region StringToEnumConverter

  public class StringToGenderConverter : StringToEnumConverter<BioService.Person.Types.Gender>
  { }

  public class StringToRightsConverter : StringToEnumConverter<BioService.Person.Types.Rights>
  { }

  public class StringToStateConverter : StringToEnumConverter<BioModule.ViewModels.PermissionState>
  { }

  public class StringToEnumConverter<TEnum> : IValueConverter
  {
    public object Convert( object value, Type targetType,
                           object parameter, CultureInfo culture)
    {
      //value.ToString();//
      return value.ToString(); //Enum.Parse(typeof(TEnum), value.ToString());
    }    
    public object ConvertBack(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
      if(value != null)
        return (TEnum)Enum.Parse(targetType, value.ToString());

      return null;
    }
    
  }

  #endregion
   

  #region ActivatedDevicesConverter
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
  #endregion

  #region ConvertToFormatedNumber
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

  #endregion

  #region NullImageConverter
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
  #endregion



#region Multi Converters


  #region MultiThumbnailConverter
  public class MultiThumbnailConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
      if (values[0] == null || values[1] == null)
        return System.Windows.Visibility.Collapsed;

      if (values[0].ToString() == values[1].ToString())
        return System.Windows.Visibility.Visible;
      else
        return System.Windows.Visibility.Collapsed;

    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
        System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
  #endregion

#endregion
}
