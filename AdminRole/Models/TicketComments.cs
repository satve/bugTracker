using System;

namespace AdminRole.Models
{
    public class TicketComments
    {
        public int Id { get; set; }

        public string Comment { get; set; }

        public DateTimeOffset Created { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}