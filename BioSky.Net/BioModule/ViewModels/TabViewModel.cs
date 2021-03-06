﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using System.Collections.ObjectModel;
using BioContracts;
using System.Windows;

namespace BioModule.ViewModels
{
  public class TabViewModel : PropertyChangedBase
  {
    private ObservableCollection<ShellTabPage> _tabPages;
    ShellTabControl _tabControl;
    public TabViewModel()
    {
     
    }

    public void update(ShellTabControl tabcontrol)
    {
      _tabControl = tabcontrol;
      TabPages = _tabControl.TabPages;
    }


    public void AddTabPage()
    {      
    }

    public ObservableCollection<ShellTabPage> TabPages
    {
      get { return _tabPages; }
      private set
      {
        if (_tabPages!=value)
        {
          _tabPages = value;
          NotifyOfPropertyChange(() => TabPages);
        }        
      }
    }
      
    private ShellTabPage _selectedTabPage;
    public ShellTabPage SelectedTabPage
    {
      get { return _selectedTabPage; }
      set
      {
        if (_selectedTabPage == value)
          return;
        _selectedTabPage = value;
        NotifyOfPropertyChange(() => SelectedTabPage);
        NotifyOfPropertyChange(() => CurrentViewTab);
      }
    }
      
    public object CurrentViewTab
    {
      get { return _selectedTabPage == null ? null : _selectedTabPage.ScreenViewModel; }
    }
  }
}
