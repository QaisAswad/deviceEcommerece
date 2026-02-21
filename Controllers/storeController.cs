using ecomerce1.Models;
using ecomerce1.Services;
using Microsoft.AspNetCore.Mvc;

namespace ecomerce1.Controllers
{
    public class storeController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly int pageSize = 8 ; 
        public storeController(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index(int pageIndex, string? search, string? brand, string? category, string? sort)
        {
            IQueryable<product> query = context.products;
            //search functionality : 
            if (search != null && search.Length > 0 ) {
                query = query.Where(p=>p.Name.Contains(search));
            }

            // filter functionality 
            if(brand != null && brand.Length >0 )
            {
                query = query.Where(p =>p.Brand.Contains(brand));
            }

            if (category != null && category.Length > 0)
            {
                query = query.Where(p => p.Category.Contains(category));
            }

            // sort functionality 
            if(sort == "price_asc") 
            {
                query = query.OrderBy(p => p.Price);
            }
            else if (sort == "price_desc")
            {
                query = query.OrderByDescending(p => p.Price);
            }
            else
            {
                //Newest
                query = query.OrderByDescending(p => p.Id);
            }

     

            //Pagination functionality 
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            var products = query.ToList();

            ViewBag.Products = products; 
            ViewBag.PageIndex = pageIndex;
            ViewBag.Totalpage = totalPages;

            var storeSearchModel = new storeSearchModel()
            {
                search = search,
                brand = brand,
                category = category,
                sort = sort
            };


            return View(storeSearchModel);
        }
        public IActionResult Details(int id)
        {
            var product = context.products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "store");
            }

            return View(product);
        }
    }
}
