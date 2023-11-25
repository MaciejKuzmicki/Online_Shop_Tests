using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using Projekt;
using Projekt.Authentication;

namespace Projekt.Pages.Users
{
    [RequireNoAuth]
    public class CreateModel : PageModel
    {
        private readonly Projekt.Userdb _context;

        public CreateModel(Projekt.Userdb context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            base.OnPageHandlerExecuting(context);
            if(HttpContext.Session.GetString("Role") != null)
            {
                context.Result = new RedirectResult("/Index");
            }
        }

        [BindProperty]
        public User User { get; set; } = default!;
        [Required, BindProperty]
        public String Name { get; set; }
        [Required, BindProperty]
        public String Surname { get; set; }
        [Required, BindProperty]
        public String Email { get; set; }
        [Required, BindProperty]
        public String Password { get; set; }
        [Required, BindProperty]
        public String PhoneNumber { get; set; }
        [Required, BindProperty]
        public String ObservedCategory { get; set; }
        

        public string errorMessage = "";

        public async Task<IActionResult> OnPostAsync()
        {
            User.Role = "client";
            User.Items = "";

          if (_context.Users == null || User == null)
            {
                return Page();
            }

          foreach (var user in _context.Users)
            {
                if (User.Email.Equals(user.Email))
                {
                    errorMessage += "There exists an account on the given e-mail";
                    return Page();
                }
            }


            _context.Users.Add(User);
            await _context.SaveChangesAsync();
        
            HttpContext.Session.SetInt32("Id", User.Id);
            HttpContext.Session.SetString("Name", User.Name);
            HttpContext.Session.SetString("Surname", User.Surname);
            HttpContext.Session.SetString("Password", User.Password);
            HttpContext.Session.SetString("Email", User.Email);
            HttpContext.Session.SetString("Role", User.Role);
            HttpContext.Session.SetString("ObservedCategory", User.ObservedCategory);
            HttpContext.Session.SetString("Items", User.Items);
            HttpContext.Session.SetString("PhoneNumber", User.PhoneNumber);


            
            return RedirectToPage("./Index");
        }
    }
}
