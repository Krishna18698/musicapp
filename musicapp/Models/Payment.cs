﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace musicapp.Models
{
    public partial class Payment
    {
        public int Paymentid { get; set; }
        public string Email { get; set; }
        public decimal? Paymentamt { get; set; }

        public virtual User EmailNavigation { get; set; }
    }
}