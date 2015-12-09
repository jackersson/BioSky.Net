using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioData;
using System.Data.Entity.Validation;

namespace BioConsole
{
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


    static void Main(string[] args)
    {
      // add name = "bioskynet_dataEntities" 
      //connectionString = "metadata=res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=(LocalDB)\MSSQLLocalDB;attachdbfilename=F:\C#\BioSkyNetSuccess\BioSky.Net\BioSky.Net\BioData\bioskynet_data.mdf;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;" 
      // providerName = "System.Data.EntityClient" />
      

      string connection_string2 = @"metadata=res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl;provider=System.Data.SqlClient;provider connection string=';data source=(LocalDB)\MSSQLLocalDB;attachdbfilename=F:\C#\BioSkyNetSuccess\BioSky.Net\BioSky.Net\BioData\bioskynet_data.mdf;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework';";
      System.Console.WriteLine(connection_string2);


      string dbPath = @"F:\C#\BioSkyNetSuccess\BioSky.Net\BioSky.Net\database\BioSkyNet.mdf";

      string connectionString3 = buildConnectionString(dbPath);
      System.Console.WriteLine(connectionString3);

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
