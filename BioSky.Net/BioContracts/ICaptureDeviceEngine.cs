using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public delegate void FrameEventHandler(object sender, ref Bitmap bitmap);
  public interface ICaptureDeviceEngine
  {

    void Stop();

    void Add(string cameraName);


    void Remove(string cameraName);


    bool CaptureDeviceActive(string cameraName);



    void Subscribe(FrameEventHandler eventListener, string cameraName);

    void Unsubscribe(FrameEventHandler eventListener, string cameraName);

    void ShowCaptureDevicePropertyPage(string cameraName, IntPtr parentWindow);

    VideoCapabilities[] GetCaptureDeviceVideoCapabilities(string cameraName);

    void ShowCaptureDeviceConfigurationPage(string cameraName, IPropertiesShowable propertiesShowable);

    AsyncObservableCollection<string> GetCaptureDevicesNames();
  }
}
