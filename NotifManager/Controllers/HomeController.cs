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
                IndexVM ivm = CurrentIndex(_session.CurrentClient.Id);

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
                    if (_session.CurrentClient.Type == "Premium" || ((List<App>)_appRep.GetAppsByClient(_session.CurrentClient.Id)).Count() < 1)
                    {
                        Random rg = new Random();

                        app.Url = app.Url[app.Url.Length] == '/' ? app.Url.Substring(0, app.Url.Length - 1) : app.Url;

                        app.IsHttps = app.Url.Contains("https");

                        if (!app.IsHttps)
                        {
                            app.SubDomain = app.Name.Replace(" ", "").ToLower();
                            app.SubDomain = app.SubDomain + rg.Next().ToString();
                        }

                        app.ClientId = _session.CurrentClient.Id;

                        app = OneSignalAPI.PostApp(app);

                        if (app.Id != Guid.Empty)
                            _appRep.PostData<App>(app);

                        return View("Index", CurrentIndex(app.ClientId));
                    }
                    else
                        return View();
                }
                else
                    return View();
            }
            else
            {
                return View();
            }
        }

        //[HttpPost]
        [AuthorizationFilter]
        public JsonResult DeleteApp(Guid id)
        {
            if (_session.CurrentClient.Id != Guid.Empty)
            {
                App app =_appRep.GetData<App>(id);

                if (app.ClientId == _session.CurrentClient.Id)
                {
                    _appRep.DeleteData<App>(id);
                    
                    return Json("Sucesso!");
                }
                else
                    return Json("Você não tem Permissão para Excluir esse Aplicativo!");
            }
            else
                return Json("Erro!");
        }

        //[AuthorizationFilter]
        //public ActionResult Message()
        //{
        //    List<MessageVM> mvm = new List<MessageVM>();
        //    List<Guid> appsId = new List<Guid>();

        //    IEnumerable<App> apps = _appRep.GetAppsByClient(_session.CurrentClient.Id);

        //    foreach (App a in apps)
        //    {
        //        appsId.Add(a.Id);
        //    }

        //    IEnumerable<Message> messages = _messageRep.GetMessagesByApp((IEnumerable<Guid>)appsId);

        //    List<App> appsAux = (List<App>)apps;

        //    foreach (Message m in messages)
        //    {
        //        App appAux = appsAux.Find(x => x.Id == m.AppId);

        //        MessageVM aux = new MessageVM(m.Id, m.AppId, appAux.RestKey, appAux.Name, appAux.Icon, m.Title, m.Content, m.SubTitle, m.Url);

        //        mvm.Add(aux);
        //    }

        //    return View(mvm);
        //}

        [AuthorizationFilter]
        public ActionResult Message(Guid appId, string restKey)
        {
            Message m = new Models.Message();

            m.AppId = appId;
            m.RestKey = restKey;

            return View(m);
        }

        [HttpPost]
        [AuthorizationFilter]
        public ActionResult Message(Message message)
        {
            App app = _appRep.GetData<App>(message.AppId);

            if (app.ClientId == _session.CurrentClient.Id)
            {
                message = OneSignalAPI.PostMessage(message);

                if (message.Id != Guid.Empty)
                {
                    _messageRep.PostData<Message>(message);
                    return View(message);
                }
                else
                {
                    return View(message);
                }
            }
            else
            {
                return View(message);
            }
        }

        [AuthorizationFilter]
        public ActionResult MessageReply(Guid messageId, Guid appId)
        {
            App app = _appRep.GetData<App>(appId);

            if (app.ClientId == _session.CurrentClient.Id)
            {
                MessageReply reply = OneSignalAPI.GetMessage(messageId, appId, app.RestKey);

                return View(reply);
            }
            else
            {
                return View("Index", CurrentIndex(_session.CurrentClient.Id));
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
                client.Type = "Comum";

                client.Id = Guid.NewGuid();

                client.Password = Hash.CreateHash(client.Password);

                if (_clientRep.PostData<Client>(client))
                {
                    _session.CurrentClient = client;
                    return RedirectToAction("Index", CurrentIndex(client.Id));
                }
                else
                    return View(client);
            }
            else
                return View();
        }

        [AuthorizationFilter]
        public ActionResult Device(Guid appId)
        {
            App app = _appRep.GetData<App>(appId);

            if (app.ClientId == _session.CurrentClient.Id)
            {
                List<Device> devices = OneSignalAPI.ListDevice(app.Id, app.RestKey);

                return View(devices);
            }
            else
            {
                return View("Index", CurrentIndex(_session.CurrentClient.Id));
            }
        }

        [AuthorizationFilter]
        public ActionResult GenerateCSV(Guid appId)
        {
            string url;
            App app = _appRep.GetData<App>(appId);

            if (app.ClientId == _session.CurrentClient.Id)
            {
                url = OneSignalAPI.GenerateCSV(app.Id, app.RestKey);

                return Redirect(url);
            }
            else
            {
                return View("Index", CurrentIndex(_session.CurrentClient.Id));
            }
        }

        [NonAction]
        private IndexVM CurrentIndex(Guid id)
        {
            IndexVM ivm = new IndexVM();
            List<MessageVM> mvm = new List<MessageVM>();
            List<Guid> appsId = new List<Guid>();

            ivm.Apps = (List<App>)_appRep.GetAppsByClient(_session.CurrentClient.Id);

            foreach (App a in ivm.Apps)
            {
                appsId.Add(a.Id);
            }

            IEnumerable<Message> messages = _messageRep.GetMessagesByApp((IEnumerable<Guid>)appsId);

            List<App> appsAux = (List<App>)ivm.Apps;

            foreach (Message m in messages)
            {
                App appAux = appsAux.Find(x => x.Id == m.AppId);

                MessageVM aux = new MessageVM(m.Id, m.AppId, appAux.RestKey, appAux.Name, appAux.Icon, m.Title, m.Content, m.SubTitle, m.Url);

                mvm.Add(aux);
            }

            ivm.Messages = mvm;

            ivm.isPremium = _session.CurrentClient.Type == "Premium" || ivm.Apps.Count() < 1;

            return ivm;
        }
    }
}