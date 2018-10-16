using System;

namespace AdminRole.Models
{
    public class TicketAttachments
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string FilePath { get; set; }

        public DateTimeOffset Created { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public int TicketId { get; set; }

        public virtual Ticket Ticket { get; set; }
     }
}