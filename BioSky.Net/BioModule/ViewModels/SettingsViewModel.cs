using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

using System.Windows.Input;
using System.Windows.Media;

namespace BioModule.ViewModels
{
  public class SettingsViewModel : Screen
  {
       
    public SettingsViewModel()
    {
      DisplayName = "Location Settings";
    }
    //************************************************Drag & Drop*****************************************************************

    public void OnMouseDown(MouseButtonEventArgs e)
    {                  
      var listbox = (ListBox)e.Source;
      if (listbox == null) return;

      var listItem = FindElementFromSource<ListBoxItem>((DependencyObject)e.OriginalSource);
      if (listItem == null) return;

      var draggedElement = new ListBoxItem { Content = listItem.Content };
      var dataObject = new DataObject(typeof(ListBoxItem), draggedElement);
      DragDrop.DoDragDrop(listbox, dataObject, DragDropEffects.Move);          
    }

    static T FindElementFromSource<T>(DependencyObject source) where T : UIElement
    {
      var dep = source;
      while ((dep != null) && !(dep is T))
        dep = VisualTreeHelper.GetParent(dep);
      return dep as T;
    }
    
    public void OnDragDropOut(DragEventArgs e)
    {
      if (e.Data.GetDataPresent("System.Windows.Controls.ListBoxItem"))
      {
        ListBoxItem contact = e.Data.GetData("System.Windows.Controls.ListBoxItem") as ListBoxItem;
        LocationsOut.Add(contact);
      }
    }

    public void OnDragDropIn(DragEventArgs e)
    { 
      if (e.Data.GetDataPresent("System.Windows.Controls.ListBoxItem"))
      {
        ListBoxItem contact = e.Data.GetData("System.Windows.Controls.ListBoxItem") as ListBoxItem;
        LocationsIn.Add(contact); 
      }
    }

    public void OnDragDropEmail(DragEventArgs e)
    {
      if (e.Data.GetDataPresent("System.Windows.Controls.ListBoxItem"))
      {
        ListBoxItem contact = e.Data.GetData("System.Windows.Controls.ListBoxItem") as ListBoxItem;
        Email.Add(contact);
      }
    }  
  
    //*************************************************Context Menu******************************************************************

    public void OnMouseRightButtonDown(MouseButtonEventArgs e)
    {

      var listbox = (ListBox)e.Source;
      if (listbox == null) return;

      ListBoxItem listItem = FindElementFromSource<ListBoxItem>((DependencyObject)e.OriginalSource);
      if (listItem == null)
      {
        MenuOpenStatus = false;
        return;
      }
      else
        MenuOpenStatus = true;
    }

    public void OnAddToInDevices() 
    {
      SelectedLocation.IsEnabled = false;
      ListBoxItem li = new ListBoxItem();
      li.Content = SelectedLocation.Content;
      LocationsIn.Add(li);                   
    }  

    public void OnAddToOutDevices()
    {
      SelectedLocation.IsEnabled = false;
      ListBoxItem li = new ListBoxItem();
      li.Content = SelectedLocation.Content;
      LocationsOut.Add(li);
    }

    public void OnDeleteInAllDevices()
    {
      Locations.Remove(SelectedLocation);
    }

    public void OnRemoveIn()
    {   
      for (int i = 0; i < Locations.Count; i++)
      {
        if (Locations[i].Content == SelectedDeviceIn.Content)
        {
          Locations[i].IsEnabled = true;
          break;
        }
      }
      LocationsIn.Remove(SelectedDeviceIn);      
    }

    public void OnAddToOutDevicesFromIn()
    {
      ListBoxItem li = new ListBoxItem();
      li.Content = SelectedDeviceIn.Content;
      LocationsOut.Add(li);
      LocationsIn.Remove(SelectedDeviceIn);     
    }

    public void OnRemoveOut()
    {
      for (int i = 0; i < Locations.Count; i++)
      {
        if (Locations[i].Content == SelectedDeviceOut.Content)
        {
          Locations[i].IsEnabled = true;
          break;
        }
      }
      LocationsOut.Remove(SelectedDeviceOut);
    }

    public void OnAddToInDevicesFromOut()
    {
      ListBoxItem li = new ListBoxItem();
      li.Content = SelectedDeviceOut.Content;
      LocationsIn.Add(li);
      LocationsOut.Remove(SelectedDeviceOut);
    }

    //************************************************Icon Source*********************************************************************

    public BitmapSource SaveIconSource
    {
      get { return ResourceLoader.OkIconSource; }
    }

    public BitmapSource CancelIconSource
    {
      get { return ResourceLoader.CancelIconSource; }
    }

    public BitmapSource RefreshIconSource
    {
      get { return ResourceLoader.RefreshIconSource; }
    }

    //***************************************************Property*********************************************************************

    public ObservableCollection<ListBoxItem> LocationsOut { get; private set; }
    public ObservableCollection<ListBoxItem> LocationsIn { get; private set; }
    public ObservableCollection<ListBoxItem> Locations { get; private set; }
    public ObservableCollection<ListBoxItem> Email { get; private set; }
    public ObservableCollection<ListBoxItem> Users { get; private set; }

    private ListBoxItem _selectedLocation;
    public ListBoxItem SelectedLocation
    {
      get
      {
        return _selectedLocation;
      }
      set
      {
        if (_selectedLocation != value)
          _selectedLocation = value;

        NotifyOfPropertyChange(() => SelectedLocation);
      }
    }

    private ListBoxItem _selectedDeviceIn;
    public ListBoxItem SelectedDeviceIn
    {
      get
      {
        return _selectedDeviceIn;
      }
      set
      {
        if (_selectedDeviceIn != value)
          _selectedDeviceIn = value;

        NotifyOfPropertyChange(() => SelectedDeviceIn);
      }
    }

    private ListBoxItem _selectedDeviceOut;
    public ListBoxItem SelectedDeviceOut
    {
      get
      {
        return _selectedDeviceOut;
      }
      set
      {
        if (_selectedDeviceOut != value)
          _selectedDeviceOut = value;

        NotifyOfPropertyChange(() => SelectedDeviceOut);
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
  }
}
