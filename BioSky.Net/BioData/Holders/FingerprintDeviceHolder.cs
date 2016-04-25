using BioService;
using System.Collections.Generic;

namespace BioData.Holders
{
  internal class FingerprintDeviceHolder
  {
    public FingerprintDeviceHolder(ILocationGetAble locationHolder)
    {
      _locationHolder = locationHolder;

      DataSet = new Dictionary<string, long>();
    }

    public void UpdateFromResponse(Location owner, FingerprintDevice responded, FingerprintDevice requested)
    {
      if (responded == null || responded.Dbresult != Result.Success)
        return;

      switch (responded.EntityState)
      {
        case EntityState.Added:
          if (owner.FingerprintDevice == null)
            owner.FingerprintDevice = new FingerprintDevice();

          if (requested != null)
            owner.FingerprintDevice.Devicename = requested.Devicename;

          Add(owner.Id, owner.FingerprintDevice);
          break;

        case EntityState.Deleted:
          Remove(owner.FingerprintDevice);
          owner.FingerprintDevice = null;
          break;
      }
    }

    private void Remove(FingerprintDevice device)
    {
      if (device != null)
        return;

      DataSet.Remove(device.Devicename);
    }

    public void Add(long locationId, FingerprintDevice device)
    {
      if (device == null)
        return;

      string deviceName = device.Devicename;
      if (!ContainesKey(deviceName))
        DataSet.Add(deviceName, locationId);
      else
      {
        Location location = _locationHolder.GetValue(DataSet[deviceName]);
        location.FingerprintDevice = null;
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
