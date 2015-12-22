using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using BioContracts;
using System.Windows.Data;
using BioModule.ResourcesLoader;
using BioModule.Utils;

namespace BioModule.ViewModels
{
  public class NotificationsViewModel : PropertyChangedBase
  {

    public NotificationsViewModel(/*TrackLocation location*/)
    {
      //_location = location;

      _notifications = new ObservableCollection<Notification>();
      /*
      Notification n = new Notification()
      {
        Status = "Success"
        , Detection_Time = "21.05.21"
        , InfoMessage = "Message"
        , LocationName = "Main Doors"
        , AdditionalVisualInfo = "Photo"
      };

      _notifications.Add(n);
      _notifications.Add(n);
      _notifications.Add(n);
      _notifications.Add(n);
      _notifications.Add(n);
      _notifications.Add(n);
      _notifications.Add(n);
      _notifications.Add(n);

      NotifyOfPropertyChange(() => Notifications);
      */
    }

    private ObservableCollection<Notification> _notifications;
    public ObservableCollection<Notification> Notifications
    {
      get { return _notifications; }
      set
      {
        if (_notifications != value)
        {
          _notifications = value;
          NotifyOfPropertyChange(() => Notifications);
        }
      }
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
}
