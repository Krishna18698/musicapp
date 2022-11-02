﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace musicapp.Models
{
    public partial class Music
    {
        public Music()
        {
            Votings = new HashSet<Voting>();
        }

        public int? Genreid { get; set; }
        public string Genrename { get; set; }
        public int MusicId { get; set; }
        public string MusicName { get; set; }
        public string MusicPath { get; set; }
        [NotMapped]
        public IFormFile musicpath { get; set; }
        public string ImgPath { get; set; }
        [NotMapped]
        public IFormFile imgpath { get; set; }
        public virtual Genre GenrenameNavigation { get; set; }
        public virtual ICollection<Voting> Votings { get; set; }
    }
}