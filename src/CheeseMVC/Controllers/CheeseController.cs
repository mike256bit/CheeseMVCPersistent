using Microsoft.AspNetCore.Mvc;
using CheeseMVC.Models;
using System.Collections.Generic;
using CheeseMVC.ViewModels;
using CheeseMVC.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CheeseMVC.Controllers
{
    public class CheeseController : Controller
    {
        private CheeseDbContext context;

        public CheeseController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }

        static string errorBag = ""; //initialize error message container
        
        public IActionResult Index(int id)
        {
            ViewBag.Error = errorBag; //errorBag holds error messages during redirect, reset error viewbag to capture new errors

            if (id == 0) //list all cheeses
            {
                IList<Cheese> cheeses = context.Cheeses
                    .Include(p => p.Category)
                    .ToList();

                ViewBag.Title = "All Cheeses";
                return View(cheeses);
            }
            else{ //list cheeses based on category ID passed from link
                IList<Cheese> cheeses = context.Cheeses
                    .Include(p => p.Category)
                    .Where(c => c.CategoryID == id)
                    .ToList();

                ViewBag.Title = context.Categories.Single(c => c.ID == id).Name + " Cheeses";
                return View(cheeses);
            }
        }

        public IActionResult Add()
        {
            ViewBag.Title = "Add a Cheese";
            AddCheeseViewModel addCheeseViewModel = new AddCheeseViewModel(context.Categories.ToList());
            return View(addCheeseViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddCheeseViewModel addCheeseViewModel)
        {
            if (ModelState.IsValid)
            {
                CheeseCategory newCheeseCategory = context.Categories.Single(p => p.ID == addCheeseViewModel.CategoryID);
                
                // TODO: Add verification stage if adding duplicate cheeses by name

                Cheese newCheese = new Cheese
                {
                    Name = addCheeseViewModel.Name,
                    Description = addCheeseViewModel.Description,
                    Category = newCheeseCategory
                };
                
                //disallow exact duplicates in DB
                foreach (var aCheese in context.Cheeses.ToList())
                {
                    if (newCheese.Name == aCheese.Name &&
                        newCheese.Description == aCheese.Description &&
                        newCheese.Category == aCheese.Category)
                    {
                        errorBag = newCheese.Name + " is already in the database with that description! Exact matches are not allowed.";
                        return Redirect("/");
                    }
                }

                context.Cheeses.Add(newCheese);
                context.SaveChanges();
                errorBag = ""; //reset errorBag to clear old errors
               
                return Redirect("/");
            }

            //reload categories and re-present form
            addCheeseViewModel = new AddCheeseViewModel(context.Categories.ToList());
            return View(addCheeseViewModel);
        }

        public IActionResult Remove()
        {
            ViewBag.title = "Remove Cheeses";
            ViewBag.cheeses = context.Cheeses.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Remove(int[] cheeseIds)
        {
            foreach (int cheeseId in cheeseIds)
            {
                Cheese theCheese = context.Cheeses.Single(c => c.ID == cheeseId);
                context.Cheeses.Remove(theCheese);
            }

            context.SaveChanges();

            errorBag = "";
            return Redirect("/");
        }
    }
}
