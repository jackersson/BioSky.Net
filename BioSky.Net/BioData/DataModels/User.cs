namespace BioData
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

  public enum Gender
  {
      Male
    , Female
  }

  public enum Rights
  {
      Operator
    , Manager
    , Supervisor
  }

  [Table("User")]
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            Card = new HashSet<Card>();
            //Email = new HashSet<Email>();
            Visitor = new HashSet<Visitor>();
        }

        [Key]
        public long UID { get; set; }

        [Column("First Name ")]
        [Required]
        [StringLength(50)]
        public string First_Name_ { get; set; }

        [Column("Last Name ")]
        [Required]
        [StringLength(50)]
        public string Last_Name_ { get; set; }

        [Column("Date Of Birth", TypeName = "smalldatetime")]
        public DateTime? Date_Of_Birth { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; }

        [StringLength(50)]
        public string Country { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        public string Photo { get; set; }

        public Guid? Thumbnail { get; set; }

        [StringLength(50)]
        public string Comments { get; set; }

        [Required]
        [StringLength(50)]
        public string Rights { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Card> Card { get; set; }
          
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Visitor> Visitor { get; set; }
    }
}
