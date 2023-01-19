using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi_Controller_EF_Dapper.AppDomain.Extensions.ErroDetailedExtension;
using WebApi_Controller_EF_Dapper.Business;
using WebApi_Controller_EF_Dapper.Domain.Database;
using WebApi_Controller_EF_Dapper.Domain.Database.Entities.Product;
using WebApi_Controller_EF_Dapper.Endpoints.Products.DTO;

namespace WebApi_Controller_EF_Dapper.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly ServiceAllProductsSold _serviceAllProductsSold;

        public ProductController(ILogger<ProductController> logger,
                                 ApplicationDbContext dbContext,
                                 ServiceAllProductsSold serviceAllProductsSold)
        {
            _logger = logger;
            _dbContext = dbContext;
            _serviceAllProductsSold = serviceAllProductsSold;
        }

        //------------------------------------------------------------------------------------
        //EndPoints
        //------------------------------------------------------------------------------------
        [HttpGet,Route("{id:guid}")]
        public IActionResult ProductGet([FromRoute] Guid id)
        {
            var products = _dbContext.Products
                                     .Include(p => p.Category)
                                     .Where(p => p.Id == id)
                                     .OrderBy(p => p.Name)
                                     .ToList();

            var productResponseDTO = products.Select(p => new ProductResponseDTO(
                                                        p.Id,
                                                        p.Name,
                                                        p.Description,
                                                        p.Price,
                                                        p.Active
                                                     ));

            return new ObjectResult(productResponseDTO);
        }

        [HttpGet,Route("")]
        public IActionResult ProductsGetAll()
        {
            var products = _dbContext.Products
                                  .AsNoTracking()
                                  .Include(p => p.Category)
                                  .OrderBy(p => p.Name)
                                  .ToList();

            var productsResponseDTO = products.Select(p => new ProductResponseDTO(
                                                        p.Id,
                                                        p.Name,
                                                        p.Description,
                                                        p.Price,
                                                        p.Active
                                                     ));

            return new ObjectResult(productsResponseDTO);
        }

        [HttpGet, Route("{id:guid}/solds")]
        public IActionResult ProductSoldGet()
        {
            var result = _serviceAllProductsSold.Execute();
            return new ObjectResult(result);
        }

        [HttpPost, Route("")]
        public async Task<IActionResult> ProductPost(ProductRequestDTO productRequestDTO)
        {
            //Usuario fixo, mas  poderia vir de um identity
            string user = "doe joe";

            //Recupero a categoria de forma sincrona
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == productRequestDTO.CategoryId);

            var product = new Product();

            product.AddProduct(productRequestDTO.Name,
                                productRequestDTO.Description,
                                productRequestDTO.Price,
                                true,
                                category,
                                user
                                );

            if (!product.IsValid)
            {
                return new ObjectResult(Results.ValidationProblem(product.Notifications.ConvertToErrorDetails()));
            }

            await _dbContext.Products.AddAsync(product);
            _dbContext.SaveChanges();

            return new ObjectResult(Results.Created($"/products/{product.Id}", product.Id));
        }

        [HttpPut, Route("{id:guid}")]
        public IActionResult ProductPut([FromRoute] Guid id,
                                         ProductRequestDTO productRequestDTO)
        {
            //Usuario fixo, mas  poderia vir de um identity
            string user = "doe joe";

            //Recupero o produto do banco
            var product = _dbContext.Products.FirstOrDefault(c => c.Id == id);

            if (product == null)
            {
                return new ObjectResult(Results.NotFound());
            }

            //Recupero a categoria de forma sincrona
            var category = _dbContext.Categories.FirstOrDefault(c => c.Id == productRequestDTO.CategoryId);

            if (category == null)
            {
                return new ObjectResult(Results.NotFound());
            }

            product.EditProduct(productRequestDTO.Name,
                                productRequestDTO.Price,
                                true,
                                category,
                                user
                                );

            if (!product.IsValid)
            {
                return new ObjectResult(Results.ValidationProblem(product.Notifications.ConvertToErrorDetails()));
            }

            _dbContext.SaveChanges();

            return new ObjectResult(Results.Ok());
        }

        [HttpDelete, Route("{id:guid}")]
        public IActionResult ProductDelete([FromRoute] Guid id)
        {
            //Recupero o produto do banco
            var product = _dbContext.Products.FirstOrDefault(c => c.Id == id);

            if (product == null)
            {
                return new ObjectResult(Results.NotFound());
            }

            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();

            return new ObjectResult(Results.Ok());
        }
    }
}

//[HttpPost]
//public IActionResult Create([FromBody] TodoItem item)
//{
//    if (item == null)
//    {
//        return BadRequest();
//    }
//    TodoItems.Add(item);
//    return CreatedAtRoute("GetTodo", new { id = item.Key }, item);
//}

//[HttpPut("{id}")]
//public IActionResult Update(string id, [FromBody] TodoItem item)
//{
//    if (item == null || item.Key != id)
//    {
//        return BadRequest();
//    }
//    var todo = TodoItems.Find(id);
//    if (todo == null)
//    {
//        return NotFound();
//    }
//    TodoItems.Update(item);
//    return new NoContentResult();
//}

//[HttpDelete("{id}")]
//public void Delete(string id)
//{
//    TodoItems.Remove(id);
//}

//public IEnumerable<TodoItem> GetAll()
//{
//    return TodoItems.GetAll();
//}

//[HttpGet("{id}", Name = "GetTodo")]
//public IActionResult GetById(string id)
//{
//    var item = TodoItems.Find(id);
//    if (item == null)
//    {
//        return NotFound();
//    }
//    return new ObjectResult(item);
//}