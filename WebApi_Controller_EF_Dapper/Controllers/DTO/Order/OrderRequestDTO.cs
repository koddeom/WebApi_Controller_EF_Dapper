namespace WebApi_Controller_EF_Dapper.Endpoints.Orders.DTO
{
    public record OrderRequestDTO(
        List<Guid> ProductListIds
    );
}