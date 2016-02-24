using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;
using BioContracts;

namespace BioModule.ViewModels
{
  public enum DialogStatus
  {
      Error
    , Ok
    , Info    
  }
  public class CustomTextDialogViewModel : Screen
  {
    public CustomTextDialogViewModel( string title = "CustomtextDialog"
                                    , string text = ""
                                    , DialogStatus status = DialogStatus.Info)
    {
      Update(title, text, status);
    }

    public void Update( string title = "CustomtextDialog"
                      , string text = ""
                      , DialogStatus status = DialogStatus.Info)
    {
      DisplayName = title ;
      Text        = text  ;
      _status     = status;
    }

    public void Apply()
    {      
      this.TryClose(true);
    }

    public void Cancel()
    {      
      this.TryClose(false);
    }

    public BitmapSource CustomDialogIcon
    {
      get 
      {
        switch (_status)
        {
          case DialogStatus.Error:
            return ResourceLoader.ErrorIconSource;
          case DialogStatus.Ok:
            return ResourceLoader.OkIconSource;
          case DialogStatus.Info:
            return ResourceLoader.InformationCircleIconSource;
        }        
        return ResourceLoader.InformationCircleIconSource;
      }
    }


    private string _text;
    public string Text
    {
      get { return _text; }
      set
      {
        if (_text != value)
        {
          _text = value;
          NotifyOfPropertyChange(() => Text);
        }
      }
    }

    private DialogStatus _status;
  
  }
}
