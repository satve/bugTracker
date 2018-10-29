using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using AdminRole.helper;
using AdminRole.Models;
using Microsoft.AspNet.Identity;

namespace AdminRole.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db { get; set; }
        private UserRoleHelper UserRoleHelper { get; set; }
        public TicketsController()
        {
            db = new ApplicationDbContext();
            UserRoleHelper = new UserRoleHelper();
        }

        // GET: Tickets
        public ActionResult Index(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                return View(db.Ticket.Include(t => t.TicketPriority).Include(t => t.Bug).Include(t => t.TicketStatus).Include(t => t.TicketType).Where(p => p.CreatorId == User.Identity.GetUserId()).ToList());
            }
            return View(db.Ticket.Include(t => t.TicketPriority).Include(t => t.Bug).Include(t => t.TicketStatus).Include(t => t.TicketType).ToList());
        }
        public ActionResult UserTickets()
        {
            string userID = User.Identity.GetUserId();
            if (User.IsInRole("Submitter"))
            {
                var tickets = db.Ticket.Where(t => t.CreatorId == userID).Include(t => t.Creator).Include(t => t.Assignee).Include(t => t.Bug);
                return View("Index", tickets.ToList());
            }
            if (User.IsInRole("Developer"))
            {
                var tickets = db.Ticket.Where(t => t.AssigneeId == userID).Include(t => t.Creator).Include(t => t.TicketComments).Include(t => t.Assignee).Include(t => t.Bug);
                return View("Index", tickets.ToList());
            }
            if (User.IsInRole("Project Manager"))
            {
                return View(db.Ticket.Include(t => t.TicketPriority).Include(t => t.TicketComments).Include(t => t.Bug).Include(t => t.TicketStatus).Include(t => t.TicketType).Where(p => p.AssigneeId == userID).ToList());
            }
            return View("Index");
        }

        [Authorize(Roles = "Developer,Project Manager")]
        public ActionResult ProjectManagerOrDeveloperTickets()
        {
            string userId = User.Identity.GetUserId();
            var ProjectMangerOrDeveloperId = db.Users.Where(p => p.Id == userId).FirstOrDefault();
            var projectsIds = ProjectMangerOrDeveloperId.Bugs.Select(p => p.Id).ToList();
            var tickets1 = db.Ticket.Where(p => projectsIds.Contains(p.BugId)).ToList();
            return View("Index", tickets1);
        }

        public ActionResult AssignDev(int ticketId)
        {
            var model = new AssignDevTicketModel();
            var ticket = db.Ticket.FirstOrDefault(p => p.Id == ticketId);
            var userRoleHelper = new UserRoleHelper();
            var users = userRoleHelper.UsersInRole("Developer");
            model.TicketId = ticketId;
            model.DeveloperList = new SelectList(users, "Id", "Name");
            return View(model);
        }

        [HttpPost]
        public ActionResult AssignDev(AssignDevTicketModel model)
        {
            var ticket = db.Ticket.FirstOrDefault(p => p.Id == model.TicketId);
            ticket.AssigneeId = model.SelectedDeveloperId;
            db.SaveChanges();
            var user = db.Users.FirstOrDefault(p => p.Id == model.SelectedDeveloperId);
            var personalEmailService = new PersonalEmailService();
            var mailMessage = new MailMessage(
            WebConfigurationManager.AppSettings["emailto"], user.Email
                   );
            mailMessage.Body = "New Assignee";
            mailMessage.Subject = "Assignee to Developer";
            mailMessage.IsBodyHtml = true;
            personalEmailService.Send(mailMessage);
            return RedirectToAction("Index");
        }
        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Ticket.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        //GET: Tickets/TicketsComments
        [HttpPost]
        public ActionResult CreateComment(int id, string body)
        {
            var ticket = db.Ticket
               .Where(p => p.Id == id)
               .FirstOrDefault();
            if (ticket == null)
            {
                return HttpNotFound();
            }
            if (string.IsNullOrWhiteSpace(body))
            {
                ViewBag.ErrorMessage = "Must require a comment..!!";
                return View("Details", new { ticket.Id });
            }
            var comment = new TicketComments();
            comment.UserId = User.Identity.GetUserId();
            comment.TicketId = ticket.Id;
            comment.Created = DateTime.Now;
            comment.Comment = body;
            db.TicketComments.Add(comment);
            var user = db.Users.FirstOrDefault(p => p.Id == comment.UserId);
            var personalEmailService = new PersonalEmailService();
            var mailMessage = new MailMessage(
            WebConfigurationManager.AppSettings["emailto"], user.Email
                   );
            mailMessage.Body = "New comment added";
            mailMessage.Subject = "New Comment";
            mailMessage.IsBodyHtml = true;
            personalEmailService.Send(mailMessage);
            db.SaveChanges();
            return RedirectToAction("Details", new { id });
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create()
        {
            ViewBag.AssigneeId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.BugId = new SelectList(db.Bugs, "Id", "Name");
            ViewBag.CreatorId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Submitter")]
        public ActionResult Create([Bind(Include = "Id,Name,Description,TicketTypeId,TicketPriorityId,BugId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                if (ticket == null)
                {
                    return HttpNotFound();
                }
                ticket.TicketStatusId = 1;
                ticket.CreatorId = User.Identity.GetUserId();
                db.Ticket.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BugId = new SelectList(db.Bugs, "Id", "Name", ticket.BugId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAttachment(int ticketId, [Bind(Include = "Id,Description,TicketTypeId")] TicketAttachments ticketAttachments, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image == null)
                {
                    return HttpNotFound();
                }

                if (!ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    ViewBag.ErrorMessage = "upload image here";

                }
                var fileName = Path.GetFileName(image.FileName);
                image.SaveAs(Path.Combine(Server.MapPath("~/ImgUploads/"), fileName));
                ticketAttachments.FilePath = "/ImgUploads/" + fileName;
                ticketAttachments.UserId = User.Identity.GetUserId();
                ticketAttachments.Created = DateTime.Now;
                ticketAttachments.UserId = User.Identity.GetUserId();
                ticketAttachments.TicketId = ticketId;
                db.TicketAttachments.Add(ticketAttachments);
                var user = db.Users.FirstOrDefault(p => p.Id == ticketAttachments.UserId);
                var personalEmailService = new PersonalEmailService();
                var mailMessage = new MailMessage(
                WebConfigurationManager.AppSettings["emailto"], user.Email
                       );
                mailMessage.Body = "New Attachment";
                mailMessage.Subject = "Add Attachment";
                mailMessage.IsBodyHtml = true;
                personalEmailService.Send(mailMessage);
                db.SaveChanges();
                return RedirectToAction("Details", new {id = ticketId });
            }
            return View(ticketAttachments);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Ticket.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssigneeId = new SelectList(db.Users, "Id", "FirstName", ticket.AssigneeId);
            ViewBag.BugId = new SelectList(db.Bugs, "Id", "Name", ticket.BugId);
            ViewBag.CreatorId = new SelectList(db.Users, "Id", "FirstName", ticket.CreatorId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,TicketTypeId,TicketPriorityId,TicketStatusId,CreatorId,AssigneeId,BugId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                var dateChanged = DateTimeOffset.Now;
                var changes = new List<TicketHistories>();

                // db.Entry(ticket).State = EntityState.Modified;
                var DbTicket = db.Ticket.FirstOrDefault(p => p.Id == ticket.Id);
                DbTicket.Name = ticket.Name;
                DbTicket.Description = ticket.Description;
                DbTicket.TicketTypeId = ticket.TicketTypeId;
                DbTicket.Updated = dateChanged;

                var originalValues = db.Entry(DbTicket).OriginalValues;
                var currentValues = db.Entry(DbTicket).CurrentValues;
                foreach (var property in originalValues.PropertyNames)
                {
                    var originalValue = originalValues[property]?.ToString();
                    var currentValue = currentValues[property]?.ToString();
                    if (originalValue != currentValue)
                    {
                        var history = new TicketHistories();
                        history.Changed = dateChanged;
                        history.NewValue = GetValueFromKey(property, currentValue);
                        history.OldValue = GetValueFromKey(property, originalValue);
                        history.Property = property;
                        history.TicketId = DbTicket.Id;
                        history.UserId = User.Identity.GetUserId();
                        changes.Add(history);
                    }
                }
                db.TicketHistories.AddRange(changes);
                db.SaveChanges();
                if (DbTicket.AssigneeId != null)
                {
                    var user = db.Users.FirstOrDefault(p => p.Id == DbTicket.AssigneeId);
                    var personalEmailService = new PersonalEmailService();
                    var mailMessage = new MailMessage(
                    WebConfigurationManager.AppSettings["emailto"], user.Email
                           );
                    mailMessage.Body = "Edit Ticket by Developer";
                    mailMessage.Subject = "Edit Tickets";
                    mailMessage.IsBodyHtml = true;
                    personalEmailService.Send(mailMessage);
                }
                return RedirectToAction("Index");
            }
            ViewBag.AssigneeId = new SelectList(db.Users, "Id", "FirstName", ticket.AssigneeId);
            ViewBag.BugId = new SelectList(db.Bugs, "Id", "Name", ticket.BugId);
            ViewBag.CreatorId = new SelectList(db.Users, "Id", "FirstName", ticket.CreatorId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        private string GetValueFromKey(string propertyName, string key)
        {
            if (propertyName == "TicketTypeId")
            {
                return db.TicketType.Find(Convert.ToInt32(key)).Name;
            }

            if (propertyName == "TicketStatusId")
            {
                return db.TicketStatus.Find(Convert.ToInt32(key)).Name;
            }

            if (propertyName == "TicketPriorityId")
            {
                return db.TicketPriority.Find(Convert.ToInt32(key)).Name;
            }

            if (propertyName == "TicketBugId")
            {
                return db.Bugs.Find(Convert.ToInt32(key)).Name;
            }
            return key;
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Ticket.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Ticket.Find(id);
            db.Ticket.Remove(ticket);
            db.SaveChanges();
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
