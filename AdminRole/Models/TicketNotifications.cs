using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminRole.Models
{
    public class TicketNotifications
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}