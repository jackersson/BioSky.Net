namespace BioData
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AccessDevice")]
    public partial class AccessDevice
    {
        [Key]
        [StringLength(50)]
        public string PortName { get; set; }

        public long? LocationID { get; set; }

        public virtual Location Location { get; set; }
    }
}
