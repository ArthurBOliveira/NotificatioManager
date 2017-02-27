using NotifManager.Models;
using NotifManager.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NotifManager.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult App()
        {
            return View();
        }

        [HttpPost]
        public ActionResult App(App app)
        {
            app.SubDomain = app.Name.Replace(" ", "").ToLower();

            app = OneSignalAPI.PostApp(app);

            app = OneSignalAPI.PutApp(app);

            return View(app);
        }

        public ActionResult Message()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Message(Message message)
        {
            message = OneSignalAPI.PostMessage(message);

            return View(message);
        }
    }
}