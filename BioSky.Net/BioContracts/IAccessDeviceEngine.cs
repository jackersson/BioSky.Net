namespace BioContracts
{
  public interface IAccessDeviceEngine
  {   
    void Add(string deviceName);

    void Remove(string deviceName);
    
    AsyncObservableCollection<string> GetDevicesNames();

    bool IsDeviceActive(string deviceName);

    bool HasObserver(IAccessDeviceObserver observer, string deviceName);

    void Execute(AccessDeviceCommands command, string deviceName);

    void Subscribe(IAccessDeviceObserver observer, string deviceName);

    void Unsubscribe(IAccessDeviceObserver observer);
    void Stop();
  }
}
