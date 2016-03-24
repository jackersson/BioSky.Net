using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioContracts;
using BioModule.ViewModels;

namespace BioModule.Utils
{

  public enum ViewModelsID
  {
      UserPage = 0
    , LocationSettings
    , VisitorsPage
    , UsersPage
    , TrackPage
    , GeneralSettings
    , AuthenticationPage
    , HomePage
    , ErrorViewer
  }

  public enum ShowableContentControl
  {
     TabControlContent
   , FlyoutControlContent
  }

  public class ViewModelSelector
  {
    public ViewModelSelector( TabViewModel tabControl, FlyoutControlViewModel flyoutControl )
    {
    
      _viewModels = new Dictionary<ViewModelsID, Type>();

      _viewModels.Add(ViewModelsID.UserPage          , Type.GetType("BioModule.ViewModels.UserPageViewModel"           ));
      _viewModels.Add(ViewModelsID.LocationSettings  , Type.GetType("BioModule.ViewModels.LocationPageViewModel"       ));
      _viewModels.Add(ViewModelsID.VisitorsPage      , Type.GetType("BioModule.ViewModels.VisitorsViewModel"           ));
      _viewModels.Add(ViewModelsID.UsersPage         , Type.GetType("BioModule.ViewModels.UsersViewModel"              ));
      _viewModels.Add(ViewModelsID.TrackPage         , Type.GetType("BioModule.ViewModels.TrackControlViewModel"       ));
      _viewModels.Add(ViewModelsID.GeneralSettings   , Type.GetType("BioModule.ViewModels.GeneralSettingsPageViewModel"));
      _viewModels.Add(ViewModelsID.AuthenticationPage, Type.GetType("BioModule.ViewModels.AuthenticationPageViewModel" ));
      _viewModels.Add(ViewModelsID.HomePage          , Type.GetType("BioModule.ViewModels.HomePageViewModel"           ));
      _viewModels.Add(ViewModelsID.ErrorViewer       , Type.GetType("BioModule.ViewModels.ErrorViewerDialogViewModel"        ));





      _showableControls = new Dictionary<ShowableContentControl, IShowableContent>();
      _showableControls.Add(ShowableContentControl.TabControlContent   , tabControl   );
      _showableControls.Add(ShowableContentControl.FlyoutControlContent, flyoutControl);
    }

    public void ShowContent(ShowableContentControl contentControl, ViewModelsID pageID, object[] args = null)
    {
      Type pageType;
      bool flag = _viewModels.TryGetValue(pageID, out pageType);
      if (flag)
      {
        IShowableContent showableControl;
        flag = _showableControls.TryGetValue(contentControl, out showableControl);
        if ( flag )
          showableControl.ShowContent(pageType, args);
      }        
    }

 
    private Dictionary<ViewModelsID, Type> _viewModels;
    private Dictionary<ShowableContentControl, IShowableContent> _showableControls;

    
  }
}
