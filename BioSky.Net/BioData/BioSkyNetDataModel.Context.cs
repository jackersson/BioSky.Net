
using BioContracts;

namespace BioData
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class BioSkyNetEntities : DbContext
    {
  
      public BioSkyNetEntities( IEntityFrameworkConnectionBuilder entityFrameworkConnectionBuilder
                              , string dataModelName = "BioSkyNetDataModel"  ) 
                              : base(entityFrameworkConnectionBuilder.createEntityFrameworkConnection(dataModelName) )
      {
       
      }
    
      protected override void OnModelCreating(DbModelBuilder modelBuilder)
      {
          throw new UnintentionalCodeFirstException();
      }

    //TODO remove after all
      public static string buildConnectionString(string database_path)
      {
        string metadata = "metadata=res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl;";
        metadata = "metadata=res://*/BioSkyNetDataModel.csdl|res://*/BioSkyNetDataModel.ssdl|res://*/BioSkyNetDataModel.msl;";
      
        string provider = "provider=System.Data.SqlClient;";
        string datasource = @"data source=(LocalDB)\MSSQLLocalDB;";
      
        string attachDbFileName = "attachdbfilename=" + database_path + ";";
      
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

      public virtual DbSet<Location> Locations { get; set; }
      public virtual DbSet<User> Users { get; set; }
      public virtual DbSet<Visitor> Visitors { get; set; }
    }
}
