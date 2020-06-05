using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zameen.Controllers.Resources;
using Zameen.Data;
using Zameen.Helpers;
using Zameen.Models;
using Zameen.Models.ViewModels;

namespace Zameen.Controllers
{

    [Authorize(Roles = Roles.Admin + "," + Roles.Executive)]
    public class CustomerController : Controller
    {

        private readonly ApplicationDbContext _db;

        [BindProperty]
        public CustomerViewModel CustomerVm { get; set; }

        public CustomerController(ApplicationDbContext db)
        {
            _db = db;
            CustomerVm = new CustomerViewModel()
            {
                Countries = _db.Countries.ToList(),
                Cities = _db.Cities.ToList(),
                Customer = new Models.Customer()
            };
        }

        public IActionResult Index()
        {
            var customer = _db.Customers
                .Include(m => m.Country)
                .Include(m => m.City);
            return View(customer);
        }

        public IActionResult Create()
        {
            return View(CustomerVm);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePost()
        {
            if (!ModelState.IsValid)
            {
                return View(CustomerVm);
            }
            _db.Customers.Add(CustomerVm.Customer);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Edit(int id)
        {
            CustomerVm.Customer = _db.Customers.Include(m =>
                    m.Country)
                .Include(m => m.City)
                .SingleOrDefault(m => m.Id == id);
            if (CustomerVm.Customer == null)
            {
                return NotFound();
            }

            return View(CustomerVm);
        }

        [HttpPost, ActionName("Edit")]
        public IActionResult EditPost()
        {
            if (!ModelState.IsValid)
            {
                return View(CustomerVm);
            }
            _db.Update(CustomerVm.Customer);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            Customer customer = _db.Customers.Find(id);
            if (customer == null)
            {
                return NotFound();
            }
            _db.Customers.Remove(customer);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


    }

}
