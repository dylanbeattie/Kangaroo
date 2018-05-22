using System;
using Microsoft.AspNetCore.Mvc;
using Pivotal;

namespace Kangaroo.Website.Controllers {
    public class PivotalController : Controller {
        private readonly Tracker tracker;

        public PivotalController(Tracker tracker) {
            this.tracker = tracker;
        }

        public ActionResult Me() {
            return Json(tracker.Me());
        }

        public ActionResult Projects() {
            return View(tracker.Projects());
        }

        public ActionResult Stories(int id) {
            return View(tracker.Stories(id));
        }
    }
}