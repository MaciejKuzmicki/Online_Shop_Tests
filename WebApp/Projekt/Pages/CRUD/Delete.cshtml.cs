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
    public class DeleteModel : PageModel
    {
        private readonly Projekt.Itemdb _context;
        private readonly Projekt.Userdb _UserContext;

        public DeleteModel(Projekt.Itemdb context, Projekt.Userdb UserContext)
        {
            _context = context;
            _UserContext = UserContext;
        }

        [BindProperty]
      public Item Item { get; set; } = default!;
        

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Itemos == null)
            {
                return NotFound();
            }

            var item = await _context.Itemos.FirstOrDefaultAsync(m => m.Id == id);

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

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (id == null || _context.Itemos == null)
            {
                return NotFound();
            }
            var item = await _context.Itemos.FindAsync(id);
            IList<User> users = await _UserContext.Users.ToListAsync();
            foreach(var user in users)
            {
                String listOfItems = user.Items;
                IList<int> itemsId = new List<int>();
                string[] parts = listOfItems.Split(':');
                foreach(string part in parts)
                {
                    int number;
                    if (int.TryParse(part, out number))
                    {
                        itemsId.Add(number);
                    }
                }
                if(itemsId.Contains(id))
                {
                    string newItems = "";
                    for (int i = 0; i <  itemsId.Count; i++)
                    {
                        if (itemsId[i] != id)
                        {
                            newItems += ":" + itemsId[i];
                        }
                    }
                    user.Items = newItems;
                }
            }
            if (item != null)
            {
                Item = item;
                _context.Itemos.Remove(Item);
                await _context.SaveChangesAsync();
                await _UserContext.SaveChangesAsync();
            }

            return RedirectToPage("/Users/MyItems");
        }
    }
}
