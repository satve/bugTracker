﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminRole.Models
{
    public class TicketPriority
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Ticket> Ticket { get; set; }
    }
}