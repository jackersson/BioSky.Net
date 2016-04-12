﻿using Caliburn.Micro;
using BioService;
using System.Linq;
using System.Collections.Generic;
using BioContracts;

namespace BioModule.ViewModels
{
  public class NotificationDialogViewModel : Screen, INotificationDialog
  {
    public NotificationDialogViewModel(IProcessorLocator locator
                                     , string title = "NotificationDialog")
    {
      _locator       = locator;
      _windowManager = _locator.GetProcessor<IWindowManager>();

      TreeItems = new List<TreeItem>();
    }

    public void Update(List<TreeItem> list                     
                      , string title = "NotificationDialog")
    {
      DisplayName = title;

      if (list == null)
        return;

      TreeItems = list;

/*      TreeItems = new List<TreeItem>();

      foreach (Location location in list.Locations)
      {
        TreeItem treeitem = new TreeItem()
        {
          Name = location.LocationName
        , IsSuccess = (location.Dbresult == ResultStatus.Success) ? true : false
        , Members = new List<TreeItem>()

        };
        if(location.AccessDevices.Count > 0)
        {
          AccessDevice aDevice = location.AccessDevices.FirstOrDefault();
          TreeItem member1;
          treeitem.Members.Add(member1 = new TreeItem()
          {
            Name = aDevice.Portname
                                              ,
            IsSuccess = (aDevice.Dbresult == ResultStatus.Success) ? true : false
                                              ,
            Members = new List<TreeItem>()
          });
        }

        if(location.CaptureDevices.Count > 0)
        {
          CaptureDevice cDevice = location.CaptureDevices.FirstOrDefault();
          treeitem.Members.Add(new TreeItem() { Name = cDevice.Devicename
                                              , IsSuccess = (cDevice.Dbresult == ResultStatus.Success) ? true : false });
        }

        TreeItems.Add(treeitem);
      }*/



    }

    public void Show()
    {
      _windowManager.ShowDialog(this);
    }

    public void Apply()
    {
      DialogResult = true;
      this.TryClose(true);
    }

    public void Cancel()
    {
      DialogResult = false;
      this.TryClose(false);
    }

    public bool GetDialogResult()
    {
      return DialogResult;
    }

    private bool _dialogResult;
    public bool DialogResult
    {
      get { return _dialogResult; }
      set
      {
        if (_dialogResult != value)
        {
          _dialogResult = value;
        }
      }
    }

    private List<TreeItem> _treeItems;
    public List<TreeItem> TreeItems
    {
      get { return _treeItems; }
      set
      {
        if (_treeItems != value)
        {
          _treeItems = value;
          NotifyOfPropertyChange(() => TreeItems);
        }
      }
    }

    private IWindowManager    _windowManager;
    private IProcessorLocator _locator      ;
  }
}
