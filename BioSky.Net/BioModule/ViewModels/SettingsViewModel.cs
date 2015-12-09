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
    public class SettingsViewModel : PropertyChangedBase
    {
      public SettingsViewModel()
      {
        _devicesIn  = new System.Windows.Controls.ListBox();
        _devicesOut = new System.Windows.Controls.ListBox();
        _devices    = new System.Windows.Controls.ListBox();

        Locations = new ObservableCollection<ListBoxItem>()
        {
            new ListBoxItem() { Content = "location 1"},
            new ListBoxItem() { Content = "location 2"},
            new ListBoxItem() { Content = "location 3"},
        };
        LocationsOut = new ObservableCollection<ListBoxItem>()
        {
        };

        LocationsIn = new ObservableCollection<ListBoxItem>()
        {
        };

        Email = new ObservableCollection<ListBoxItem>()
        {
        };
        Users = new ObservableCollection<ListBoxItem>()
        {
           new ListBoxItem() { Content = "adaman1991@mail.ru"},
           new ListBoxItem() { Content = "AlexSpark@mail.ru"},
           new ListBoxItem() { Content = "Spark@mail.ru"},
        };
      }
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

        private System.Windows.Controls.ListBox _devicesIn;
        public System.Windows.Controls.ListBox DevicesIn
        {
          get
          {
            return _devicesIn;
          }
          set
          {
            if (_devicesIn != value)
              _devicesIn = value;

            NotifyOfPropertyChange(() => DevicesIn);
          }
        }

        private System.Windows.Controls.ListBox _devicesOut;
        public System.Windows.Controls.ListBox DevicesOut
        {
          get
          {
            return _devicesOut;
          }
          set
          {
            if (_devicesOut != value)
              _devicesOut = value;

            NotifyOfPropertyChange(() => DevicesOut);
          }
        }

        private System.Windows.Controls.ListBox _devices;
        public System.Windows.Controls.ListBox Devices
        {
          get
          {
            return _devices;
          }
          set
          {
            if (_devices != value)
              _devices = value;

            NotifyOfPropertyChange(() => Devices);
          }
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

      //************************************************************************************************************************          
    }
}
