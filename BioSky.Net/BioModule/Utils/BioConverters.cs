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
//using static BioFaceService.Person.Types;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;
using BioContracts;
using BioFaceService;

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
    public ConverterInitializer( IBioSkyNetRepository database )
    {
      PhotoIDConverter                  = new ConvertPhotoIdToImage          (database );
      PersonIdToFirstnameConverter      = new ConvertPersonIdToFirstname     (database.PersonHolder);
      PersonIdToLastnameConverter       = new ConvertPersonIdToLastname      (database.PersonHolder);
      LocationIdToLocationnameConverter = new ConvertLocationIdToLocationname(database.LocationHolder);
    }
  }
  public class ConvertLongToDateTime : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
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
