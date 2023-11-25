using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Projekt;
using Projekt.Authentication;

namespace Projekt.Pages.Users
{
    [BindProperties(SupportsGet = true)]
    [RequireAuth(RequiredRole ="admin")]
    public class IndexModel : PageModel
    {
        private readonly Projekt.Userdb _context;
        private readonly Itemdb _itemContext;


        public IndexModel(Projekt.Userdb context, Itemdb itemContext)
        {
            _context = context;
            _itemContext = itemContext;
        }

        public IList<User> User { get;set; } = default!;
        public User User2 { get; set; } = default!;

        public string? Password { get; set; } = "";
        public string? ConfirmPassword { get; set; } = "";
        public string errorMessage { get; set; } = "";
        public string? password { get; set; } = "";

        public async Task OnGetAsync()
        {
            if (_context.Users != null)
            {
                User = await _context.Users.ToListAsync();
            }
        }

        public async Task OnPostAsync()
        {
            password = HttpContext.Session.GetString("Password");
            await Console.Out.WriteLineAsync("PASSWORD2: " + password);

            string submitButton = Request.Form["action"];
            User = await _context.Users.ToListAsync();
            if (submitButton.Equals("password"))
            {
                if (Password == null || ConfirmPassword == null)
                {
                    errorMessage = "Type the Password and Confirm Password";
                    return; 
                }
                else if (!Password.Equals(ConfirmPassword))
                {
                    errorMessage = "Password and Confirm Password are not equal";
                    return;
                }
                else if(Password.Equals(ConfirmPassword) && !Password.Equals(password))
                {
                    errorMessage = "Password is incorrect";
                    return;
                }
                else
                {
                    int.TryParse(Request.Cookies["userId"], out int id);

                    if (id == null || _context.Users == null)
                    {
                        return;
                    }

                    var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
                    if (user == null)
                    {
                        return;
                    }
                    else
                    {
                        User2 = user;
                    }
                    await Console.Out.WriteLineAsync("HERE2");
                    string[] items = User2.Items.Split(':');
                    IList<int> itemsId = new List<int>();
                    foreach (string part in items)
                    {
                        int number;
                        if (int.TryParse(part, out number))
                        {
                            itemsId.Add(number);
                        }
                    }
                    await Console.Out.WriteLineAsync("HERE3");
                    foreach (var item in itemsId)
                    {
                        var item1 = await _itemContext.Itemos.FindAsync(item);
                        if (item1 != null) _itemContext.Itemos.Remove(item1);
                    }
                    _context.Users.Remove(User2);
                    await _itemContext.SaveChangesAsync();
                    await _context.SaveChangesAsync();
                    User = await _context.Users.ToListAsync();

                    RedirectToPage("Users/UserItems");
                }
            }
            return;
        }
    }
}
