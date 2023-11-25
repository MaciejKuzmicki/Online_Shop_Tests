using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Projekt.Authentication;
using System.ComponentModel.DataAnnotations;

namespace Projekt.Pages.Users
{
    [RequireNoAuth]
    [BindProperties]
    public class LoginModel : PageModel
    {
		private readonly Projekt.Userdb _context;

		public LoginModel(Projekt.Userdb context)
		{
			_context = context;
		}

        public string errorMessage= "";

		[System.ComponentModel.DataAnnotations.Required(ErrorMessage = "The e-mail is required"), EmailAddress]
        public String Email { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Password is required")]
        public String Password { get; set; }

        public void OnGet()
        {
        }

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            base.OnPageHandlerExecuting(context);
            if(HttpContext.Session.GetString("Role") != null)
            {
                context.Result = new RedirectResult("/");
            }
        }
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            foreach(var user in _context.Users)
            {
                if(Email.Equals(user.Email))
                {
                    if(!Password.Equals(user.Password))
                    {
                        errorMessage += "Password is incorrect";
                        return Page();
                    }
					HttpContext.Session.SetInt32("Id", user.Id);
					HttpContext.Session.SetString("Name", user.Name);
					HttpContext.Session.SetString("Surname", user.Surname);
					HttpContext.Session.SetString("Password", user.Password);
					HttpContext.Session.SetString("Email", user.Email);
					HttpContext.Session.SetString("Role", user.Role);
					HttpContext.Session.SetString("ObservedCategory", user.ObservedCategory);
					HttpContext.Session.SetString("Items", user.Items);
					HttpContext.Session.SetString("PhoneNumber", user.PhoneNumber);
                    return RedirectToPage("./Index");
                    
				}
            }
            errorMessage += "User not found";
            return Page();
        }


    }
}
