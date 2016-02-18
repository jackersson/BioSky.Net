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
    public CustomTextDialogViewModel(string title = "CustomtextDialog", string text = "", DialogStatus status = DialogStatus.Info)
    {
      Update(title, text, status);
    }

    public void Update(string title = "CustomtextDialog", string text = "", DialogStatus status = DialogStatus.Info)
    {
      DisplayName = title ;
      Text        = text  ;
      Status      = status;
    }

    public void Apply()
    {
      DialogResult = true;
      this.TryClose(DialogResult);
    }

    public void Cancel()
    {
      DialogResult = false;
      this.TryClose(DialogResult);
    }
    public BitmapSource CustomDialogIcon
    {
      get 
      {
        if (Status == DialogStatus.Error)
          return ResourceLoader.ErrorIconSource;
        else if (Status == DialogStatus.Ok)
          return ResourceLoader.OkIconSource;
        else if (Status == DialogStatus.Info)
          return ResourceLoader.InformationCircleIconSource;

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
    public DialogStatus Status
    {
      get { return _status; }
      set
      {
        if (_status != value)
        {
          _status = value;
          NotifyOfPropertyChange(() => Status);
        }
      }
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
          NotifyOfPropertyChange(() => DialogResult);
        }
      }
    }
  }
}
