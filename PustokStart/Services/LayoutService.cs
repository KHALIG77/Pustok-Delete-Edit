﻿using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PustokStart.DAL;
using PustokStart.Models;
using PustokStart.ViewModels;

namespace PustokStart.Services
{
    public class LayoutService
    {
        private readonly PustokContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LayoutService(PustokContext context,IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            this._httpContextAccessor = httpContextAccessor;
        }
        public Dictionary<string, string> GetSettings()
        {
            return _context.Settings.ToDictionary(x => x.Key, x => x.Value);

        }
        public List<Category> GetCategories()
        {
            return _context.Categories.Include(x=>x.SubCategories).ToList();
        }
        public BasketViewModel GetBasket()
        {
            var bv= new BasketViewModel();
            var basketJson = _httpContextAccessor.HttpContext.Request.Cookies["basket"];
            if (basketJson != null) 
            {
                var cookieItems = JsonConvert.DeserializeObject<List<BasketItemCookieViewModel>>(basketJson);
            
                foreach (var ci in cookieItems)
                {
                    BasketItemViewModel bi = new BasketItemViewModel
                    {
                        Count = (int)ci.Count,
                        Book = _context.Books.Include(x => x.BookImages).FirstOrDefault(x => x.Id == ci.BookId),

                    };
                    bv.BasketItems.Add(bi);
                    bv.TotalPrice += (bi.Book.DiscountPerctent > 0 ? (bi.Book.SalePrice * (100 - bi.Book.DiscountPerctent) / 100) : bi.Book.SalePrice) * bi.Count;
                }
            }

            return bv;  
        }
    }
}
