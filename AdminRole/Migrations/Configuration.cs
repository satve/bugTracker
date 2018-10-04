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
                roleManager.Create(new IdentityRole { Name = "Project Moderator" });
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
        }
    }
}
