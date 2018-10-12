namespace AdminRole.Migrations
{
    using AdminRole.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<AdminRole.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "AdminRole.Models.ApplicationDbContext";
        }

        protected override void Seed(AdminRole.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            ApplicationUser adminUser;

            if (!context.Users.Any(p => p.UserName == "admin@mybugapp.com"))
            {
                adminUser = new ApplicationUser();
                adminUser.UserName = "admin@mybugapp.com";
                adminUser.Email = "admin@mybugapp.com";
                adminUser.FirstName = "Admin";
                adminUser.LastName = "Dhillon";
                adminUser.DisplayName = "Admin Dhillon";
                userManager.Create(adminUser, "Password-1");
            }
            else
            {
                adminUser = context.Users.Where(p => p.UserName == "admin@mybugapp.com")
                    .FirstOrDefault();
            }

            //Check if the adminUser is already on the Admin role
            //If not, add it.
            if (!userManager.IsInRole(adminUser.Id, "Admin"))
            {
                userManager.AddToRole(adminUser.Id, "Admin");
            }
            context.TicketType.AddOrUpdate(x => x.Id,
             new Models.TicketType() { Id = 1, Name = "Error Fixes" },
             new Models.TicketType() { Id = 2, Name = "Software Update" },
             new Models.TicketType() { Id = 3, Name = "Add Helpers" },
             new Models.TicketType() { Id = 4, Name = "Database Bugs" });
            context.TicketPriority.AddOrUpdate(x => x.Id,
               new Models.TicketPriority() { Id = 1, Name = "High" },
               new Models.TicketPriority() { Id = 2, Name = "Medium" },
               new Models.TicketPriority() { Id = 3, Name = "Low" },
               new Models.TicketPriority() { Id = 4, Name = "Urgent" });
            context.TicketStatus.AddOrUpdate(x => x.Id,
               new Models.TicketStatus() { Id = 1, Name = "Finished Now" },
               new Models.TicketStatus() { Id = 2, Name = "Started Now" },
               new Models.TicketStatus() { Id = 3, Name = "Not Started Yet" },
               new Models.TicketStatus() { Id = 4, Name = "Currently In progress" });
            context.SaveChanges();
        }
    }
}