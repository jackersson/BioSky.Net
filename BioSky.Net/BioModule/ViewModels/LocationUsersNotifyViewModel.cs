﻿using BioData;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioModule.DragDrop;

namespace BioModule.ViewModels
{
  public class LocationUsersNotifyViewModel : Screen
  {
    public LocationUsersNotifyViewModel()
    {
      DisplayName = "Users Notification";

      DragableWithDisabledItem disabledDragable = new DragableWithDisabledItem();
      DragableWithRemoveItem   removeDragable   = new DragableWithRemoveItem();

      UsersList       = new DragablListBoxViewModel(disabledDragable);
      UsersNotifyList = new DragablListBoxViewModel(removeDragable);
      UsersNotifyList.ItemRemoved += UsersList.ItemDropped;      
    }

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
  }
}
