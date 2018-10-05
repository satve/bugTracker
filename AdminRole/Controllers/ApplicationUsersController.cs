using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AdminRole.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace AdminRole.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ApplicationUsers
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: ApplicationUsers/Details/5
        public ActionResult ChangeRole(string id)
        {
            var model = new UserRoleViewModel();

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            var user = userManager.FindById(id);
            model.Id = id;
            model.Name = User.Identity.Name;
            var roles = roleManager.Roles.ToList();
            var userRoles = userManager.GetRoles(id);
            model.Roles = new MultiSelectList(roles, "Name", "Name", userRoles);

            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeRole(UserRoleViewModel model)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            var user = userManager.FindById(model.Id);
            var userRoles = userManager.GetRoles(user.Id);
            foreach (var role in userRoles)
            {
                userManager.RemoveFromRole(user.Id, role);
            }
            foreach (var role in model.SelectedRoles)
            {
                userManager.AddToRole(user.Id, role);
            }
            var signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            signInManager.SignIn(user, isPersistent: false, rememberBrowser: false);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}