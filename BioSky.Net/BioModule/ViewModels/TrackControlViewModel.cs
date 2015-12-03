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



    public TrackControlViewModel()
    {
  
      _trackControl = new ShellTrackControl();
      _trackControl.TrackItems.Add(new ShellTrackItem() { Caption = "MainDoors", ScreenViewModel = new TrackControlItemViewModel() });

      NotifyOfPropertyChange(() => TrackControlItems);
      NotifyOfPropertyChange(() => CurrentViewTab);
    }
  }
}
