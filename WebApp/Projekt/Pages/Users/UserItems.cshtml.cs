using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Projekt.Authentication;
using SendGrid.Helpers.Mail;
using System.Data;

namespace Projekt.Pages.Users
{
    [BindProperties(SupportsGet = true)]
    [RequireAuth(RequiredRole = "admin")]
    public class UserItemsModel : PageModel
    {
        private readonly Itemdb _itemContext;
        private readonly Userdb _userContext;

        public String Category { get; set; } = "Any";
        public String? Name { get; set; }
        public Double? Min { get; set; } = null;
        public Double? Max { get; set; } = null;
        public String errorMessage { get; set; } = "";
        public String secondErrorMessage { get; set; } = "";
        public int id { get; set; }


        public UserItemsModel(Itemdb itemContext, Userdb userContext)
        {
            _itemContext = itemContext;
            _userContext = userContext;
        }

        public IList<Item> Temp { get; set; } = new List<Item>();
        public IList<Item> Item { get; set; } = default!;
        public IList<Item> TempTemp { get; set; } = default!;

        public bool errors { get; set; } = true;

        public String UserName = default!;

        public async Task<IActionResult> OnGetAsync()
        {

            int.TryParse(Request.Cookies["userId"], out int id);
            var user = await _userContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (id == null || _itemContext == null || _userContext == null || user == null)
            {
                return NotFound();
            }
            UserName = user.Name;

            IList<int> itemsId = new List<int>();
            string[] parts = user.Items.Split(':');
            foreach (string part in parts)
            {
                int number;
                if (int.TryParse(part, out number))
                {
                    itemsId.Add(number);
                }
            }

            if (Min > Max)
            {
                errorMessage = "Min price cannot be higher than the max price";
            }
            if (Min < 0 || Max < 0)
            {
                secondErrorMessage = "Price cannot be negative";
            }
            if (errorMessage.Length > 0 || secondErrorMessage.Length > 0)
            {
                errors = false;
                return Page();
            }

            if (_itemContext.Itemos != null)
            {
                TempTemp = await _itemContext.Itemos.ToListAsync();
                foreach (var item in TempTemp)
                {
                    if (itemsId.Contains(item.Id)) Temp.Add(item);
                }
                if (Category.Equals("Any") && Name == null)
                {
                    Item = Temp.ToList();
                }
                else if (!Category.Equals("Any") && Name == null)
                {
                    for (int i = 0; i < Temp.Count; i++)
                    {
                        if (Temp[i].Category == Category)
                        {
                            Item.Add(Temp[i]);
                        }
                    }
                }
                else if (Category.Equals("Any") && Name != null)
                {
                    for (int i = 0; i < Temp.Count; i++)
                    {
                        if (Temp[i].Name.Contains(Name))
                        {
                            Item.Add(Temp[i]);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Temp.Count; i++)
                    {
                        if (Temp[i].Name.Contains(Name) && Temp[i].Category == Category)
                        {
                            Item.Add(Temp[i]);
                        }
                    }
                }

                for (int i = Item.Count - 1; i >= 0; i--)
                {
                    var item = Item[i];
                    Double minValue = Min == null ? Double.MinValue : (Double)Min;
                    Double maxValue = Max == null ? Double.MaxValue : (Double)Max;
                    if (item.Price > maxValue || item.Price < minValue)
                    {
                        Item.RemoveAt(i);
                    }
                }
            }

            return Page();

        }
    }
}
