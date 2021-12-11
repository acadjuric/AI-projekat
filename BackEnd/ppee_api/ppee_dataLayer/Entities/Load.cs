using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ppee_dataLayer.Entities
{
    public class Load
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string Date { get; set; }

        public string FromTime { get; set; }

        public string ToTime { get; set; }

        public long MWh  { get; set; }
    }
}
