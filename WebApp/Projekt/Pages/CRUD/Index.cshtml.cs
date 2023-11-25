using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using Projekt;

namespace Projekt.CRUD
{
    [BindProperties(SupportsGet = true)]
    public class IndexModel : PageModel
    {
        private readonly Projekt.Itemdb _context;

        public String Category { get; set; } = "Any";
        public String? Name { get; set; }
        public Double? Min { get; set; } = null;
        public Double? Max { get; set; } = null;
        public String errorMessage { get; set; } = "";
        public String secondErrorMessage { get; set; } = "";

        public IndexModel(Itemdb context)
        {
            _context = context;
        }

        public IList<Item> Item { get;set; } = default!;
		public IList<Item> Temp { get; set; } = default!;


        public async Task OnGetAsync()
        {
            if(Min > Max )
            {
                errorMessage = "Min price cannot be higher than the max price";
            }
            if (Min < 0 || Max < 0)
            {
                secondErrorMessage = "Price cannot be negative";
            }
            if(errorMessage.Length > 0 || secondErrorMessage.Length >0)
            {
                return;
            }

            if (_context.Itemos != null)
            {
                Temp = await _context.Itemos.ToListAsync();
                if (Category.Equals("Any") && Name == null)
                {
                    Item = Temp;
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
        }

      
      


    }
}
