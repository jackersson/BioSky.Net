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
    , Help
    , Info2
  }
  public class CustomTextDialogViewModel : Screen
  {
    public CustomTextDialogViewModel( IWindowManager windowManager
                                    , string title = "CustomtextDialog"
                                    , string text = "" 
                                    , DialogStatus status = DialogStatus.Info
                                    , int fontSize = 14)
    {
      _windowManager = windowManager;
      Update(title, text, status, fontSize);
    }

    public void Update( string title = "CustomtextDialog"
                      , string text = ""
                      , DialogStatus status = DialogStatus.Info
                      , int fontSize = 14)
    {
      DisplayName = title   ;
      Text        = text    ;
      _status     = status  ;
      FontSize    = fontSize;
    }

    public bool? Show()
    {
      return _windowManager.ShowDialog(this);
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
          case DialogStatus.Help:
            return ResourceLoader.HelpDialogIconSource;
          case DialogStatus.Info2:
            return ResourceLoader.InfoDialogIconSource;  
        }        
        return ResourceLoader.InformationCircleIconSource;
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
        }
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

    private int _fontSize;
    public int FontSize
    {
      get { return _fontSize; }
      set
      {
        if (_fontSize != value)
        {
          _fontSize = value;
          NotifyOfPropertyChange(() => FontSize);
        }
      }
    }
    private DialogStatus   _status       ;
    private IWindowManager _windowManager;

  
  }
}
