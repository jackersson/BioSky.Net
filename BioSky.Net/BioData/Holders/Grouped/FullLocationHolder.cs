﻿using BioContracts;
using BioService;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Linq;
using System;
using BioContracts.Holders;

namespace BioData.Holders.Grouped
{
  public class FullLocationHolder : PropertyChangedBase, IFullLocationHolder
  {
    public FullLocationHolder(IProcessorLocator locator)
    {
      DataSet = new Dictionary<long, Location>();     
      Data = new AsyncObservableCollection<Location>();

      AccessDevices  = new List<AccessDevice>();
      CaptureDevices = new List<CaptureDevice>();

      _dialogsHolder = locator.GetProcessor<IDialogsHolder>();
    }

    public void Init(Google.Protobuf.Collections.RepeatedField<Location> data)
    {
      try
      {
        Data = new AsyncObservableCollection<Location>(data);

        foreach (Location location in data)
        {
          _dataSet.Add(location.Id, location);
          AccessDevices.Add(location.AccessDevice);
          CaptureDevices.Add(location.CaptureDevice);
        }

        OnDataChanged();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
    }

    public void Add(Location requested, Location responded)
    {
      if (responded.Dbresult == Result.Success && !_dataSet.ContainsKey(responded.Id))
      {
        requested.Id         = responded.Id        ;
        requested.AccessType = responded.AccessType;

        AddAccessDevice (requested, responded.AccessDevice);
        AddCaptureDevice(requested, responded.CaptureDevice);

        Data.Add(requested);
        _dataSet.Add(requested.Id, requested);        
      }
      OnDataChanged();
      ShowLocationResult(requested, responded);
    }

    public void Update( Location requested
                      , Location responded)
    {
      if (responded.Dbresult == Result.Success)
      {
        Location oldItem = GetValue(requested.Id);
        Console.WriteLine(Data);
        if (oldItem != null)
        {
          if(requested.CaptureDevice != null)
          {
            oldItem.CaptureDevice = (oldItem.CaptureDevice == null)
                        ? new CaptureDevice() 
                        : oldItem.CaptureDevice;
            oldItem.CaptureDevice.MergeFrom(requested.CaptureDevice);
          }
          
          if(requested.AccessDevice != null)
          {
            oldItem.AccessDevice = (oldItem.AccessDevice == null)
                       ? new AccessDevice()
                       : oldItem.AccessDevice;
            oldItem.AccessDevice.MergeFrom(requested.AccessDevice);
          }

          CopyFrom(responded, oldItem);
        }

        Console.WriteLine(Data);
        Console.WriteLine(DataSet);
      }


      OnDataChanged();
      ShowLocationResult(requested, responded);
    }

    private void CopyFrom(Location from, Location to)
    {
      if (from.LocationName != "")
        to.LocationName = from.LocationName;

      if (from.Description != "")
        to.Description = from.Description;

      if(from.EntityState == EntityState.Modified)
          to.AccessType = from.AccessType;

      if (from.AccessDevice != null)
        AddAccessDevice(to, from.AccessDevice);

      if (from.CaptureDevice != null)       
        AddCaptureDevice(to, from.CaptureDevice);

      to.Persons.Clear();
      to.Persons.Add(from.Persons);
    }

    public void Remove( Location requested
                      , Location responded )
    {
      if (responded != null)
      {
        if (responded.Dbresult == Result.Success)
        {
          _dataSet.Remove(requested.Id);
          var item = Data.Where(x => x.Id == requested.Id).FirstOrDefault();
          if (item != null)
            Data.Remove(item);
        }
      }
      OnDataChanged();
      ShowLocationResult(requested, responded);
    }

    private void ShowLocationResult(Location requested, Location responded)
    {
      LocationItems.Clear();

      if (responded == null)
        responded = new Location() { Id = requested.Id, Dbresult = Result.Failed };

      TreeItem locationItem = new TreeItem()
      {
          Name = string.Format("Location: {0} ({1})", responded.LocationName, responded.Id)
        , IsSuccess = (responded.Dbresult == Result.Success) ? true : false
      };

      if (responded.AccessDevice != null)
      {
        AccessDevice accessDevice = responded.AccessDevice;
        locationItem.Members.Add(new TreeItem
        {
            Name = string.Format("Access Device: {0} ({1})", accessDevice.Portname, accessDevice.Id)
          , IsSuccess = (accessDevice.Dbresult == Result.Success) ? true : false
        });
      }

      if (responded.CaptureDevice != null)
      {
        CaptureDevice captureDevice = responded.CaptureDevice;
        locationItem.Members.Add(new TreeItem
        {
            Name = string.Format("Capture Device: {0} ({1})", captureDevice.Devicename, captureDevice.Id)
          , IsSuccess = (captureDevice.Dbresult == Result.Success) ? true : false
        });
      }

      if (responded.Persons != null)
      {
        if(responded.Persons.Count > 0)
        {
          TreeItem personsItem = new TreeItem() { Name = "Persons", IsSuccess = true };

          foreach (Person person in responded.Persons)
          {
            personsItem.Members.Add(new TreeItem
            {
                Name = string.Format("Person: {0} {1} ({2})", person.Firstname, person.Lastname, person.Id)
              , IsSuccess = (person.Dbresult == Result.Success) ? true : false
            });
          }

          locationItem.Members.Add(personsItem);
        }
      }

      LocationItems.Add(locationItem);

      _dialogsHolder.NotificationDialog.Update(LocationItems, "LocationNotificationDialog");
      _dialogsHolder.NotificationDialog.Show();
    }

    public Location GetValue(long Id)
    {
      Location location = null;
      DataSet.TryGetValue(Id, out location);

      return location;
    }

    public Location GetValue(Location location)
    {
      if (location == null)
        return null;

      Location response = Data.Where(x => x.LocationName == location.LocationName && x.Description == location.Description).FirstOrDefault();
      return response;
    }

    public bool HasUserAccess(long locationID, long userID)
    {

      return true;
    }

    private void OnDataChanged()
    {
      if (DataChanged != null)
        DataChanged();
    }

    private void OnDataUpdated(Google.Protobuf.Collections.RepeatedField<Location> list)
    {
      if (DataUpdated != null)
        DataUpdated(list);
    }

    private void AddAccessDevice (Location owner, AccessDevice responded )
    {
      if (responded == null)
        return;

      owner.AccessDevice.Id         = responded.Id;
      owner.AccessDevice.Locationid = owner.Id    ;      
    }

    private void AddCaptureDevice(Location owner, CaptureDevice responded)
    {
      if (responded == null)
        return;
            
      owner.CaptureDevice.Id         = responded.Id;
      owner.CaptureDevice.Locationid = owner.Id    ;  
    }

    private AsyncObservableCollection<Location> _data;
    public AsyncObservableCollection<Location> Data
    {
      get { return _data; }
      private set
      {
        if (_data != value)
        {
          _data = value;
          NotifyOfPropertyChange(() => Data);
        }
      }
    }

    private List<AccessDevice> _accessDevices;
    public List<AccessDevice> AccessDevices
    {
      get { return _accessDevices; }
      private set
      {
        if (_accessDevices != value)
          _accessDevices = value;
      }
    }

    private List<CaptureDevice> _captureDevices;
    public List<CaptureDevice> CaptureDevices
    {
      get { return _captureDevices; }
      private set
      {
        if (_captureDevices != value)
          _captureDevices = value;
      }
    }

    private Dictionary<long, Location> _dataSet;
    public Dictionary<long, Location> DataSet
    {
      get { return _dataSet; }
      private set
      {
        if (_dataSet != value)
          _dataSet = value;
      }
    }

    private List<TreeItem> _locationItems;
    public List<TreeItem> LocationItems
    {
      get
      {
        return (_locationItems == null) ? _locationItems = new List<TreeItem>()
                                     : _locationItems;
      }
      set
      {
        if (_locationItems != value)
          _locationItems = value;
      }
    }

    private Dictionary<long, HashSet<long>> _allowedUsersDataSet;
    public Dictionary<long, HashSet<long>> AllowedUsersDataSet
    {
      get { return _allowedUsersDataSet; }
      private set
      {
        if (_allowedUsersDataSet != value)
          _allowedUsersDataSet = value;
      }
    }

    private IDialogsHolder _dialogsHolder;

    public event DataChangedHandler DataChanged;
    public event DataUpdatedHandler<Google.Protobuf.Collections.RepeatedField<Location>> DataUpdated;    
  }
}
