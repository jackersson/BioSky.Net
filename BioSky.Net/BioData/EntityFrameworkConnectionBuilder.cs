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

    public string createEntityFrameworkConnection(string dataModelName)
    {
      string metadata = string.Format(@"res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl", dataModelName);
      metadata = "metadata=res://*/BioSkyNetDataModel.csdl|res://*/BioSkyNetDataModel.ssdl|res://*/BioSkyNetDataModel.msl;";
      string provider = "provider=System.Data.SqlClient;";
      string datasource = @"data source=(LocalDB)\MSSQLLocalDB;";

      string attachDbFileName = "attachdbfilename=" + _dbConnectionstring + ";";

      string integratedSecurity = "integrated security=True;";
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



  }
}
