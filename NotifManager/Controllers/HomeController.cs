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
            IndexVM ivm = new IndexVM();

            ivm.Clients = (List<Client>)_clientRep.GetData<Client>(false);
            ivm.Apps = (List<App>)_clientRep.GetData<App>(false);
            ivm.Messages = (List<Message>)_clientRep.GetData<Message>(false);

            return View(ivm);
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

            if (clientLogged != null)
            {
                if (Hash.ValidatePassword(c.Password, clientLogged.Password))
                    _session.CurrentClient = clientLogged;
            }

            return View();
        }

        [Authorize]
        public ActionResult App()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult App(App app)
        {
            app.SubDomain = app.Name.Replace(" ", "").ToLower();

            app = OneSignalAPI.PostApp(app);

            if (app.Id != Guid.Empty)
                _appRep.PostData<App>(app);

            return View(app);
        }

        [Authorize]
        public ActionResult Message()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Message(Message message)
        {
            message = OneSignalAPI.PostMessage(message);

            if (message.Id != Guid.Empty)
                _messageRep.PostData<Message>(message);

            return View(message);
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
    }
}