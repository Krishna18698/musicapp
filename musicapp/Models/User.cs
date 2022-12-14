// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace musicapp.Models
{
    public partial class User
    {
        public User()
        {
            BankAccounts = new HashSet<BankAccount>();
            Feedback1s = new HashSet<Feedback1>();
            Payments = new HashSet<Payment>();
            Votings = new HashSet<Voting>();
        }

        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string MusicPlan { get; set; }
        public DateTime? Dob { get; set; }
        public string Role { get; set; }
        public string Roles { get; set; }

        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<Feedback1> Feedback1s { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Voting> Votings { get; set; }
    }
}