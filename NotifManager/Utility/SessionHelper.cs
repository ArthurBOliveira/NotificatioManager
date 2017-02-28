using NotifManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotifManager.Utility
{
    public class SessionHelper
    {
        private const string SessionCurrentClient = "CurrentClient";

        private System.Web.SessionState.HttpSessionState _context;

        public SessionHelper(System.Web.SessionState.HttpSessionState stateBase)
        {
            _context = stateBase;
        }

        public string SessionId
        {
            get
            {
                return _context.SessionID;
            }
        }

        public Client CurrentClient
        {
            get
            {
                if (_context[SessionCurrentClient] == null)
                    _context[SessionCurrentClient] = new Client();
                return (Client)_context[SessionCurrentClient];
            }
            set
            {
                _context[SessionCurrentClient] = value;
            }
        }
    }
}