using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Projekt;
using Projekt.Authentication;

namespace Projekt.Pages.Users
{
	[BindProperties(SupportsGet = true)]
	[RequireAuth(RequiredRole = "client")]
	public class MyItemsModel : PageModel
	{
		private readonly Itemdb _context;
		private readonly Userdb _userContext;

		public String Category { get; set; } = "Any";
		public String? Search { get; set; }
        public Double? Min { get; set; } = null;
        public Double? Max { get; set; } = null;
        public String errorMessage { get; set; } = "";
        public String secondErrorMessage { get; set; } = "";

        public MyItemsModel(Itemdb context, Userdb userContext)
		{
			_context = context;
			_userContext = userContext;
		}

		public IList<Item> Item { get; set; } = default!;
		public IList<Item> Temp { get; set; } = default!;
		public IList<Item> TempTemp { get; set; } = default!;



		public async Task OnGetAsync()
		{
            String listOfItems = HttpContext.Session.GetString("Items") ?? "";
			IList<int> itemsId = new List<int>();
            string[] parts = listOfItems.Split(':');
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
                return;
            }


			
            if (_context.Itemos != null)
			{
				TempTemp = await _context.Itemos.ToListAsync();
				foreach (var item in TempTemp)
				{
					if (itemsId.Contains(item.Id)) Temp.Add(item);
				}
				if (Category.Equals("Any") && Search == null) 
				{ 
					Item = Temp; 
				}
				else if (!Category.Equals("Any") && Search == null)
				{
					for (int i = 0; i < Temp.Count; i++)
					{
						if (Temp[i].Category == Category)
						{
							Item.Add(Temp[i]);
						}
					}
				}
				else if (Category.Equals("Any") && Search != null)
				{
					for (int i = 0; i < Temp.Count; i++)
					{
						if (Temp[i].Name.Contains(Search))
						{
							Item.Add(Temp[i]);
						}
					}
				}
				else
				{
					for (int i = 0; i < Temp.Count; i++)
					{
						if (Temp[i].Name.Contains(Search) && Temp[i].Category == Category)
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
		}
	}
}
