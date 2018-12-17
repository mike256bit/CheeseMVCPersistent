using CheeseMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheeseMVC.ViewModels
{
    public class AddMenuItemViewModel
    {
        public int cheeseID { get; set; }
        public int menuID { get; set; }

        public Menu Menu { get; set; }
        public List<SelectListItem> Cheeses { get; set; } = new List<SelectListItem>();

        public AddMenuItemViewModel() { }

        public AddMenuItemViewModel(IEnumerable<Cheese> cheeses, Menu menu)
        {
            Menu = menu;

            foreach(Cheese aCheese in cheeses)
            {
                Cheeses.Add(new SelectListItem
                {
                    Value = aCheese.ID.ToString(),
                    Text = aCheese.Name
                });   
            }
        }
    }
}
