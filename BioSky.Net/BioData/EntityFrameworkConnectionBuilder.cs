using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioContracts;
using System.Data.Entity.Core.EntityClient;

namespace BioData
{
  public class EntityFrameworkConnectionBuilder : IEntityFrameworkConnectionBuilder
  {   
    public string _dbConnectionstring;

    public EntityFrameworkConnectionBuilder( string dbConnectionstring )
    {     
      _dbConnectionstring =  dbConnectionstring;
    }
   //  <add name = "BioSkyNetDataModel" 
   //   connectionString="data source=(LocalDB)\MSSQLLocalDB;attachdbfilename=F:\C#\BioSkyNetSuccess\BioSky.Net\BioSky.Net\database\BioSkyNet.mdf;integrated security=True;connect timeout=30;MultipleActiveResultSets=True;App=EntityFramework" 
   //   providerName="System.Data.SqlClient" />
    public string createEntityFrameworkConnection()
    {    
      string datasource = @"data source=(LocalDB)\MSSQLLocalDB;";

      string attachDbFileName = "attachdbfilename=" + _dbConnectionstring + ";";

      string integratedSecurity = "integrated security=True;";
      string multipleActiveResultSets = "MultipleActiveResultSets=True;";
      string app = "App=EntityFramework";

      string providerConnectionString = 
                                        datasource
                                      + attachDbFileName
                                      + integratedSecurity
                                      + multipleActiveResultSets
                                      + app;

      string connection_string = providerConnectionString;

      return connection_string;
    }



  }
}
