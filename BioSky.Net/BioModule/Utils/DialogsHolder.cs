using BioModule.ViewModels; 
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

    private PeriodTimePickerViewModel _periodTimePicker;
    public PeriodTimePickerViewModel PeriodTimePicker
    {
      get
      {
        if (_periodTimePicker == null)
          return _periodTimePicker = new PeriodTimePickerViewModel(_locator);
        return _periodTimePicker;
      }
    }

    private AuthenticationPageViewModel _authenticationPage;
    public AuthenticationPageViewModel AuthenticationPage
    {
      get
      {
        if (_authenticationPage == null)
          return _authenticationPage = new AuthenticationPageViewModel(_locator);
        return _authenticationPage;
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
          return _cameraDialog = new CameraDialogViewModel(_locator); 
        return _cameraDialog; 
      } 
    } 

    private CustomTextDialogViewModel _customTextDialog; 
    public CustomTextDialogViewModel CustomTextDialog
    { 
      get 
      { 
        if (_customTextDialog == null) 
          return _customTextDialog = new CustomTextDialogViewModel(_windowManager); 
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


    private readonly IWindowManager    _windowManager; 
    private readonly IProcessorLocator _locator      ; 


  } 
} 

