using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi_Controller_EF_Dapper.AppDomain.Extensions.ErroDetailedExtension;
using WebApi_Controller_EF_Dapper.Domain.Database;
using WebApi_Controller_EF_Dapper.Domain.Database.Entities.Product;
using WebApi_Controller_EF_Dapper.Endpoints.Categories.DTO;

namespace WebApi_Controller_EF_Dapper.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly ApplicationDbContext _dbContext;
        
        public CategoryController(ILogger<CategoryController> logger,
                                 ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        //------------------------------------------------------------------------------------
        //EndPoints
        //------------------------------------------------------------------------------------

        [HttpGet,Route("{id:guid}")]
        public IActionResult CategoryGet([FromRoute] Guid id)
        {
            var Categorys = _dbContext.Categories
                         .AsNoTracking()
                         .ToList();

            var categoryResponseDTO = Categorys.Where(p => p.Id == id)
                                               .Select(p => new CategoryResponseDTO(
                                                       p.Id,
                                                       p.Name,
                                                       p.Active
                                                     ));

            return new ObjectResult(categoryResponseDTO);
        }

        [HttpGet,Route("")]
        public IActionResult CategorysGetAll()
        {

            //teste
            var pathBase = HttpContext.Request.PathBase;

            var categories = _dbContext.Categories
                          .AsNoTracking()
                          .ToList();

            var categoriesResponseDTO = categories.Select(c => new CategoryResponseDTO
            (
                c.Id,
                c.Name,
                c.Active
            ));

            return new ObjectResult(categoriesResponseDTO);
        }

        [HttpPost,Route("")]
        public async Task<IActionResult> CategoryPost(CategoryRequestDTO categoryRequestDTO)
        {
            //Usuario fixo, mas  poderia vir de um identity
            string user = "doe joe";

            var category = new Category();

            category.AddCategory(categoryRequestDTO.Name,
                                  user);

            if (!category.IsValid)
            {
                return new ObjectResult(Results.ValidationProblem(category.Notifications.ConvertToErrorDetails()));
            }

            await _dbContext.Categories.AddAsync(category);
            _dbContext.SaveChanges();

            return new ObjectResult(Results.Created($"/category/{category.Id}", category.Id));
        }


        [HttpPut,Route("{id:guid}")]
        public IActionResult CategoryPut([FromRoute] Guid id,
                                         CategoryRequestDTO categoryRequestDTO)
        {
            //Usuario fixo, mas  poderia vir de um identity
            string user = "doe joe";

            var category = _dbContext.Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return new ObjectResult(Results.NotFound());
            }

            category.EditInfo(categoryRequestDTO.Name,
                              categoryRequestDTO.Active,
                              user);

            if (!category.IsValid)
            {
                return new ObjectResult(Results.ValidationProblem(category.Notifications
                                                         .ConvertToErrorDetails()));
            }

            _dbContext.SaveChanges();

            return new ObjectResult(Results.Ok());
        }

        [HttpDelete,Route("{id:guid}")]
        public IActionResult CategoryDelete([FromRoute] Guid id)
        {
            //Recupero o produto do banco
            var category = _dbContext.Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return new ObjectResult(Results.NotFound());
            }

            _dbContext.Categories.Remove(category);
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