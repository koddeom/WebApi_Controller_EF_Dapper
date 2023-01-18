namespace WebApi_Controller_EF_Dapper.Endpoints.Products.DTO
{
    public record ProductSoldResponseDTO(
         Guid Id,
         string Name,
         int Amount
        );
}