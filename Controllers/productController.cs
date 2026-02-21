using ecomerce1.Models;
using ecomerce1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecomerce1.Controllers
{
    [Authorize(Roles="Admin")]
    [Route("/Admin/[controller]/{action=Index}/{id?}")]
    public class productController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;
        private readonly int pageSize = 5 ;

        public productController(ApplicationDbContext context , IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index(int pageIndex , string? search , string? column, string? orderBy) 
        {
            IQueryable<product> query = context.products;

            //search functionality
            if(search != null)
            {
                query = query.Where(p=>p.Name.Contains(search) || p.Brand.Contains(search));
            }

            //sort functionality 
            string[] validdColumns = {"Id", "Name", "Brand", "Category", "Price", "CreatedAt" };
            string[] validOrderBy = {"desc", "asc" };

            if(!validdColumns.Contains(column))
            {
                column = "Id";
            }
            if(!validOrderBy.Contains(orderBy)) {
                orderBy = "desc"; 
            }


            if(column == "Name")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Name);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Name);
                }
            }

            else if (column == "Brand")
            {
                if (orderBy == "asc")
                    query = query.OrderBy(p => p.Brand);
                else
                {
                    query = query.OrderByDescending(p => p.Brand);
                }
            }
            else if (column == "Category")
            {
                if (orderBy == "asc")
                    query = query.OrderBy(p => p.Category);
                else
                {
                    query = query.OrderByDescending(p => p.Category);
                }
            }
            else if (column == "Price")
            {
                if (orderBy == "asc")
                    query = query.OrderBy(p => p.Price);
                else
                {
                    query = query.OrderByDescending(p => p.Price);
                }
            }
            else if (column == "Category")
            {
                if (orderBy == "asc")
                    query = query.OrderBy(p => p.Category);
                else
                {
                    query = query.OrderByDescending(p => p.Category);
                }
            }
            else if (column == "CreatedAt")
            {
                if (orderBy == "asc")
                    query = query.OrderBy(p => p.createdAt);
                else
                {
                    query = query.OrderByDescending(p => p.createdAt);
                }
            }
            else
            {
                if (orderBy == "asc")
                    query = query.OrderBy(p => p.Id);
                else
                {
                    query = query.OrderByDescending(p => p.Id);
                }
            }


            //query = query.OrderByDescending(p => p.Id);

            //Pagination functionality 
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            decimal count =query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex-1) * pageSize).Take(pageSize);

            var products =query.ToList();

            ViewData["pageIndex"] = pageIndex;
            ViewData["TotalPages"] = totalPages;
            ViewData["Search"] = search ?? "";
            ViewData["Column"]= column;
            ViewData["OrderBy"]= orderBy;


            return View(products);
        }
        public IActionResult Create() 
        { 
           return View(); 
        }
        [HttpPost]
        public IActionResult Create(productDto productDto)
        {
            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("Image file","is required");
            }
            if (!ModelState.IsValid) {
                return View(productDto);
            }

            // save the image file
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productDto.ImageFile!.FileName);

            string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            // save the new product in the database
            product product = new product()
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageFileName = newFileName,
                createdAt = DateTime.Now,
            };
            context.products.Add(product);
            context.SaveChanges();

            return RedirectToAction("Index","product");
        }

        public IActionResult Edit(int id) {
            var product = context.products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "product");
            }

            //create productDto from product 
            var productDto = new productDto()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description
            };

            ViewData["productId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAT"] = product.createdAt.ToString("dd/MM/yyyy");

            return View(productDto); 
        
        }

        [HttpPost]
        public IActionResult Edit(int id , productDto productDto)
        {
            var product = context.products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "product");
            }
            if(!ModelState.IsValid)
            {
                ViewData["productId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAT"] = product.createdAt.ToString("dd/MM/yyyy");
               
                return View(productDto);
            }
            // update the image file if we have a new image file
            string newFileName = product.ImageFileName;
            if (productDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(productDto.ImageFile.FileName);

                string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFile.CopyTo(stream);
                }

                // delete the old image
                string oldImageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
                System.IO.File.Delete(oldImageFullPath);
            }
            product.Name = productDto.Name;
            product.Brand = productDto.Brand;
            product.Category = productDto.Category;
            product.Price = productDto.Price;
            product.Description = productDto.Description;
            product.ImageFileName = newFileName;
            //product.createdAt= DateTime.Now; 

            context.SaveChanges();
            return RedirectToAction("Index", "Product");

        }

        public IActionResult Delete(int id)
        {
            var product = context.products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "product");
            }
            string imageFullPath = environment.WebRootPath + "/products" + product.ImageFileName ;
                System.IO.File.Delete(imageFullPath);
            context.products.Remove(product);
            context.SaveChanges(true);

            return RedirectToAction("Index" , "product");
        }
    }
}
