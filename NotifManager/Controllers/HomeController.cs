using NotifManager.Models;
using NotifManager.Repositories;
using NotifManager.Utility;
using NotifManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NotifManager.Controllers
{
    public class HomeController : Controller
    {
        private AppRepository _appRep;
        private ClientRepository _clientRep;
        private MessageRepository _messageRep;

        private SessionHelper _session;

        public HomeController()
        {
            _appRep = new AppRepository();
            _clientRep = new ClientRepository();
            _messageRep = new MessageRepository();
            _session = new SessionHelper(System.Web.HttpContext.Current.Session);
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            if (_session.CurrentClient.Id != Guid.Empty)
            {
                IndexVM ivm = new IndexVM();

                ivm.Apps = (List<App>)_appRep.GetAppsByClient(_session.CurrentClient.Id);

                return View(ivm);
            }
            else
                return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(Client c)
        {
            Client clientLogged = _clientRep.GetClientByEmail(c.Email);

            if (clientLogged != null && Hash.ValidatePassword(c.Password, clientLogged.Password))
            {
                _session.CurrentClient = clientLogged;               

                return View("Index", CurrentIndex(clientLogged.Id));
            }
            else
            {
                return View();
            }
        }

        [AuthorizationFilter]
        public ActionResult Logout()
        {
            _session.CurrentClient = new Client();

            return View("Index");
        }

        [AuthorizationFilter]
        public ActionResult App()
        {
            return View();
        }

        [HttpPost]
        [AuthorizationFilter]
        public ActionResult App(App app)
        {
            if (ModelState.IsValid)
            {
                if (_session.CurrentClient.Id != Guid.Empty)
                {
                    app.SubDomain = app.Name.Replace(" ", "").ToLower();

                    app.ClientId = _session.CurrentClient.Id;

                    app = OneSignalAPI.PostApp(app);

                    if (app.Id != Guid.Empty)
                        _appRep.PostData<App>(app);

                    return View("Index", CurrentIndex(app.ClientId));
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        [AuthorizationFilter]
        public ActionResult Message()
        {
            return PartialView();
        }

        [HttpPost]
        [AuthorizationFilter]
        public ActionResult Message(Message message)
        {
            if (ModelState.IsValid)
            {
                App app = _appRep.GetData<App>(message.AppId);
                if (app.ClientId == _session.CurrentClient.Id)
                {
                    message.RestKey = app.RestKey;

                    message = OneSignalAPI.PostMessage(message);

                    if (message.Id != Guid.Empty)
                    {
                        _messageRep.PostData<Message>(message);
                        return Json(message);
                    }
                    else
                    {
                        return Json("Error");
                    }
                }
                else
                {
                    return Json("Error");
                }
                
            }
            else
            {
                return Json("Error");
            }
            
        }

        [AllowAnonymous]
        public ActionResult Client()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Client(Client client)
        {
            if (ModelState.IsValid)
            {
                client.Id = Guid.NewGuid();

                client.Password = Hash.CreateHash(client.Password);

                _clientRep.PostData<Client>(client);

                return View(client);
            }
            else
                return View();
        }

        [NonAction]
        private IndexVM CurrentIndex(Guid id)
        {
            IndexVM ivm = new IndexVM();

            ivm.Apps = (List<App>)_appRep.GetAppsByClient(id);

            return ivm;
        }
    }
}