using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using LabFix.NetcoreMVC.Models;
using System.Linq;
using System.Text;
using System;

namespace LabFix.NetcoreMVC.Controllers
{
    public class ManageController : Controller
    {
        private northwindContext _dbContext;
        public ManageController(northwindContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Customer()
        {

            var cusAll = _dbContext.Customers.ToList();

            return View(cusAll);

        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        public string GenerateAutoId(int length)
        {
            const string valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        [HttpPost]
        public IActionResult Create(Customers customers)
        {
            if (ModelState.IsValid)
            {
                customers.Customerid = GenerateAutoId(4);
                _dbContext.Customers.Add(customers);
                _dbContext.SaveChanges();

                return RedirectToAction("Customer");
            }

            return View(customers);
        }


        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            else
            {
                var data = _dbContext.Customers.FirstOrDefault(x => x.Customerid == id);
                return View(data);
            }
        }

        [HttpPost]
        public IActionResult Edit(
            string id,
            [Bind("Customerid,Companyname,Contactname,Contacttitle,Country,Postalcode")] Customers customers)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _dbContext.Customers.Update(customers);
                _dbContext.SaveChanges();

                return RedirectToAction("Customer");
            }
            return View(customers);
        }

        public IActionResult Delete(string id)
        {
            var cust = _dbContext.Customers.FirstOrDefault(m => m.Customerid == id);
            if (cust == null)
            {
                return RedirectToAction("Customer");
            }
            _dbContext.Customers.Remove(cust);
            _dbContext.SaveChanges();
            return RedirectToAction("Customer");
        }

        public IActionResult Error()
        {
            return View();
        }
    }

}