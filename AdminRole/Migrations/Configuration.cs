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

            ApplicationUser adminUser = null;
            ApplicationUser DeveloperUser = null;
            ApplicationUser ProjectManagerUser = null;
            ApplicationUser SubmitterUser = null;

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

            if (!userManager.IsInRole(adminUser.Id, "Admin"))
            {
                userManager.AddToRole(adminUser.Id, "Admin");
            }

            if (!context.Users.Any(p => p.UserName == "manager@mybugapp.com"))
            {
                ProjectManagerUser = new ApplicationUser();
                ProjectManagerUser.UserName = "manager@mybugapp.com";
                ProjectManagerUser.Email = "manager@mybugapp.com";
                ProjectManagerUser.FirstName = "Manager";
                ProjectManagerUser.LastName = "Dhillon";
                ProjectManagerUser.DisplayName = "Manager Dhillon";
                userManager.Create(ProjectManagerUser, "Password-2");
            }
            else
            {
                ProjectManagerUser = context.Users.Where(p => p.UserName == "manager@mybugapp.com")
                    .FirstOrDefault();
            }

            if (!userManager.IsInRole(ProjectManagerUser.Id, "Project Manager"))
            {
                userManager.AddToRole(ProjectManagerUser.Id, "Project Manager");
            }

            if (!context.Users.Any(p => p.UserName == "developer@mybugapp.com"))
            {
                DeveloperUser = new ApplicationUser();
                DeveloperUser.UserName = "developer@mybugapp.com";
                DeveloperUser.Email = "developer@mybugapp.com";
                DeveloperUser.FirstName = "Developer";
                DeveloperUser.LastName = "Dhillon";
                DeveloperUser.DisplayName = "Developer Dhillon";
                userManager.Create(DeveloperUser, "Password-3");
            }
            else
            {
                DeveloperUser = context.Users.Where(p => p.UserName == "developer@mybugapp.com")
                    .FirstOrDefault();
            }

            if (!userManager.IsInRole(DeveloperUser.Id, "Developer"))
            {
                userManager.AddToRole(DeveloperUser.Id, "Developer");
            }

            if (!context.Users.Any(p => p.UserName == "submitter@mybugapp.com"))
            {
                SubmitterUser = new ApplicationUser();
                SubmitterUser.UserName = "submitter@mybugapp.com";
                SubmitterUser.Email = "submitter@mybugapp.com";
                SubmitterUser.FirstName = "Submitter";
                SubmitterUser.LastName = "Dhillon";
                SubmitterUser.DisplayName = "Submitter Dhillon";
                userManager.Create(SubmitterUser, "Password-4");
            }
            else
            {
                SubmitterUser = context.Users.Where(p => p.UserName == "submitter@mybugapp.com")
                    .FirstOrDefault();
            }

            if (!userManager.IsInRole(SubmitterUser.Id, "Submitter"))
            {
                userManager.AddToRole(SubmitterUser.Id, "Submitter");
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