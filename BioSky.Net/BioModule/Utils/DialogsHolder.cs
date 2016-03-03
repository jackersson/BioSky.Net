﻿using BioModule.ViewModels;
using Caliburn.Micro;
using BioContracts;

namespace BioModule.Utils
{
  public class DialogsHolder
  {
    public DialogsHolder(IProcessorLocator locator)
    {
      _locator = locator;
      _windowManager = _locator.GetProcessor<IWindowManager>();
    }

    private NotificationDialogViewModel _notificationDialog;
    public NotificationDialogViewModel NotificationDialog
    {
      get
      {
        if (_notificationDialog == null)
          _notificationDialog = new NotificationDialogViewModel(_windowManager, null);

        return _notificationDialog;
      }
    }

    private AboutDialogViewModel _aboutDialog;
    public AboutDialogViewModel AboutDialog
    {
      get
      {
        if(_aboutDialog == null)
          return _aboutDialog = new AboutDialogViewModel(_windowManager);
        return _aboutDialog;
      }
    }

    private CameraDialogViewModel _cameraDialog;
    public CameraDialogViewModel CameraDialog
    {
      get
      {
        if (_cameraDialog == null)
          return _cameraDialog = new CameraDialogViewModel(_locator, _windowManager);
        return _cameraDialog;
      }
    }

    private CustomTextDialogViewModel _customTextDialog;
    public CustomTextDialogViewModel CustomTextDialog
    {
      get
      {
        if (_customTextDialog == null)
          return _customTextDialog = new CustomTextDialogViewModel( _windowManager);
        return _customTextDialog;
      }
    }

    private CustomTextDialogViewModel _areYouSureDialog;
    public CustomTextDialogViewModel AreYouSureDialog
    {
      get
      {
        if (_areYouSureDialog == null)
          return _areYouSureDialog = new CustomTextDialogViewModel(_windowManager
                                                                  , "Confirmation Dialog"
                                                                  , "Are you sure?"
                                                                  , DialogStatus.Help, 20);
        return _areYouSureDialog;
      }
    }

    private LoginDialogViewModel _loginDialog;
    public LoginDialogViewModel LoginDialog
    {
      get
      {
        if (_loginDialog == null)
          return _loginDialog = new LoginDialogViewModel(_windowManager);
        return _loginDialog;
      }
    }

    private readonly IWindowManager    _windowManager;
    private readonly IProcessorLocator _locator      ;

  }
}
