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


  public class ImageViewModel : PropertyChangedBase
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
      //_currentImageControl = new System.Windows.Controls.Image();
     // _sliderControl = new System.Windows.Controls.Slider();
      //_toggleZoomToFit = new MahApps.Metro.Controls.ToggleSwitch();

      IsCheckedValueControll = true;
     
    }

    private BitmapSource _currentImage;

    /*
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
    */

    /*
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
      */
    /*
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
    */
    private bool _isCheckedValueControll;
    public bool IsCheckedValueControll
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
      
      if (IsCheckedValueControll == false)
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
    }
    public void Resize(double width, double height, int sliderValue)
    {

      if (IsCheckedValueControll == true)
      {
        ZoomToFit(width, height);
      }
      else
      {
        ResizeOnSlider(width, height, sliderValue);
      }
         
    }

    public void OnZoomToFit(double width, double height)
    {
      if (IsCheckedValueControll == true)
      {
        ZoomToFit(width, height);
      }
    }

    public void UploadClick(double viewWidth, double viewHeight)
    {    
      
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Multiselect = false;
      openFileDialog.Filter = "All Images (*.jpeg; *.jpg; *.png; *.gif; *.bmp)|*.txt; *.jpeg; *.jpg; *.png; *.gif; *.bmp";
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

      ZoomToFit(viewWidth, viewHeight);
    }

    public void CancelClick(double viewWidth, double viewHeight)
    {
      _currentImage = ResourceLoader.UserDefaultImageIconSource;
      NotifyOfPropertyChange(() => UserDefaultImageIconSource);

      ZoomToFit(viewWidth, viewHeight);

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
   
    //***********************************************************Icon Source************************************************

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
      get
      {

        if (_currentImage == null)
          _currentImage = ResourceLoader.UserDefaultImageIconSource;
        return _currentImage;

      }
    }




  }
}
