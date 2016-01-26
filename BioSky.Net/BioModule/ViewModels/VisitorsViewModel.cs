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
      DisplayName = "Visitors";

      _locator = locator;
      _bioEngine = locator.GetProcessor<IBioEngine>();
      _selector = locator.GetProcessor<ViewModelSelector>();
      _bioService = _locator.GetProcessor<IServiceManager>();

      _visitors         = new RepeatedField<Visitor>();

      _bioEngine.Database().DataChanged += VisitorsViewModel_DataChanged;     

    }

    protected async override void OnActivate()
    {
      await _bioService.DatabaseService.VisitorRequest(new CommandVisitor());
    }

    public void VisitorsViewModel_DataChanged(object sender, EventArgs args)
    {
      OnPersonsChanged(_bioEngine.Database().Visitors);
    }

    private void OnPersonsChanged(VisitorList visitors)
    {
      foreach (Visitor item in visitors.Visitors)
      {
        if (Visitors.Contains(item))
          return;

        Visitors.Add(item);
      }
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

    private readonly IProcessorLocator _locator   ;
    private readonly ViewModelSelector _selector  ;
    private readonly IBioEngine        _bioEngine ;
    private readonly IServiceManager   _bioService;
  }
}
