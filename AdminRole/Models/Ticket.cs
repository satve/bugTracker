﻿using System;
using System.Collections.Generic;

namespace AdminRole.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }

        public int TicketTypeId { get; set; }
        public virtual TicketType TicketType { get; set; }

        public int TicketPriorityId { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }

        public int TicketStatusId { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }

        public virtual ICollection<TicketAttachments> TicketAttachments { get; set; }

        public virtual ICollection<TicketComments> TicketComments { get; set; }

        public virtual ICollection<TicketHistories> TicketHistories { get; set; }

        public virtual ICollection<TicketNotifications> TicketNotifications { get; set; }

        public string CreatorId { get; set; }
        public virtual ApplicationUser Creator { get; set; }

        public string AssigneeId { get; set; }
        public virtual ApplicationUser Assignee { get; set; }

        public int BugId { get; set; }
        public virtual Bug Bug { get; set; }

        public Ticket()
        {
            TicketAttachments = new HashSet<TicketAttachments>();
            TicketComments = new HashSet<TicketComments>();
            TicketHistories = new HashSet<TicketHistories>();
            TicketNotifications = new HashSet<TicketNotifications>();

            this.Created = DateTime.Now;
        }
    }
}