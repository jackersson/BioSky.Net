
using Caliburn.Micro;
using BioContracts;
using BioModule.Utils;

namespace BioModule.ViewModels
{
  //Top right corner (User information)
  public class LoginInformationViewModel : PropertyChangedBase
  {
    public LoginInformationViewModel(IProcessorLocator locator)
    {
      _locator = locator;
      _dialogsHolder     = _locator.GetProcessor<DialogsHolder>();
      _viewModelSelector = _locator.GetProcessor<ViewModelSelector>();
    }


    public void OpenTabAuthentication()
    {
      _dialogsHolder.AuthenticationPage.Show();
    }

    private readonly IProcessorLocator _locator          ;
    private          ViewModelSelector _viewModelSelector;
    private          DialogsHolder     _dialogsHolder    ;
                                       

  }
}
