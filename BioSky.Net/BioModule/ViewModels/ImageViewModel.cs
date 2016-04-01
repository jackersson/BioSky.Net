using System;
using System.Collections.Generic;

using Caliburn.Micro;
using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;
using System.IO;
using System.Drawing;
using BioModule.Utils;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using BioContracts;

namespace BioModule.ViewModels
{
  public interface IImageViewUpdate
  {
    void UpdateOneImage(ref Bitmap img);

    void UpdateTwoImage(ref Bitmap img1, ref Bitmap img2);

    void SetOneImage(BitmapSource img);

    void SetTwoImages(BitmapSource img, BitmapSource img2);
  }

  public class ImageViewModel : Screen, IImageViewUpdate
  {
    public ImageViewModel(IProcessorLocator locator)
    {    
      _bitmapUtils   = new BitmapUtils();
      _bioFileUtils  = new BioFileUtils();     

      ZoomToFitState = true;      

      BitmapSources = new AsyncObservableCollection<BitmapSource>();

      ImageDispathcer = Dispatcher.CurrentDispatcher;

      _notifier = locator.GetProcessor<INotifier>();
    }

    private AsyncObservableCollection<BitmapSource> _bitmapSources;
    public AsyncObservableCollection<BitmapSource> BitmapSources
    {
      get { return _bitmapSources; }
      set
      {
        if (_bitmapSources != value)
        {
          _bitmapSources = value;
          NotifyOfPropertyChange(() => BitmapSources);
        }
      }
    }

    #region Update

    public void SetOneImage(BitmapSource img)
    {
      if(img != null)
      {
        BitmapSources.Clear();
        BitmapSources.Add(img);
        Zoom(_imageViewWidth, _imageViewHeight, _imageControllWidth, _imageControllHeight);
      }
    }

    public void SetTwoImages(BitmapSource img, BitmapSource img2)
    {
      if (img != null || img2 != null)
      {
        BitmapSources.Clear();
        BitmapSources.Add(img);
        BitmapSources.Add(img2);
        Zoom(_imageViewWidth, _imageViewHeight, _imageControllWidth, _imageControllHeight);
      }
    }

    public void UpdateOneImage(ref Bitmap img)
    {
      if (img == null)
        return;

      BitmapSource newFrame = BitmapConversion.BitmapToBitmapSource(img);
      newFrame.Freeze();

      if(BitmapSources.Count != 1)
      {
        BitmapSources.Clear();
        BitmapSources.Add(newFrame);
      }

      BitmapSources[0] = newFrame;

      if (_width != _imageViewWidth || _height != _imageViewHeight)
      {
        _width = _imageViewWidth;
        _height = _imageViewHeight;
        Zoom(_imageViewWidth, _imageViewHeight, _imageControllWidth, _imageControllHeight);
      }
    }

    public void UpdateTwoImage(ref Bitmap img1, ref Bitmap img2)
    {
      if (img1 != null)
      {
        BitmapSource newFrame = BitmapConversion.BitmapToBitmapSource(img1);
        newFrame.Freeze();

        if (BitmapSources.Count < 2)
          BitmapSources.Add(newFrame);

        BitmapSources[0] = newFrame;
      }

      if(img2 != null)
      {
        BitmapSource newFrame = BitmapConversion.BitmapToBitmapSource(img2);
        newFrame.Freeze();

        if (BitmapSources.Count < 2)
          BitmapSources.Add(newFrame);
        if(BitmapSources.Count == 2)
          BitmapSources[1] = newFrame;
        else
          BitmapSources[0] = newFrame;
      }

      if (_width != _imageViewWidth || _height != _imageViewHeight)
      {
        _width = _imageViewWidth;
        _height = _imageViewHeight;
        Zoom(_imageViewWidth, _imageViewHeight, _imageControllWidth, _imageControllHeight);
      }
    }

    public string Upload()
    {
      var dialog = _bioFileUtils.OpenFileDialog();
      if (dialog.ShowDialog() == true)
      {
        string filename = dialog.FileName;             
        UpdateFromFile(filename);
        return filename;        
      }
      return null;
    }

    public BitmapImage UpdateFromFile(string fileName = "")
    {
      if (!File.Exists(fileName))
      {
        Clear();
        return null;
      }
       
      BitmapImage bmp    = _bitmapUtils.GetImageSource(fileName);

      BitmapSources.Clear();
      BitmapSources.Add(bmp);

      Zoom(_imageViewWidth, _imageViewHeight, _imageControllWidth, _imageControllHeight);

      return bmp;      
    }  
    #endregion

    #region Interface
    public virtual void Clear()
    {
      BitmapSources.Clear();
      Zoom(_imageViewWidth, _imageViewHeight, _imageControllWidth, _imageControllHeight);
    }
    
    public virtual void OnClear(double viewWidth, double viewHeight, double imageControlWidth, double imageControlHeight)
    {
      Clear();
      Zoom(viewWidth, viewHeight, _imageControllWidth, _imageControllHeight);      
    }  

    public void Zoom(double viewWidth, double viewHeight, double itemsViewWidth, double itemsViewHeight)
    {
      if (BitmapSources.Count == 0)
        return;

      if (viewWidth == 0 && viewHeight == 0)
        return;

      _imageViewWidth  = viewWidth;
      _imageViewHeight = viewHeight;

      _imageControllWidth = itemsViewWidth;
      _imageControllHeight = itemsViewHeight;

      double width  = itemsViewWidth;
      double height = itemsViewHeight;

      try
      {
        //if (width == 0 && height == 0)
        //{
          if (BitmapSources.Count == 1)
          {
            width = BitmapSources[0].Width;
            height = BitmapSources[0].Height;
          }
          else if (BitmapSources.Count == 2)
          {
            width = BitmapSources[0].Width + BitmapSources[1].Width;
            height = Math.Max(BitmapSources[0].Height, BitmapSources[1].Height);
          }
        //}
      }
      catch(Exception ex)
      {
        _notifier.Notify(ex);
      }

      double imageWidth  = width  == 0 ? viewWidth : width;
      double imageHeight = height == 0 ? viewHeight : height;

      double maxWidthScale  = viewWidth / imageWidth;
      double maxHeightScale = viewHeight / imageHeight;

      CalculatedImageScale = Math.Min(maxHeightScale, maxWidthScale) * ZoomRate / 100;
    }
    #endregion

    #region UI

    private double _calculatedImageScale;
    public double CalculatedImageScale
    {
      get { return _calculatedImageScale; }
      set
      {
        if (_calculatedImageScale != value)
        {
          _calculatedImageScale = value;
          NotifyOfPropertyChange(() => CalculatedImageScale);
        }
      }
    }

    private Dispatcher _imageDispathcer;
    public Dispatcher ImageDispathcer
    {
      get { return _imageDispathcer; }
      set
      {
        if (_imageDispathcer != value)
        {
          _imageDispathcer = value;
          NotifyOfPropertyChange(() => ImageDispathcer);
        }
      }
    }


    private double _zoomRate;
    public double ZoomRate
    {
      get { return _zoomRate; }
      set
      {
        if (_zoomRate != value)
        {
          _zoomRate = value;

          ZoomToFitState = (_zoomRate == ZOOM_TO_FIT_RATE);
          NotifyOfPropertyChange(() => ZoomRate);
        }
      }
    }

    private bool _zoomToFitState;
    public bool ZoomToFitState
    {
      get { return _zoomToFitState; }
      set
      {
        if (_zoomToFitState != value)
        {
          _zoomToFitState = value;
          ZoomRate = _zoomToFitState ? ZOOM_TO_FIT_RATE : ZoomRate;
          NotifyOfPropertyChange(() => ZoomToFitState);
        }
      }
    }
    
    #endregion

    #region Global Variables

    private double _imageViewWidth      = 0;
    private double _imageViewHeight     = 0;
    private double _imageControllWidth  = 0;
    private double _imageControllHeight = 0;
    private double _width               = 0;
    private double _height              = 0;

    private const double ZOOM_TO_FIT_RATE = 99;   

    private   BitmapUtils  _bitmapUtils  ;
    private   BioFileUtils _bioFileUtils ; 
    protected FaceFinder   _faceFinder   ;
    protected MarkerUtils  _marker       ;
    private INotifier _notifier;
    #endregion
  }
}
