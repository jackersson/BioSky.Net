using System;
using System.Collections.Generic;

using BioContracts;

using BioFaceService;
using Caliburn.Micro;

using System.Collections.ObjectModel;

using System.IO;
using BioData.Holders;

namespace BioData
{
  public class BioSkyNetRepository : PropertyChangedBase, IBioSkyNetRepository
  {
    VisitorHolder       _visitorHolder      ;
    PersonHolder        _personHolder       ;
    LocationHolder      _locationHolder     ;
    CardHolder          _cardHolder         ;
    CaptureDeviceHolder _captureDeviceHolder;
    AccessDeviceHolder  _accessDeviceHolder ;
    PhotoHolder         _photoHolder        ;

    public BioSkyNetRepository()
    {
      _visitorHolder       = new VisitorHolder      ();
      _personHolder        = new PersonHolder       ();     
      _locationHolder      = new LocationHolder     ();
      _cardHolder          = new CardHolder         ();
      _captureDeviceHolder = new CaptureDeviceHolder();
      _accessDeviceHolder  = new AccessDeviceHolder ();
      _photoHolder         = new PhotoHolder        ();

      string mediaParametr = "MEDIA_PATHWAY:";
      string mediaPath = GetConfigFile(mediaParametr);

      if (mediaPath == null)      
        LocalStoragePath = AppDomain.CurrentDomain.BaseDirectory;    
    }

    IHolder<Visitor, long> IBioSkyNetRepository.VisitorHolder
    {
      get { return _visitorHolder; }
    }

    IHolder<AccessDevice, long> IBioSkyNetRepository.AccessDeviceHolder
    {
      get { return _accessDeviceHolder; }
    }

    IHolder<Location, long> IBioSkyNetRepository.LocationHolder
    {
      get { return _locationHolder; }
    }

    IHolder<Photo, long> IBioSkyNetRepository.PhotoHolder
    {
      get { return _photoHolder; }
    }

    IHolder<Person, long> IBioSkyNetRepository.PersonHolder
    {
      get { return _personHolder; }
    }

    IHolder<CaptureDevice, long> IBioSkyNetRepository.CaptureDeviceHolder
    {
      get { return _captureDeviceHolder; }
    }

    IHolder<Card, string> IBioSkyNetRepository.CardHolder
    {
      get { return _cardHolder; }
    }

    //TODO to Utils not Repository
    public string GetConfigFile(string parametr)
    {
      string path = AppDomain.CurrentDomain.BaseDirectory + "config.txt";
      FileInfo configFile = new FileInfo(path);

      if (!configFile.Exists)
      {
        //Create a file to write to.
        using (StreamWriter streamWriter = configFile.CreateText())
        {
          streamWriter.WriteLine(parametr);
        }
      }

      using (StreamReader sr = new StreamReader(path))
      {
        bool hasParametr = false;
        string sub;
        while (!sr.EndOfStream)
        {
          var line = sr.ReadLine();
          if (!hasParametr)
          {
            if (line.StartsWith(parametr))
            {
              hasParametr = true;
              if (line.Length == parametr.Length)
              {
                Console.WriteLine("Path is not set");
                return null;
              }

              sub = line.Substring(parametr.Length, line.Length - parametr.Length);
              Console.WriteLine(sub);
              return (sub);
            }
          }
        }
      }
      return null;
    }

    private string _localStoragePath;
    public string LocalStoragePath
    {
      get { return _localStoragePath; }
      set
      {
        if (_localStoragePath != value)
        {
          _localStoragePath = value;
          NotifyOfPropertyChange(() => LocalStoragePath);
          NotifyOfPropertyChange(() => MediaFolderAddress);
          NotifyOfPropertyChange(() => PersonsFolderAddress);
        }
      }
    }

    public string MediaFolderAddress
    {
      get { return _localStoragePath + "\\media"; }      
    }

    public string PersonsFolderAddress
    {
      get { return MediaFolderAddress + "\\persons"; }    
    }

  }
}
