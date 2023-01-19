using Microsoft.AspNetCore.Mvc;
using WebApi_Controller_EF_Dapper.AppDomain.Extensions.ErroDetailedExtension;
using WebApi_Controller_EF_Dapper.Domain.Database;
using WebApi_Controller_EF_Dapper.Domain.Database.Entities.Product;
using WebApi_Controller_EF_Dapper.Endpoints.Orders.DTO;

namespace WebApi_Controller_EF_Dapper.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public OrderController(ILogger<OrderController> logger,
                                 ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        //------------------------------------------------------------------------------------
        //EndPoints
        //------------------------------------------------------------------------------------
        [HttpGet,Route("{id:guid}")]
        public IActionResult OrderGet([FromRoute] Guid id)
        {
            //Usuario fixo, mas  poderia vir de um identity
            string userName = "doe joe";

            var order = _dbContext.Orders.FirstOrDefault(order => order.Id == id);

            var productsResponseDTO = order.Products.Select(p => new OrderProductDTO(p.Id,
                                                                                     p.Name));

            var orderResponseDTO = new OrderResponseDTO(order.Id,
                                                        userName,
                                                        productsResponseDTO
                                                        );

            return new ObjectResult(orderResponseDTO);
        }

        [HttpPost, Route("")]
        public async Task<IActionResult> OrderPost(OrderRequestDTO orderRequestDTO)
        {
            //Usuario fixo, mas  poderia vir de um identity
            var userId = "123456";
            var userName = "Doe Joe Client";

            var products = new List<Product>();

            List<Product> orderProducts = new List<Product>();

            if (orderRequestDTO.ProductListIds.Any())

                orderProducts = _dbContext.Products.Where(p => orderRequestDTO.ProductListIds
                                                                           .Contains(p.Id))
                                                                           .ToList();
            if (orderProducts == null)
            {
                return new ObjectResult(Results.NotFound());
            }

            var order = new Order();

            order.AddOrder(userId, userName, (orderProducts));

            if (!order.IsValid)
            {
                return new ObjectResult(Results.ValidationProblem(order.Notifications.ConvertToErrorDetails()));
            }

            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            return new ObjectResult(Results.Created($"/orders/{order.Id}", order.Id));
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