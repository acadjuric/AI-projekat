﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ppee_dataLayer.Entities
{
    public class Load
    {
        public Load()
        {

        }

        public string Date { get; set; }

        public string FromTime { get; set; }

        public string ToTime { get; set; }

        public long MWh { get; set; }
    }
}
