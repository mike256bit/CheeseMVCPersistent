using Microsoft.AspNetCore.Mvc;
using CheeseMVC.Models;
using System.Collections.Generic;
using CheeseMVC.ViewModels;
using CheeseMVC.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CheeseMVC.Controllers
{
    public class MenuController : Controller
    {
        private CheeseDbContext context;

        public MenuController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Index()
        {
            List<Menu> menus = context.Menus.ToList();
            ViewBag.Title = "Cheese Menus";
            return View(menus);
        }

        public IActionResult Add(int id)
        {
            AddMenuViewModel addMenuViewModel = new AddMenuViewModel();
            ViewBag.Title = "Add a Menu";
            return View(addMenuViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddMenuViewModel addMenuViewModel)
        {
            if (ModelState.IsValid)
            {
                Menu newMenu = new Menu()
                {
                    Name = addMenuViewModel.Name
                };

                context.Menus.Add(newMenu);
                context.SaveChanges();

                return Redirect("/Menu/ViewMenu/" + newMenu.ID);

            }

            return View(addMenuViewModel);
        }

        public IActionResult ViewMenu(int id)
        {
            Menu theMenu = context.Menus.Single(p => p.ID == id);

            List<CheeseMenu> items = context
                .CheeseMenus
                .Include(item => item.Cheese)
                .Where(cm => cm.MenuID == id)
                .ToList();

            ViewMenuViewModel viewMenuViewModel = new ViewMenuViewModel()
            {
                Menu = theMenu,
                Items = items
            };

            ViewBag.Title = theMenu.Name;
            return View(viewMenuViewModel);
        }

        public IActionResult AddItem(int id)
        {
            //TODO: FIX THIS
            Menu theMenu = context.Menus.Single(p => p.ID == id);
            List<Cheese> theCheeses = context.Cheeses.ToList();

            AddMenuItemViewModel addMenuItemViewModel = new AddMenuItemViewModel(theCheeses, theMenu);

            ViewBag.Title = "Add an item to \"" + theMenu.Name + "\"";
            return View(addMenuItemViewModel);
        }

        [HttpPost]
        public IActionResult AddItem(AddMenuItemViewModel addMenuItemViewModel)
        {
            if (ModelState.IsValid)
            {
                IList<CheeseMenu> existingCheck = context.CheeseMenus
                    .Where(cm => cm.CheeseID == addMenuItemViewModel.cheeseID)
                    .Where(cm => cm.MenuID == addMenuItemViewModel.menuID).ToList();

                if(existingCheck.Count == 0)
                {
                    CheeseMenu newCheeseMenu = new CheeseMenu
                    {
                        CheeseID = addMenuItemViewModel.cheeseID,
                        MenuID = addMenuItemViewModel.menuID
                    };

                    context.CheeseMenus.Add(newCheeseMenu);
                    context.SaveChanges(); 
                }

                return Redirect("/Menu/ViewMenu/" + addMenuItemViewModel.menuID);

            }

            ViewBag.Title = "Add an item to " + addMenuItemViewModel.Menu;
            return View(addMenuItemViewModel);
        }
    }
}
