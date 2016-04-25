using BioService;
using System.Collections.Generic;

namespace BioData.Holders
{
  public interface ILocationGetAble
  {
    Location GetValue(long id);
  }
  internal class AccessDeviceHolder
  {
    public AccessDeviceHolder(ILocationGetAble locationHolder)
    {
      _locationHolder = locationHolder;

      DataSet = new Dictionary<string, long>();
    }

    public void UpdateFromResponse(Location owner, AccessDevice responded, AccessDevice requested)
    {
      if (responded == null || responded.Dbresult != Result.Success)
        return;

      switch (responded.EntityState)
      {
        case EntityState.Added:
          if (owner.AccessDevice == null)
            owner.AccessDevice = new AccessDevice();

          if (requested != null)
            owner.AccessDevice.Portname = requested.Portname;

          Add(owner.Id, owner.AccessDevice);
          break;

        case EntityState.Deleted:
          Remove(owner.AccessDevice);
          owner.AccessDevice = null;          
          break;
      }
    }

    private void Remove(AccessDevice device) {
      if (device == null)
        return;

      DataSet.Remove(device.Portname);
    }

    public void Add(long locationId, AccessDevice device)
    {
      if (device == null)
        return;

      string deviceName = device.Portname;
      if (!ContainesKey(deviceName))
        DataSet.Add(deviceName, locationId);
      else
      {
        Location location = _locationHolder.GetValue(DataSet[deviceName]);
        location.AccessDevice = null;
        DataSet[deviceName] = locationId;
      }  
    }

    private bool ContainesKey(string key)
    {
      long result;
      return DataSet.TryGetValue(key, out result);
    }

    private Dictionary<string, long> _dataSet;
    public Dictionary<string, long> DataSet
    {
      get { return _dataSet; }
      private set
      {
        if (_dataSet != value)
          _dataSet = value;
      }
    }

    private readonly ILocationGetAble _locationHolder;
  }
  
}
