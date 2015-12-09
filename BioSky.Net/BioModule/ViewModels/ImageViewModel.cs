using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

using Caliburn.Micro;
using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;

using Microsoft.Win32;
using System.IO;
using System.Drawing;
using MahApps.Metro.Controls;

namespace BioModule.ViewModels
{


  class ImageViewModel : PropertyChangedBase
  {

    double _width;
    double _height;
    double _scale;
    


    public double Scale
    {
      get
      {
        return _scale;
      }
      set
      {
        if (_scale != value)
          _scale = value;

        NotifyOfPropertyChange(() => Scale);
      }
    }    

    private double CurrWidth
    {
      get
      {
        return _width;
      }
      set
      {
        if (_width != value)
          _width = value;

        NotifyOfPropertyChange(() => CurrWidth);
      }
    }

    private double CurrHeight
    {
      get
      {
        return _height;
      }
      set
      {
        if (_height != value)
          _height = value;

        NotifyOfPropertyChange(() => CurrHeight);
      }
    } 
   
    public ImageViewModel()
    {
      _currentImageControl = new System.Windows.Controls.Image();
      _sliderControl = new System.Windows.Controls.Slider();
      _toggleZoomToFit = new MahApps.Metro.Controls.ToggleSwitch();
    }

    private BitmapSource _currentImage;

    private System.Windows.Controls.ScrollViewer _currentImageScrollViewer;

    private System.Windows.Controls.ScrollViewer CurrentImageScrollViewer
    {
      get
      {
        return _currentImageScrollViewer;
      }
      set
      {       
        if (_currentImageScrollViewer != value)
          _currentImageScrollViewer = value;

        NotifyOfPropertyChange(() => CurrentImageScrollViewer);
      }
    }

    private System.Windows.Controls.Image _currentImageControl;  
    public System.Windows.Controls.Image CurrentImageControl
    {
      get
      {
        return _currentImageControl;
      }
      set
      {
        if (_currentImageControl != value)
          _currentImageControl = value;

        NotifyOfPropertyChange(() => CurrentImageControl);
      }
    }

    private System.Windows.Controls.Slider _sliderControl;
    public System.Windows.Controls.Slider Slider
    {
      get
      {
        return _sliderControl;
      }
      set
      {
        if (_sliderControl != value)
          _sliderControl = value;

        NotifyOfPropertyChange(() => Slider);
      }
    }
    
    private MahApps.Metro.Controls.ToggleSwitch _toggleZoomToFit;
    public MahApps.Metro.Controls.ToggleSwitch FitSwitchControll
    {
      get
      {
        return _toggleZoomToFit;
      }
      set
      {
        if (_toggleZoomToFit != value)
          _toggleZoomToFit = value;

        NotifyOfPropertyChange(() => FitSwitchControll);
      }
    }

    bool _isCheckedValueControll;
    private bool isCheckedValueControll
    {
      get
      {
        return _isCheckedValueControll;
      }
      set
      {
        if (_isCheckedValueControll != value)
          _isCheckedValueControll = value;

        NotifyOfPropertyChange(() => _isCheckedValueControll);
      }
    }
    public void ResizeOnSlider(double width, double height, int sliderValue)
    {
      int i = sliderValue;
      double d = i / 100D;

      if (UserDefaultImageIconSource != null)
      {
        double aspect_ratio = d;

        double ratio = Math.Min(_currentImage.Width, _currentImage.Height) / Math.Max(_currentImage.Width, _currentImage.Height);

        CurrWidth = aspect_ratio * width * ratio;
        CurrHeight = aspect_ratio * height * ratio;

        Scale = CurrWidth / _currentImage.Width;
      }
    }
    public void Resize(double width, double height)
    {
      bool val = FitSwitchControll.IsChecked.Value;
      //string val2 = (string)FitSwitchControll.IsEnabled.ToString();
     
      ZoomToFit(width, height);
      string val2 = isCheckedValueControll.ToString();

      if (FitSwitchControll.IsChecked.Equals(true))
      {
        ZoomToFit(width, height);
        
      }
      Console.WriteLine(val2);
/*
      Console.WriteLine(val);
      Console.WriteLine(val3);*/
      Console.WriteLine(" ");
    }

    public void UploadClick()
    {    
      
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Multiselect = false;
      openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
      openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      
      
      if (openFileDialog.ShowDialog() == true)
      {
        String filePath = openFileDialog.FileName;
        if(File.Exists(filePath))
        {
          System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile(filePath);

          _currentImage = BitmapConversion.BitmapToBitmapSource(bmp);
          NotifyOfPropertyChange(() => UserDefaultImageIconSource);
        }
      }
    }

    public void CancelClick()
    {
      _currentImage = ResourceLoader.UserDefaultImageIconSource;
      NotifyOfPropertyChange(() => UserDefaultImageIconSource);
    }

    public BitmapSource UploadIconSource
    {
      get { return ResourceLoader.UploadIconSource; }
    }
    
    public BitmapSource CancelIconSource
    {
      get { return ResourceLoader.CancelIconSource; }
    }

    public BitmapSource UserDefaultImageIconSource
    {
      get {

        if (_currentImage == null)
          _currentImage = ResourceLoader.UserDefaultImageIconSource;
        return _currentImage; 
      
      }      
    }

    public void ZoomToFit( double viewWidth, double viewHeight )
    {

      if (UserDefaultImageIconSource != null)
      {
        double aspect_ratio = 0.9;

        double ratio = Math.Min(_currentImage.Width, _currentImage.Height) / Math.Max(_currentImage.Width, _currentImage.Height);

        CurrWidth = aspect_ratio * viewWidth * ratio;
        CurrHeight = aspect_ratio * viewHeight * ratio;

        Scale = CurrWidth / _currentImage.Width;
      }      
    }    
  }
}
