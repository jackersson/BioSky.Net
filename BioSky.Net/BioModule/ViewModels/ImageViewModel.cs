﻿using System;
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
using BioData;
using BioService;
using System.Windows.Threading;
using BioModule.Utils;
using BioContracts;

namespace BioModule.ViewModels
{
  public class ImageViewModel : Screen
  {
    public ImageViewModel()
    {    
      _bitmapUtils   = new BitmapUtils();
      _bioFileUtils  = new BioFileUtils();

      _faceFinder = new FaceFinder ();
      _marker     = new MarkerUtils();

      ZoomToFitState = true;    
    }

    #region Update
  
    public void UpdateFromImage(ref Bitmap img)
    {
      if (img == null)
        return;

      Bitmap processedFrame = DrawFaces(ref img);

      BitmapSource newFrame = BitmapConversion.BitmapToBitmapSource(processedFrame);
      newFrame.Freeze();

      CurrentImageSource = newFrame;      

      if (_width != _imageViewWidth || _height != _imageViewHeight)
      { 
        _width  = _imageViewWidth;
        _height = _imageViewHeight;
        Zoom(_imageViewWidth, _imageViewHeight);
      }
    }

    public Bitmap DrawFaces(ref Bitmap img)
    {      
      return _marker.DrawRectangles(_faceFinder.GetFaces(ref img), ref img);
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
      CurrentImageSource = bmp;
      Zoom(_imageViewWidth, _imageViewHeight);

      return bmp;      
    }  
    #endregion

    #region Interface
    public virtual void Clear()
    {    
      CurrentImageSource = null;
      Zoom(_imageViewWidth, _imageViewHeight);
    }
    
    public virtual void OnClear(double viewWidth, double viewHeight)
    {
      Clear();
      Zoom(viewWidth, viewHeight);      
    }    

    public void Zoom(double viewWidth, double viewHeight)
    {
      _imageViewWidth  = viewWidth;
      _imageViewHeight = viewHeight;
      
      double imageWidth  = CurrentImageSource.Width  == 0 ? viewWidth  : CurrentImageSource.Width;
      double imageHeight = CurrentImageSource.Height == 0 ? viewHeight : CurrentImageSource.Height;

      double maxWidthScale  = viewWidth  / imageWidth ;      
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

    private BitmapSource _currentImageSource;
    public BitmapSource CurrentImageSource
    {
      get
      {
        if (_currentImageSource == null)
          _currentImageSource = ResourceLoader.UserDefaultImageIconSource;
        return _currentImageSource;
      }
      protected set
      {
        try
        {
          if (_currentImageSource != value)
          {
            _currentImageSource = value;
            NotifyOfPropertyChange(() => CurrentImageSource);
          }
        }
        catch (TaskCanceledException ex)
        {
          Console.WriteLine(ex.Message);
        }
      }
    }  
      
    #endregion    

    #region Global Variables

    private double _imageViewWidth  = 0;
    private double _imageViewHeight = 0;
    private double _width           = 0;
    private double _height          = 0;

    private const double ZOOM_TO_FIT_RATE = 99;   

    private   BitmapUtils  _bitmapUtils  ;
    private   BioFileUtils _bioFileUtils ; 
    protected FaceFinder   _faceFinder   ;
    protected MarkerUtils  _marker       ;
    #endregion
  }
}
