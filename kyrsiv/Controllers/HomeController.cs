using AuthApp.Models;
using kyrsiv.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace kyrsiv.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserContext db;
        public HomeController(UserContext context)
        {
            db = context;
        }


        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Messeng(Messeng mess)
        {
            if (mess != null)
            {
                try
                {
                    db.Messengs.Add(mess);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch
                {
                    ViewBag.Error = "Ошибка отправки сообщения";
                    return RedirectToAction("Index");
                }

            }
            ViewBag.Error = "Ошибка отправки сообщения";
            return RedirectToAction("Index");
        }
    }
}