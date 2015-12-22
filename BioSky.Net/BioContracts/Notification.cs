using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public enum NotificationStatus
   {
      Success
    , Warning
    , Failure
   }

   public class Notification
   {
     public string InfoMessage { get; set; }
     public string AdditionalVisualInfo { get; set; }   
     public string Detection_Time { get; set; }
     public string LocationName { get; set; }
     public string Status { get; set; }

   }
  
}
