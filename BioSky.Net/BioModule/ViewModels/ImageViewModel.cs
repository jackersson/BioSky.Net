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
    public ImageViewModel( )
    {
      
      _bitmapUtils   = new BitmapUtils();
      _bioFileUtils  = new BioFileUtils();     
      
      ImageItems = new AsyncObservableCollection<ImageItemViewModel>();

      ZoomToFitState = true;
    }  

    #region Update

    public virtual void SetSingleImage(BitmapSource img)
    {
      if (ImageItems.Count > 1)      
        ImageItems.Clear();
      
      ShowImage(img, 0);      
    }
    public virtual void SetDoubleImage(BitmapSource first, BitmapSource second)
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

      if (index < ImageItems.Count)
      {
        ImageItemViewModel item = ImageItems[index];
        item.UpdateImageSource(img);
        item.Zoom(_scrollFieldWidth, _scrollFieldHeight);
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
    
    public void Zoom(double viewWidth, double viewHeight)
    {      
      if (viewWidth == _scrollFieldWidth && _scrollFieldHeight == viewHeight)
        return;

      _scrollFieldWidth  = viewWidth;
      _scrollFieldHeight = viewHeight;

      Zoom(ZoomRate);
    }

    public void Zoom(double zoomRate)
    {   
      double zR = zoomRate / 100 / ImageItems.Count;
      foreach (ImageItemViewModel imageItem in ImageItems)
        imageItem.Zoom(_scrollFieldWidth * zR, _scrollFieldHeight * zR);
    }
    #endregion

    #region UI
  
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
          Zoom(_zoomRate);
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
    #endregion
  }
}
