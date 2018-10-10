using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminRole.Models
{
    public class TicketType
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public ICollection<Ticket> Ticket { get; set; }
    }
}