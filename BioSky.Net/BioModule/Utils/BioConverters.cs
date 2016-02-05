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
//using static BioService.Person.Types;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;
using BioContracts;
using BioService;

namespace BioModule.Utils
{

  public class ConverterInitializer
  {

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
    public ConverterInitializer( IBioSkyNetRepository database )
    {
      PhotoIDConverter                  = new ConvertPhotoIdToImage          (database );
      FileLocationToImageConverter      = new ConvertFileLocationToImage     (database);
      PersonIdToFirstnameConverter      = new ConvertPersonIdToFirstname     (database.PersonHolder);
      PersonIdToLastnameConverter       = new ConvertPersonIdToLastname      (database.PersonHolder);
      LocationIdToLocationnameConverter = new ConvertLocationIdToLocationname(database.LocationHolder);
    }
  }


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
      _photoHolder = _database.PhotoHolder;
      _personHolder = _database.PersonHolder;
    }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        Person person = null;
        bool personFound = _personHolder.DataSet.TryGetValue((long)value, out person);
        
        if(person != null)
        {
          Photo photo = null;
          bool photoFound = _photoHolder.DataSet.TryGetValue(person.Thumbnail, out photo);

          if (photoFound)
          {
            string fullFilePathway = _database.LocalStorage.LocalStoragePath + "\\" + photo.FileLocation;
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
    private readonly IHolder<Person, long> _personHolder;
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
    public ConvertLocationIdToLocationname(IHolder<Location, long> locationHolder)
    {
      _locationHolder = locationHolder;
    }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        Location location = null;
        bool locationFound = _locationHolder.DataSet.TryGetValue((long)value, out location);      
        return (locationFound) ? location.LocationName : "";
      }
      return null;
    }
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    private readonly IHolder<Location, long> _locationHolder;
  }

  #endregion

  #region ConvertPersonIdToFirstname
  public class ConvertPersonIdToFirstname : IValueConverter
  {
    public ConvertPersonIdToFirstname(IHolder<Person, long> personHolder)
    {
      _personHolder = personHolder;
    }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        Person person = null;
        bool personFound = _personHolder.DataSet.TryGetValue((long)value, out person);
        if (personFound)
          return person.Firstname;
      }
      return null;
    }
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    private readonly IHolder<Person, long> _personHolder;
  }
  #endregion

  #region ConvertPersonIdToLastname
  public class ConvertPersonIdToLastname : IValueConverter
  {
    public ConvertPersonIdToLastname(IHolder<Person, long> personHolder)
    {
      _personHolder = personHolder;
    }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        Person person = null;
        bool personFound = _personHolder.DataSet.TryGetValue((long)value, out person);
        if (personFound)
          return person.Lastname;
      }
      return null;
    }
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    private readonly IHolder<Person, long> _personHolder;
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

  #region StringToEnumConverter

  public class StringToGenderConverter : StringToEnumConverter<BioService.Person.Types.Gender>
  { }

  public class StringToRightsConverter : StringToEnumConverter<BioService.Person.Types.Rights>
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

  #region ConvertStatusToImage
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
}
