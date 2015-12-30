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

using System.Windows.Data;
using BioModule.Utils;
using System.Windows.Controls;


namespace BioModule.ViewModels
{
  public class TrackControlViewModel : Screen
  {
    public TrackControlViewModel(IBioEngine bioEngine, ViewModelSelector selector)
    {
      _bioEngine = bioEngine;
      _selector = selector;
      _notifications = new VisitorsViewModel(bioEngine, selector);

      DisplayName = "Tracking";

      foreach (TrackLocation location in _bioEngine.TrackLocationEngine().TrackLocations())      
        location.ScreenViewModel = new TrackControlItemViewModel(_bioEngine, location, _selector);    

      OnChecked();
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
    private readonly IBioEngine        _bioEngine    ;
    private readonly ViewModelSelector _selector     ;

    //******************************************ComboBoxLocationCheck**************************

    private string _selectedItems;
    public string SelectedItems
    {
      get
      {
        return _selectedItems;
      }
      set
      {
        if (_selectedItems != value)
          _selectedItems = value;

        NotifyOfPropertyChange(() => SelectedItems);
      }
    }

    public void OnChecked()
    {
      string caption = "";

      int count = 0;
      foreach (TrackLocation pair in _bioEngine.TrackLocationEngine().TrackLocations())
      {
        caption += pair.IsChecked ? (pair.Caption + ";") : "";
        if (pair.IsChecked)
          ++count;
      }

      if (count >= _bioEngine.TrackLocationEngine().TrackLocations().Count)
        caption = "All";

      if (count == 0)
        caption = "None";

      SelectedItems = caption;
    }
    
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
  
  public class ComboBoxItemTemplateSelector : DataTemplateSelector
  {
    // Can set both templates from XAML
    public DataTemplate SelectedItemTemplate { get; set; }
    public DataTemplate ItemTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      bool selected = false;

      // container is the ContentPresenter
      FrameworkElement fe = container as FrameworkElement;
      if (fe != null)
      {
        DependencyObject parent = fe.TemplatedParent;
        if (parent != null)
        {
          ComboBox cbo = parent as ComboBox;
          if (cbo != null)
            selected = true;
        }
      }

      if (selected)
        return SelectedItemTemplate;
      else
        return ItemTemplate;
    }
  }
}
