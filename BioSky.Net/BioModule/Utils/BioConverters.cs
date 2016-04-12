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
using static BioService.Location.Types;
using BioContracts.Holders;

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

    private static ConvertPermissionToVisibility _permissionToVisibilityConverter;
    public static ConvertPermissionToVisibility PermissionToVisibilityConverter
    {
      get { return _permissionToVisibilityConverter; }
      set
      {
        if (_permissionToVisibilityConverter != value)
        {
          _permissionToVisibilityConverter = value;
        }
      }
    }

    private static MultiPermissionConverter _multiPermissionToVisibilityConverter;
    public static MultiPermissionConverter MultiPermissionToVisibilityConverter
    {
      get { return _multiPermissionToVisibilityConverter; }
      set
      {
        if (_multiPermissionToVisibilityConverter != value)
        {
          _multiPermissionToVisibilityConverter = value;
        }
      }
    }
    #endregion


    public ConverterInitializer(IProcessorLocator locator)
    {
      _locator   = locator;
      _database  = _locator.GetProcessor<IBioSkyNetRepository>();
      _bioEngine = _locator.GetProcessor<IBioEngine>();
      

      PhotoIDConverter                     = new ConvertPhotoIdToImage          (_database)          ;
      FileLocationToImageConverter         = new ConvertFileLocationToImage     (_database)          ;
      PersonIdToFirstnameConverter         = new ConvertPersonIdToFirstname     (_database.Persons)  ;
      PersonIdToLastnameConverter          = new ConvertPersonIdToLastname      (_database.Persons)  ;
      LocationIdToLocationnameConverter    = new ConvertLocationIdToLocationname(_database.Locations);
      PermissionToVisibilityConverter      = new ConvertPermissionToVisibility  (_bioEngine)         ;
      MultiPermissionToVisibilityConverter = new MultiPermissionConverter       (_bioEngine)         ;
    }

    private IProcessorLocator    _locator  ;
    private IBioSkyNetRepository _database ;
    private IBioEngine           _bioEngine;
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

  #region ConvertPermissionToVisibility
  public class ConvertPermissionToVisibility : IValueConverter
  {
    public ConvertPermissionToVisibility(IBioEngine bioEngine)
    {
      _bioEngine = bioEngine;
    }
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return true;
      if (value != null)
      {
        Activity activity = (Activity)parameter;
        bool flag = _bioEngine.IsActivityAllowed(activity);

        return flag;

      }
      return null;
    }
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    private IBioEngine _bioEngine;

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

  public class ConvertLongToFullDateTime : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        long newvalue = (long)value;
        return new DateTime((long)value);
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
        
        if(person != null && person.Photos != null)
        {

          Photo photo = person.Photos.Where(x => x.Id == person.Photoid).FirstOrDefault(); //.GetValue(person.Thumbnailid);

          if (photo != null)
          {
            string fullFilePathway = _database.LocalStorage.GetParametr(ConfigurationParametrs.MediaPathway) + photo.PhotoUrl;
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
        string fullFilePathway = _database.LocalStorage.GetParametr(ConfigurationParametrs.MediaPathway) + "\\" + value;

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

  #region ConvertMessageTypeToImage
  public class ConvertMessageTypeToImage : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        MessageType messageType = (MessageType)value;
        switch(messageType)
        {
          case MessageType.Error:
            return ResourceLoader.ErrorIconSource;

          case MessageType.Information:
            return ResourceLoader.InformationCircleIconSource;            

          case MessageType.Warning:
            return ResourceLoader.WarningIconSource;
        }          
      }
      return ResourceLoader.ErrorIconSource;
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

  public class StringToStateConverter : StringToEnumConverter<AccessType>
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

  #region BooleanToCollapsedVisibilityConverter
  public class BooleanToCollapsedVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value != null)
      {
        bool flag = (bool)value;
        return (flag)? Visibility.Visible: Visibility.Collapsed;
      }
      return Visibility.Collapsed;
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

  #region MultiPermissionConverter
  public class MultiPermissionConverter : IMultiValueConverter
  {
    public MultiPermissionConverter(IBioEngine bioEngine)
    {
      _bioEngine = bioEngine;
    }
    public object Convert(object[] values, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
      //values[0] = CurrentPermissionRights
      //values[1] = CurrentIsEnabledState
      //values[2] = Parametr(Activity)

      if (values[0] != null && values[2] != null)
      {
        Activity activity = (Activity)values[2];
        bool flag = _bioEngine.IsActivityAllowed(activity);

        if (flag)
        {
          bool isEnabledFlag = (bool)values[1];
          return isEnabledFlag;
        }

        return flag;
      }
      return null;

    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
        System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    private IBioEngine _bioEngine;
  }
  #endregion

  #region MultiUserAccessToLocationConverter
  public class MultiUserAccessToLocationConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
      if (values != null)
      {
        long id = (long)values[0];
        ISet<long> set = (ISet<long>)values[1];
        return (set != null && set.Contains(id)) ? ResourceLoader.OkIconSource : ResourceLoader.CancelIconSource;
      }

      return ResourceLoader.OkIconSource;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
        System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
  #endregion

  #region MultiAccessDeviceToLocationConverter
  public class MultiAccessDeviceToLocationConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
      if (values != null)
      {
        string collectionItem = values[0] != null ? values[0].ToString() : string.Empty;
        string actualItem = values[1] != null ? values[1].ToString() : string.Empty;
        string desiredItem = values[2] != null ? values[2].ToString() : string.Empty;

        if (actualItem != string.Empty && collectionItem == actualItem && desiredItem == actualItem)
          return ResourceLoader.OkIconSource;
        else if (actualItem != string.Empty && desiredItem != string.Empty && desiredItem != actualItem && desiredItem == collectionItem)
          return ResourceLoader.OkIconSource;
        else if (actualItem == string.Empty && desiredItem != string.Empty && desiredItem == collectionItem)
          return ResourceLoader.OkIconSource;
      }

      return ResourceLoader.CancelIconSource;
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
