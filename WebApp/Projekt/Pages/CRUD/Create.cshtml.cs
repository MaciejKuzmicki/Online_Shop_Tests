using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Projekt;
using Projekt.Authentication;

namespace Projekt.CRUD
{
    [RequireAuth(RequiredRole="client")]
    public class CreateModel : PageModel
    {
        private readonly Projekt.Itemdb _context;
        private readonly Projekt.Userdb _userContext;

        public CreateModel(Projekt.Itemdb context, Projekt.Userdb userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public IActionResult OnGet()
        {
            return Page();
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
        [BindProperty]
        public IFormFile ImageFile { get; set; }

        public String ImageData { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {

            String Email = HttpContext.Session.GetString("Email") ?? "";

            

            byte[] bytes = null;
            if(Item.ImageFile != null)
            {
                using (Stream fs = Item.ImageFile.OpenReadStream())
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        bytes = br.ReadBytes((Int32)fs.Length);
                    }
                }
                Item.ImageData=Convert.ToBase64String(bytes, 0, bytes.Length);
            }
            _context.Itemos.Add(Item);
            await _context.SaveChangesAsync();


            foreach (var user in _userContext.Users)
            {
                if(user.Email.Equals(Email))
                {
                    user.Items += ":" + Item.Id;
					HttpContext.Session.SetString("Items", user.Items);
                    break;
				}
			}

            /*foreach(var user in _userContext.Users)
            {
                if(user.ObservedCategory.Equals(Category)) 
                {
                    string emailMessage = "Dear " + user.Name + " " + user.Surname +
                        "\nOn your observed category new item has appeared: " + Name +
                        "\nDo not forget to check this out before someone will buy it\n" +
                        "Best Regards\n" +
                        "Your ProjectTeam";
                    EmailSender.SendEmail(user.Email, user.Name + " " + user.Surname, "New Item Appeared", emailMessage).Wait();
                }
            }*/
            //await _userContext.SaveChangesAsync();


            return RedirectToPage("/Users/MyItems");
        }
    }
}
