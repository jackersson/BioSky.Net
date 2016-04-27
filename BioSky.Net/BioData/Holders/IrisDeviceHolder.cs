using BioService;
using System.Collections.Generic;

namespace BioData.Holders
{
  internal class IrisDeviceHolder
  {
    public IrisDeviceHolder(ILocationGetAble locationHolder)
    {
      _locationHolder = locationHolder;

      DataSet = new Dictionary<string, long>();
    }

    public void UpdateFromResponse(Location owner, IrisDevice responded, IrisDevice requested)
    {
      if (responded == null || responded.Dbresult != Result.Success)
        return;

      switch (responded.EntityState)
      {
        case EntityState.Added:
          if (owner.IrisDevice == null)
            owner.IrisDevice = new IrisDevice();

          if (requested != null)
            owner.IrisDevice.Devicename = requested.Devicename;

          Add(owner.Id, owner.IrisDevice);
          break;

        case EntityState.Deleted:
          Remove(owner.IrisDevice);
          owner.IrisDevice = null;
          break;
      }
    }

    private void Remove(IrisDevice device)
    {
      if (device != null)
        return;

      DataSet.Remove(device.Devicename);
    }

    public void Add(long locationId, IrisDevice device)
    {
      if (device == null)
        return;

      string deviceName = device.Devicename;
      if (!ContainesKey(deviceName))
        DataSet.Add(deviceName, locationId);
      else
      {
        Location location = _locationHolder.GetValue(DataSet[deviceName]);
        location.IrisDevice = null;
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
