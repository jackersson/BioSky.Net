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
    void SetSingleImage(BitmapSource img);
    void SetDoubleImage(BitmapSource first, BitmapSource second);
  }

  public class ImageViewModel : Screen, IImageViewUpdate
  {
    public ImageViewModel()
    {    
      _bitmapUtils   = new BitmapUtils();
      _bioFileUtils  = new BioFileUtils();     
      
      ImageItems = new AsyncObservableCollection<ImageItemViewModel>();

      ZoomToFitState = true;
    }  

    #region Update

    public void SetSingleImage(BitmapSource img)
    {
      if (ImageItems.Count > 1)      
        ImageItems.Clear();
      
      ShowImage(img, 0);      
    }
    public void SetDoubleImage(BitmapSource first, BitmapSource second)
    {
      ShowImage(first , 0);
      ShowImage(second, 1);
    }

    private void ShowImage(BitmapSource img, int index )
    {
      if (ImageItems.Count <= index)
      {
        ImageItems.Add(new ImageItemViewModel());
        NotifyOfPropertyChange(() => ImageItems);
      }

      ImageItems[index].UpdateImageSource(img);
      ImageItems[index].Zoom(_scrollFieldWidth, _scrollFieldHeight);
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

      SetSingleImage(bmp);

      return bmp;      
    }  
    #endregion

    #region Interface
    public virtual void Clear()
    {
      ImageItems.Clear();
      Zoom(_scrollFieldWidth, _scrollFieldHeight);
    }
    
    /*
    public virtual void OnClear(double viewWidth, double viewHeight)
    {
      Clear();
      Zoom(viewWidth, viewHeight);      
    }  
    */
    public void Zoom(double viewWidth, double viewHeight)
    {

      if (viewWidth == _scrollFieldWidth && _scrollFieldHeight == viewHeight)
        return;
      
      _scrollFieldWidth  = viewWidth;
      _scrollFieldHeight = viewHeight;

      int itemsCount  = ImageItems.Count;
      double zoomRate = ZoomRate / 100;
      foreach (ImageItemViewModel imageItem in ImageItems)      
        imageItem.Zoom(viewWidth * zoomRate / itemsCount, viewHeight * zoomRate / itemsCount);      

    }
    #endregion

    #region UI
    /*
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
    */
    private AsyncObservableCollection<ImageItemViewModel> _imageItems;
    public AsyncObservableCollection<ImageItemViewModel> ImageItems
    {
      get { return _imageItems; }
      set
      {
        if (_imageItems != value)
        {
          _imageItems = value;
          NotifyOfPropertyChange(() => ImageItems);
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

    private double _scrollFieldWidth      = 0 ;
    private double _scrollFieldHeight     = 0 ;
        
    private const double ZOOM_TO_FIT_RATE = 99;   

    private   BitmapUtils  _bitmapUtils  ;
    private   BioFileUtils _bioFileUtils ; 

   // private INotifier _notifier;
    #endregion
  }
}
