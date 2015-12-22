using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using BioModule.ResourcesLoader;
using System.Windows.Media.Imaging;

using BioData;
using System.Collections.ObjectModel;
using BioModule.Model;

using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

using BioModule.Utils;


namespace BioModule.ViewModels
{
  public class VisitorsViewModel : PropertyChangedBase
  {  
    public VisitorsViewModel(IBioEngine bioEngine, ViewModelSelector selector)
    {
      _bioEngine    = bioEngine;
      _visitors     = new ObservableCollection<Visitor>();
      _selector     = selector;
      
      List <Visitor> visitors = (List<Visitor>)_bioEngine.Database().getAllVisitors();
      foreach (Visitor visitor in visitors)
        _visitors.Add(visitor);

      Visitor visitor2 = new Visitor {  Status =  "allow" };
      _visitors.Add(visitor2);

      Visitor visitor3 = new Visitor { Status = "allow2" };
      _visitors.Add(visitor3);

      Visitor visitor5 = new Visitor { Status = "allow3" };
      _visitors.Add(visitor5);
      NotifyOfPropertyChange(() => Visitors);
    }

    private ObservableCollection<Visitor> _visitors;
    public ObservableCollection<Visitor> Visitors
    {
      get { return _visitors; }
      set
      {
        if (_visitors != value)
        {
          _visitors = value;
          NotifyOfPropertyChange(() => Visitors);
        }
      }
    }

    public string Caption()
    {
      return "Visitors";
    }

  
//**********************************************************Context Menu*****************************************************
    
  
    private Visitor _selectedItem;
    public Visitor SelectedItem
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

    private bool _canOpenInNewTab;
    public bool CanOpenInNewTab
    {
      get { return _canOpenInNewTab; }
      set
      {
        if (_canOpenInNewTab != value)
        {
          _canOpenInNewTab = value;
          NotifyOfPropertyChange(() => CanOpenInNewTab);
        }
      }
    }

    public void OnMouseRightButtonDown(Visitor visitor)
    {
      CanOpenInNewTab = (visitor != null);
      SelectedItem = visitor;
    }

    public void ShowUserPage()
    {
      _selector.OpenTab(ViewModelsID.UserPage, new object[] { SelectedItem.User });
    }
    public void RemoveVisitor()
    {
      _visitors.Remove(SelectedItem);
    }

    private readonly ViewModelSelector _selector;
    private readonly IBioEngine _bioEngine;

    //--------------------------------------------------- UI --------------------------------------
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
