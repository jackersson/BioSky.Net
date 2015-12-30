namespace BioData
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

  public enum VisitorStatus
  {
      VerificationSuccess
    , VerificationFailed
  }

    [Table("Visitor")]
    public partial class Visitor
    {
        [Key]
        public long UID { get; set; }

        [Column("User UID")]
        public long? User_UID { get; set; }

        [Column("Photo ID")]
        public long? Photo_ID { get; set; }

        [Column("Full Photo ID")]
        public long? Full_Photo_ID { get; set; }

        [Column("Detection Time")]
        public DateTime? Detection_Time { get; set; }

        [Column("Locaion ID")]
        public long? Locaion_ID { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public virtual Location Location { get; set; }

        public virtual User User { get; set; }
    }
}
