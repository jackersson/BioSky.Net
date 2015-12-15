using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioContracts;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using BioModule.ResourcesLoader;

using System.Windows;
using System.Windows.Media.Imaging;

using BioData;
using BioModule.Model;

using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;


namespace BioModule.ViewModels
{
  public class TrackControlViewModel : PropertyChangedBase
  {
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

    private ShellTrackControl _trackControl;
    public ShellTrackControl TrackControl
    {
      get { return _trackControl; }
    }
    
    public ObservableCollection<ShellTrackItem> TrackControlItems
    {
      get
      {    
        return _trackControl.TrackItems;
      }  
    }

    public void AddMenu()
    {
      _trackControl.TrackItems.Add(new ShellTrackItem() { Caption = "Adede", ScreenViewModel = new TrackControlItemViewModel() });
    }


    private ShellTrackItem _selectedTabPage;
    public ShellTrackItem SelectedTabPage
    {
      get { return _selectedTabPage; }
      set
      {
        if (_selectedTabPage == value)
          return;
        _selectedTabPage = value;
        NotifyOfPropertyChange(() => SelectedTabPage);
        NotifyOfPropertyChange(() => CurrentViewTab);
      }
    }

    public object CurrentViewTab
    {
      get { return _trackControl.TrackItems[0].ScreenViewModel; }
    }

    public object CurrentTabControl
    {
      get { return _trackControl.ScreenViewModel; }
    }


    IBioEngine _bioEngine;
    public TrackControlViewModel(IBioEngine bioEngine)
    {
  
   

      _bioEngine = bioEngine;
      _locations = new ObservableCollection<Location>();

    


      List<Location> locations = (List<Location>)_bioEngine.Database().getAllLocations();
      foreach (Location location in locations)
      _locations.Add(location);
      
      NotifyOfPropertyChange(() => TrackControlItems);
      NotifyOfPropertyChange(() => CurrentViewTab);
      NotifyOfPropertyChange(() => Locations);

    }   


    private ObservableCollection<Location> _locations;
    public ObservableCollection<Location> Locations
    {
      get { return _locations; }
      set
      {
        if (_locations != value)
        {
          _locations = value;
          NotifyOfPropertyChange(() => Locations);
        }
      }
    }
    

    //**********************************************************Context Menu*****************************************************

  

    private Location _selectedItem;
    public Location SelectedItem
    {
      get
      {
        return _selectedItem;
      }
      set
      {
        if (_selectedItem != value)
          _selectedItem = value;

        NotifyOfPropertyChange(() => SelectedItem);
      }
    }

    private bool _menuOpenStatus;
    public bool MenuOpenStatus
    {
      get
      {
        return _menuOpenStatus;
      }
      set
      {
        if (_menuOpenStatus != value)
          _menuOpenStatus = value;

        NotifyOfPropertyChange(() => MenuOpenStatus);
      }
    }


    public void OnMouseRightButtonDown(MouseButtonEventArgs e)
    {
      
    }

  }
}
