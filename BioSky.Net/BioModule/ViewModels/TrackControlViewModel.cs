using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioContracts;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using BioModule.ResourcesLoader;
using BioModule.Model;

using System.Windows;
using System.Windows.Media.Imaging;
using BioData;

namespace BioModule.ViewModels
{
  public class TrackControlViewModel : PropertyChangedBase
  {

    public TrackControlViewModel(IBioEngine bioEngine)
    {
      _bioEngine = bioEngine;

      _notifications = new VisitorsViewModel(bioEngine);

      foreach (TrackLocation location in _bioEngine.TrackLocationEngine().TrackLocations())
        location.ScreenViewModel = new TrackControlItemViewModel(_bioEngine, location);      
    }

    public ObservableCollection<TrackLocation> TrackControlItems
    {
      get {  return _bioEngine.TrackLocationEngine().TrackLocations();  }      
    }

    private TrackLocation _selectedTrackLocation;
    public TrackLocation SelectedTrackLocation
    {
      get { return _selectedTrackLocation; }
      set
      {
        if (_selectedTrackLocation == value)
          return;
        _selectedTrackLocation = value;
        NotifyOfPropertyChange(() => SelectedTrackLocation);
       
      }
    }   

    public void AddMenu()
    {
      Visitor v = new Visitor()
      {        
         User_UID = 1       
        , Locaion_ID = 2
        , Status = "VerificationSuccess"         
      };

      _bioEngine.Database().AddVisitor(v);
      _bioEngine.Database().SaveChanges();

      foreach (TrackLocation location in _bioEngine.TrackLocationEngine().TrackLocations())
      {
        TrackControlItemViewModel vm = (TrackControlItemViewModel)location.ScreenViewModel;
        if (vm != null)
          vm.Update();
      }
    }

    public VisitorsViewModel Notifications
    {
      get { return _notifications; }
    }

    
    public string Caption()
    {
      return "Tracking";
    }

    private readonly VisitorsViewModel _notifications;
    private readonly IBioEngine _bioEngine;

    //**************************************** UI *******************************************
    public BitmapSource AddIconSource
    {
      get { return ResourceLoader.AddIconSource; }
    }

    public BitmapSource RemoveIconSource
    {
      get { return ResourceLoader.RemoveIconSource; }
    }

    public BitmapSource DeleteIconSource
    {
      get { return ResourceLoader.DeleteIconSource; }
    }
  }
}
