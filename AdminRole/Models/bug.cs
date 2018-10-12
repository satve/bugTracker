using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminRole.Models
{
    public class Bug
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Ticket> Ticket { get; set; }

        public Bug()
        {
            Users = new HashSet<ApplicationUser>();
            Ticket = new HashSet<Ticket>();
        }
    }
}