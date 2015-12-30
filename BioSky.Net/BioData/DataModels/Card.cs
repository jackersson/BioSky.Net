namespace BioData
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Card")]
    public partial class Card
    {
        [StringLength(50)]
        public string CardID { get; set; }

        public long? UserID { get; set; }

        public virtual User User { get; set; }
    }
}
