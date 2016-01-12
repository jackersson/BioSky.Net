using BioData;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BioModule.ViewModels
{
  public class LocationAccessDevicesViewModel : Screen
  {
    public LocationAccessDevicesViewModel()
    {
      DisplayName = "Access Devices";

      AccessDevices = new ObservableCollection<string>();

      AccessDevices.Add("COM1");
      AccessDevices.Add("COM2");
      AccessDevices.Add("COM3");
    }

    public void Update( Location location)
    {

    }

    public void OnMouseDown(DependencyObject e )
    {
      Console.WriteLine("Here");
      DragDrop.DoDragDrop(e, SelectedItem, DragDropEffects.Move);
    }

    private ObservableCollection<string> _accessDevicesIn;
    public ObservableCollection<string> AccessDevicesIn
    {
      get { return _accessDevicesIn; }
      set
      {
        if (_accessDevicesIn != value)
        {
          _accessDevicesIn = value;
          NotifyOfPropertyChange(() => AccessDevicesIn);
        }
      }
    }

    private string _selectedItemIn;
    public string SelectedItemIn
    {
      get { return _selectedItemIn; }
      set
      {
        if (_selectedItemIn != value)
        {
          _selectedItemIn = value;
          NotifyOfPropertyChange(() => SelectedItemIn);
        }
      }
    }

    private ObservableCollection<string> _accessDevices;
    public ObservableCollection<string> AccessDevices
    {
     get { return _accessDevices; }
      set
      {
        if ( _accessDevices != value )
        {
          _accessDevices = value;
          NotifyOfPropertyChange(() => AccessDevices);
        }
      }
    }

    private string _selectedItem;
    public string SelectedItem
    {
      get { return _selectedItem; }
      set
      {
        if (_selectedItem != value)
        {
          _selectedItem = value;
          NotifyOfPropertyChange(() => SelectedItem);
        }
      }
    }

    private Location _currentLocation;
    public Location CurrentLocation
    {
      get { return _currentLocation; }
      set
      {
        if ( _currentLocation != value )
        {
          _currentLocation = value;
          NotifyOfPropertyChange(() => CurrentLocation);
        }
      }
    }

  }
}
