using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projekt;

namespace Projekt.CRUD
{
    public class EditModel : PageModel
    {
        private readonly Projekt.Itemdb _context;

        public EditModel(Projekt.Itemdb context)
        {
            _context = context;
        }

        [BindProperty]
        public Item Item { get; set; } = default!;
        [BindProperty]
        public String Name { get; set; }
        [BindProperty]
        public String Description { get; set; }
        [BindProperty]
        public String State { get; set; }
        [BindProperty]
        public Double Price { get; set; }
        [BindProperty]
        public String Category { get; set; }
        
        public IFormFile? ImageFile { get; set; }
        [BindProperty]
        public String ImageData { get; set; }
        private String temp { get; set; }
        
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Itemos == null)
            {
                return NotFound();
            }
            
            var item =  await _context.Itemos.FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            Item = item;
            Name = item.Name;
            Description = item.Description;
            State = item.State;
            Price = item.Price;
            Category = item.Category;
            
            ImageData = item.ImageData;

            return Page();
        }

        
        public async Task<IActionResult> OnPostAsync()
        {
           
            _context.Attach(Item).State = EntityState.Modified;
            
            try
            {
                if (Item.ImageFile != null)
                {
                    byte[] bytes = null;
                    using (Stream fs = Item.ImageFile.OpenReadStream())
                    {
                        using (BinaryReader br = new(fs))
                        {
                            bytes = br.ReadBytes((Int32)fs.Length);
                        }
                    }

                    Item.ImageData = Convert.ToBase64String(bytes, 0, bytes.Length);
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(Item.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("/Users/MyItems");
        }

        private bool ItemExists(int id)
        {
          return (_context.Itemos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
