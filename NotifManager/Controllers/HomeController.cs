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

        public HomeController()
        {
            _appRep = new AppRepository();
            _clientRep = new ClientRepository();
            _messageRep = new MessageRepository();
        }

        // GET: Home
        public ActionResult Index()
        {
            IndexVM ivm = new IndexVM();

            ivm.Clients = (List<Client>)_clientRep.GetData<Client>(false);
            ivm.Apps = (List<App>)_clientRep.GetData<App>(false);
            ivm.Messages = (List<Message>)_clientRep.GetData<Message>(false);

            return View(ivm);
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

            if (app.Id != Guid.Empty)
                _appRep.PostData<App>(app);

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

            if(message.Id != Guid.Empty)
                _messageRep.PostData<Message>(message);

            return View(message);
        }

        public ActionResult Client()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Client(Client client)
        {
            client.Id = Guid.NewGuid();

            _clientRep.PostData<Client>(client);

            return View(client);
        }
    }
}