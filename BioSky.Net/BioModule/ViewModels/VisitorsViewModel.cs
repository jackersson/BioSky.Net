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

using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

using BioModule.Utils;
using BioContracts;
using BioFaceService;
using Google.Protobuf.Collections;

namespace BioModule.ViewModels
{
  public class VisitorsViewModel : Screen
  {      
    public VisitorsViewModel(IProcessorLocator locator )
    {
      _locator = locator;     

      //_visitors         = new RepeatedField<Visitor>();      

      //IBioEngine bioEngine = locator.GetProcessor<IBioEngine>();
     // Visitors = bioEngine.Database().Visitors.Visitors;
      

      DisplayName = "Visitors";
    }


    //TODO only when data comes
    public void Init()
    {
      IBioEngine bioEngine = _locator.GetProcessor<IBioEngine>();
      Visitors = bioEngine.Database().Visitors.Visitors;
    }


    public void Update()
    {
      NotifyOfPropertyChange(() => Visitors);
    }
    
    private RepeatedField<Visitor> _visitors;
    public RepeatedField<Visitor> Visitors
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
    
    private readonly IProcessorLocator _locator;

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
