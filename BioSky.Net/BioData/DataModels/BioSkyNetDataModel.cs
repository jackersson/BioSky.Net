namespace BioData
{
  using System;
  using System.Data.Entity;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Linq;
  using BioContracts;

  public partial class BioSkyNetDataModel : DbContext
  {
    public BioSkyNetDataModel(IEntityFrameworkConnectionBuilder entityFrameworkConnectionBuilder )
                            : base(entityFrameworkConnectionBuilder.createEntityFrameworkConnection())
    {
    }

    public virtual DbSet<AccessDevice> AccessDevices { get; set; }
    public virtual DbSet<Card> Cards { get; set; }
   
    public virtual DbSet<Location> Locations { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Visitor> Visitors { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Location>()
          .Property(e => e.Location_Name)
          .IsUnicode(false);

      modelBuilder.Entity<Location>()
          .Property(e => e.User_Notification)
          .IsUnicode(false);

      modelBuilder.Entity<Location>()
          .HasMany(e => e.Visitor)
          .WithOptional(e => e.Location)
          .HasForeignKey(e => e.Locaion_ID);

      modelBuilder.Entity<User>()
          .Property(e => e.First_Name_)
          .IsUnicode(false);

      modelBuilder.Entity<User>()
          .Property(e => e.Last_Name_)
          .IsUnicode(false);

      modelBuilder.Entity<User>()
          .Property(e => e.Gender)
          .IsFixedLength()
          .IsUnicode(false);

      modelBuilder.Entity<User>()
          .Property(e => e.Country)
          .IsUnicode(false);

      modelBuilder.Entity<User>()
          .Property(e => e.City)
          .IsUnicode(false);

      modelBuilder.Entity<User>()
          .Property(e => e.Photo)
          .IsUnicode(false);

      modelBuilder.Entity<User>()
          .Property(e => e.Comments)
          .IsUnicode(false);

      modelBuilder.Entity<User>()
          .Property(e => e.Rights)
          .IsUnicode(false);

      modelBuilder.Entity<User>()
          .HasMany(e => e.Card)
          .WithOptional(e => e.User)
          .HasForeignKey(e => e.UserID);

      /*
      modelBuilder.Entity<User>()
          .HasMany(e => e.Email)
          .WithOptional(e => e.User)
          .HasForeignKey(e => e.UserID);
          */
      modelBuilder.Entity<User>()
          .HasMany(e => e.Visitor)
          .WithOptional(e => e.User)
          .HasForeignKey(e => e.User_UID);
    }
  }
}
