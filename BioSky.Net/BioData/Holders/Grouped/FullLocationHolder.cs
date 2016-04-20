using BioContracts;
using BioService;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Linq;
using System;
using BioContracts.Holders;
using BioContracts.CaptureDevices;
using BioContracts.AccessDevices;

namespace BioData.Holders.Grouped
{
  public class FullLocationHolder : PropertyChangedBase, IFullLocationHolder
  {
    public FullLocationHolder(IProcessorLocator locator)
    {
      DataSet = new Dictionary<long, Location>();     
      Data = new AsyncObservableCollection<Location>();

      AccessDevicesSet  = new HashSet<string>();
      CaptureDevicesSet = new HashSet<string>();

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
          if(location.AccessDevice != null)
            AccessDevicesSet.Add(location.AccessDevice.Portname);
          if (location.CaptureDevice != null)
            CaptureDevicesSet.Add(location.CaptureDevice.Devicename);
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

        CheckDevices(requested);

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
          CheckDevices(requested);

          if (requested.CaptureDevice != null)
          {
            if (requested.CaptureDevice.EntityState == EntityState.Deleted)
            {
              CaptureDevicesSet.Remove(requested.CaptureDevice.Devicename);
              oldItem.CaptureDevice = new CaptureDevice();
            }
            else
            {
              oldItem.CaptureDevice = (oldItem.CaptureDevice == null)
                                      ? new CaptureDevice()
                                      : oldItem.CaptureDevice;
              oldItem.CaptureDevice.MergeFrom(requested.CaptureDevice);
            }
          }          
          if(requested.AccessDevice != null)
          {
            if (requested.AccessDevice.EntityState == EntityState.Deleted)
            {
              AccessDevicesSet.Remove(requested.AccessDevice.Portname);
              oldItem.AccessDevice = new AccessDevice();
            }
            else
            {
              oldItem.AccessDevice = (oldItem.AccessDevice == null)
                                     ? new AccessDevice()
                                     : oldItem.AccessDevice;
              oldItem.AccessDevice.MergeFrom(requested.AccessDevice);
            }
          }
          CopyFrom(responded, oldItem);
        }

        Console.WriteLine(Data);
        Console.WriteLine(DataSet);
      }
      OnDataChanged();
      ShowLocationResult(requested, responded);
    }

    public void Remove(Location requested
                      , Location responded)
    {
      if (responded != null)
      {
        if (responded.Dbresult == Result.Success)
        {
          _dataSet.Remove(requested.Id);
          var item = Data.Where(x => x.Id == requested.Id).FirstOrDefault();
          if (item != null)
          {
            if (item.AccessDevice != null && item.AccessDevice.Locationid > 0)
              AccessDevicesSet.Remove(item.AccessDevice.Portname);
            if (item.CaptureDevice != null && item.CaptureDevice.Locationid > 0)
              CaptureDevicesSet.Remove(item.CaptureDevice.Devicename);

            Data.Remove(item);
          }
        }
      }
      OnDataChanged();
      ShowLocationResult(requested, responded);
    }

    private void CheckDevices(Location requested)
    {      
      if (requested.AccessDevice == null && requested.CaptureDevice == null)
        return;

      foreach (Location location in Data)
      {
        AccessDevice accessDevice = location.AccessDevice;
        if (requested.AccessDevice != null && requested.AccessDevice.EntityState != EntityState.Deleted)
        {
          if (requested.AccessDevice != null && accessDevice != null)
            if (accessDevice.Portname == requested.AccessDevice.Portname)
              location.AccessDevice = new AccessDevice();
        }

        CaptureDevice captureDevice = location.CaptureDevice;
        if (requested.CaptureDevice != null && requested.CaptureDevice.EntityState != EntityState.Deleted)
        {
          if (requested.CaptureDevice != null && captureDevice != null)
            if (captureDevice.Devicename == requested.CaptureDevice.Devicename)
              location.CaptureDevice = new CaptureDevice();
        }
      }
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
        string state = requested.AccessDevice.EntityState.ToString();
        locationItem.Members.Add(new TreeItem
        {
            Name = string.Format("Access Device: {0} ({1}) {2}", accessDevice.Portname, accessDevice.Id, state)
          , IsSuccess = (accessDevice.Dbresult == Result.Success) ? true : false
        });
      }

      if (responded.CaptureDevice != null)
      {
        CaptureDevice captureDevice = responded.CaptureDevice;
        string state = requested.CaptureDevice.EntityState.ToString();
        locationItem.Members.Add(new TreeItem
        {
            Name = string.Format("Capture Device: {0} ({1}) {2}", captureDevice.Devicename, captureDevice.Id, state)
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

      if (owner.AccessDevice != null 
          && owner.AccessDevice.Locationid > 0 
          && !string.IsNullOrEmpty(owner.AccessDevice.Portname)
          && !AccessDevicesSet.Contains(owner.AccessDevice.Portname))
        AccessDevicesSet.Add(owner.AccessDevice.Portname);
    }

    private void AddCaptureDevice(Location owner, CaptureDevice responded)
    {
      if (responded == null)
        return;
            
      owner.CaptureDevice.Id         = responded.Id;
      owner.CaptureDevice.Locationid = owner.Id    ;

      if (owner.CaptureDevice != null 
          && owner.CaptureDevice.Locationid > 0 
          && !string.IsNullOrEmpty(owner.CaptureDevice.Devicename)
          && !CaptureDevicesSet.Contains(owner.CaptureDevice.Devicename))
        CaptureDevicesSet.Add(owner.CaptureDevice.Devicename);
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

    private HashSet<string> _accessDevicesSet;
    public HashSet<string> AccessDevicesSet
    {
      get { return _accessDevicesSet; }
      private set
      {
        if (_accessDevicesSet != value)
          _accessDevicesSet = value;
      }
    }

    private HashSet<string> _captureDevicesSet;
    public HashSet<string> CaptureDevicesSet
    {
      get { return _captureDevicesSet; }
      private set
      {
        if (_captureDevicesSet != value)
          _captureDevicesSet = value;
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

    private IDialogsHolder       _dialogsHolder      ;
    private ICaptureDeviceEngine _captureDeviceEngine;
    private IAccessDeviceEngine  _accessDeviceEngine ;

    public event DataChangedHandler DataChanged;
    public event DataUpdatedHandler<Google.Protobuf.Collections.RepeatedField<Location>> DataUpdated;    
  }
}
