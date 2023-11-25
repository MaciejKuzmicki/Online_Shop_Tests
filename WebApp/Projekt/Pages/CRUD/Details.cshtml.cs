using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Projekt;

namespace Projekt.CRUD
{
    public class DetailsModel : PageModel
    {
        private readonly Projekt.Itemdb _context;
        private readonly Projekt.Userdb _UserContext;

        public DetailsModel(Projekt.Itemdb context, Userdb userContext)
        {
            _context = context;
            _UserContext = userContext;
        }
        [BindProperty]  
        public Item Item { get; set; } = default!;
        [BindProperty]
        public string UserName { get; set; }
        [BindProperty]
        public string UserPhoneNumber { get; set; }
        [BindProperty]
        public string UserEmail { get; set; }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id == null || _context.Itemos == null)
            {
                return NotFound();
            }

            var item = await _context.Itemos.FirstOrDefaultAsync(m => m.Id == id);
            IList<User> users = await _UserContext.Users.ToListAsync();
            foreach (var user in users)
            {
                String listOfItems = user.Items;
                string[] parts = listOfItems.Split(':');
                foreach (string part in parts)
                {
                    int number;
                    if (int.TryParse(part, out number))
                    {
                        if(number == id)
                        {
                            UserName = user.Name;
                            UserPhoneNumber = user.PhoneNumber;
                            UserEmail = user.Email;
                        }
                    }
                }
            }
            if (item == null)
            {
                return NotFound();
            }
            else 
            {
                Item = item;
            }
            return Page();
        }
    }
}
