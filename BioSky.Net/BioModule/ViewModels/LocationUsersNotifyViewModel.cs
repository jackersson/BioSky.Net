using BioData;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioModule.DragDrop;
using BioContracts;
using BioService;
using BioModule.Utils;

namespace BioModule.ViewModels
{
  public class LocationUsersNotifyViewModel : Screen, IUpdatable
  {
    public LocationUsersNotifyViewModel(IProcessorLocator locator, IWindowManager windowManager)
    {
      DisplayName = "UsersNotification";

      _locator = locator;
      _bioService = _locator.GetProcessor<IServiceManager>();
      _bioEngine = _locator.GetProcessor<IBioEngine>();

      DragableWithDisabledItem disabledDragable = new DragableWithDisabledItem();
      DragableWithRemoveItem   removeDragable   = new DragableWithRemoveItem();

      UsersList       = new DragablListBoxViewModel(disabledDragable);
      UsersNotifyList = new DragablListBoxViewModel(removeDragable);
      UsersNotifyList.ItemRemoved += UsersList.ItemDropped;
      /*

      foreach (Person item in _bioEngine.Database().Persons)
      {
        DragableItem dragableItem = new DragableItem() { ItemContext = item, ItemEnabled = true, DisplayName = item.Firstname + " " + item.Lastname };
        AddToGeneralDeviceList(dragableItem);
      }
      */
    }

    public void AddToGeneralDeviceList(DragableItem item, bool isEnabled = true)
    {
      if (item == null)
        return;

      DragableItem newItem = item.Clone();
      newItem.ItemEnabled = isEnabled;
      UsersList.Add(newItem);
    }
/*
    private void OnPersonsChanged(PersonList Persons)
    {
      foreach (Person item in Persons.Persons)
      {
        DragableItem dragableItem = new DragableItem() { ItemContext = item, ItemEnabled = true, DisplayName = item.Firstname + " " + item.Lastname};

        if (UsersList.ContainsItem(dragableItem))
        {
          return;
        }

        AddToGeneralDeviceList(dragableItem);
      }
    }*/

    private DragablListBoxViewModel _usersList;
    public DragablListBoxViewModel UsersList
    {
      get { return _usersList; }
      set
      {
        if (_usersList != value)
        {
          _usersList = value;
          NotifyOfPropertyChange(() => UsersList);
        }
      }
    }

    private DragablListBoxViewModel _usersNotifyList;
    public DragablListBoxViewModel UsersNotifyList
    {
      get { return _usersNotifyList; }
      set
      {
        if (_usersNotifyList != value)
        {
          _usersNotifyList = value;
          NotifyOfPropertyChange(() => UsersNotifyList);
        }
      }
    }
    public void Update(Location location)
    {
      _location = location;
    }

    public void Apply()
    {

    }
    public void Remove(bool all)
    {

    }

    private          Location          _location  ;
    private readonly IProcessorLocator _locator   ;
    private readonly IBioEngine        _bioEngine ;
    private readonly IServiceManager   _bioService;
  }
}
