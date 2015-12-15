using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioData;
using System.Data.Entity.Validation;
using System.IO.Ports;
using System.Threading;

using BioContracts;
using BioAccessDevice;
using BioAccessDevice.Commands;
using BioAccessDevice.Abstract;

namespace BioConsole
{
  public class AccessDeviceObserver : IObserver<AccessDeviceActivity>
  {
    public void OnCompleted()
    {
      Console.WriteLine("Additional activity data will not be transmitted.");
    }

    public void OnError(Exception error)
    {
      Console.WriteLine(error.Message);
    }

    public void OnNext(AccessDeviceActivity value)
    {
      Console.WriteLine("The Command is {0}", value.CommandID);

      if (value.CommandID == AccessDeviceCommands.CommandDallasKey)
      {
        if ( value.Data != null )
        {
          for (int i = 0; i < value.Data.Length; ++i)
          {
            Console.Write(value.Data[i] + " ");
          }
        }
        Console.WriteLine();
      }
    }

    public IDisposable Subscribe(IObserver<AccessDeviceListener> observer)
    { 
      throw new NotImplementedException();
    }
    public virtual void Unsubscribe()
    {
      unsubscriber.Dispose();
    }

    public virtual void Subscribe(IObservable<AccessDeviceActivity> provider)
    {
      unsubscriber = provider.Subscribe(this);
    }

    private IDisposable unsubscriber;
  }

  class Program
  {
    public static string buildConnectionString( string database_path )
    {
      string metadata = "metadata=res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl;";
      metadata = "metadata=res://*/BioSkyNetDataModel.csdl|res://*/BioSkyNetDataModel.ssdl|res://*/BioSkyNetDataModel.msl;";

      string provider = "provider=System.Data.SqlClient;";
      string datasource = @"data source=(LocalDB)\MSSQLLocalDB;";

      string attachDbFileName = "attachdbfilename=" + database_path + ";";

      string integratedSecurity       = "integrated security=True;";
      string multipleActiveResultSets = "MultipleActiveResultSets=True;";
      string app = "App=EntityFramework';";

      string providerConnectionString = "provider connection string=';"
                                      + datasource
                                      + attachDbFileName
                                      + integratedSecurity
                                      + multipleActiveResultSets
                                      + app;

      string connection_string = metadata + provider + providerConnectionString;

      return connection_string;
    }


    private static void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
    {
      SerialPort sp = (SerialPort)sender;
      //byte[] response = new byte[7];

      int i = 0;
      while ( i < sp.BytesToRead)
      {
        int el = sp.ReadByte();
        Console.Write(el);
        i++;
      }
        

      Console.WriteLine("Data Received:");
      

      //byte[] response = GetBytes(indata);
      // byte[] response = Encoding.UTF8.GetBytes(indata);

      ///for ( int i = 0; i < response.Length; ++i)
      //{
      // Console.Write(response[i] + " ");
      // }
      Console.WriteLine("");
    }

    static byte[] GetBytes(string str)
    {
      byte[] bytes = new byte[str.Length * sizeof(char)];
      System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
      return bytes;
    }



    static void Main(string[] args)
    {      
      string comPortName = "COM5";     

      AccessDevicesEngine accessDeviceEngine = new AccessDevicesEngine();
      

      accessDeviceEngine.Add(comPortName);

      AccessDeviceObserver obs = new AccessDeviceObserver();
      accessDeviceEngine.Subscribe(obs, comPortName);


      accessDeviceEngine.Execute(AccessDeviceCommands.CommandReady, comPortName);

      Thread.Sleep(1000);

      accessDeviceEngine.Execute(AccessDeviceCommands.CommandReset, comPortName);

      Thread.Sleep(1000);

      accessDeviceEngine.Execute(AccessDeviceCommands.CommandAccess, comPortName);

      Thread.Sleep(1000);

      accessDeviceEngine.Execute(AccessDeviceCommands.CommandReset, comPortName);

      Thread.Sleep(10000);

      //accessDeviceEngine.Remove(comPortName);

      Console.ReadLine();
      
      
     /* 
      SerialPort sp = new SerialPort();
      sp.PortName = "COM5";
      sp.BaudRate = 4800;
      

      //sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
   
      Thread.Sleep(50);

      try
      {
        sp.Open();
        if ( sp.IsOpen )
        {
          //byte[] command = new byte[] { 129, 0, 0, 8, 1 };
          //byte[] command = new byte[] { 130, 0, 0, 0, 0, 8, 2 };
          byte[] command = new byte[] { 130, 0, 0, 8, 0, 8, 10 };
          sp.Write(command, 0, command.Length);

          Thread.Sleep(50);
        
          byte[] response = new byte[command.Length];
          sp.Read(response, 0, command.Length);

          for (int i = 0; i < response.Length; ++i)
          {
            Console.Write(response[i] + " ");
          }
        }

      }
      catch ( Exception ex )
      {
        Console.Write(ex.Message);
      }

      Console.ReadLine();

      */
      
















      // add name = "bioskynet_dataEntities" 
      //connectionString = "metadata=res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=(LocalDB)\MSSQLLocalDB;attachdbfilename=F:\C#\BioSkyNetSuccess\BioSky.Net\BioSky.Net\BioData\bioskynet_data.mdf;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;" 
      // providerName = "System.Data.EntityClient" />


      //string connection_string2 = @"metadata=res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl;provider=System.Data.SqlClient;provider connection string=';data source=(LocalDB)\MSSQLLocalDB;attachdbfilename=F:\C#\BioSkyNetSuccess\BioSky.Net\BioSky.Net\BioData\bioskynet_data.mdf;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework';";
      // System.Console.WriteLine(connection_string2);


      //string dbPath = @"F:\C#\BioSkyNetSuccess\BioSky.Net\BioSky.Net\database\BioSkyNet.mdf";

      //string connectionString3 = buildConnectionString(dbPath);
      //System.Console.WriteLine(connectionString3);

      /*
      using (BioSkyNetEntities db = new BioSkyNetEntities(connectionString3))
      {

        try
        {
          DateTime date = new DateTime(2015, 12, 4);
          // создаем два объекта User
          User user1 = new User { UID = 1, First_Name_ = "Tom", Last_Name_ = "Henks", Gender = "Male", Rights="Operator" };
          //Users user2 = new Users { Id = 2, FirstName = "Taras", LastName = "Lishchenko" };

          // добавляем их в бд



          //db.Entry(user1).State = EntityState.Added;
          db.Users.Add(user1);

          db.SaveChanges();
          //Console.WriteLine("Объекты успешно сохранены");

          // получаем объекты из бд и выводим на консоль
          var users = db.Users;
          Console.WriteLine("Список объектов:");
          foreach (User u in users)
          {
            Console.WriteLine("{0}.{1} - {2}", u.UID, u.First_Name_, u.Last_Name_);
          }
          
        }
        catch (DbEntityValidationException e)
        {
          foreach (var eve in e.EntityValidationErrors)
          {
            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                eve.Entry.Entity.GetType().Name, eve.Entry.State);
            foreach (var ve in eve.ValidationErrors)
            {
              Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                  ve.PropertyName, ve.ErrorMessage);
            }
          }
          throw;
        }

        Console.ReadLine();

      }
      */
    }
  }
}
