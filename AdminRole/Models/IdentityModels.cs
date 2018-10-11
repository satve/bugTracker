using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using AdminRole.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections;

namespace AdminRole.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public ApplicationUser()
        {
            Bugs = new HashSet<Bug>();
            CreatedTickets = new HashSet<Ticket>();
            AssignedTickets = new HashSet<Ticket>();
        }
        public virtual ICollection<Bug> Bugs { get; set; }
        [InverseProperty("Creator")]
        public virtual ICollection<Ticket> CreatedTickets { get; set; }
        [InverseProperty("Assignee")]
        public virtual ICollection<Ticket> AssignedTickets { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Models.Bug> Bugs { get; set; }
        public DbSet<Models.Ticket> Ticket { get; set; }
        public DbSet<Models.TicketStatus> TicketStatus { get; set; }
        public DbSet<Models.TicketType> TicketType { get; set; }
        public DbSet<Models.TicketPriority> TicketPriority { get; set; }
        public DbSet<Models.TicketComments> TicketComments { get; set; }
        public DbSet<Models.TicketAttachments> TicketAttachments { get; set; }
    }
}