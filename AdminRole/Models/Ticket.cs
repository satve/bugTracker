﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        public int CreatorId { get; set; }
        public virtual ApplicationUser Creator { get; set; }

        public int AssigneeId { get; set; }
        public virtual ApplicationUser Assignee { get; set; }

        public string BugId { get; set; }
        public virtual Bug Bug { get; set; }

        public ICollection<ApplicationUser> Users { get; set; }

        public Ticket()
        {
            Users = new HashSet<ApplicationUser>();
        }
    }
}