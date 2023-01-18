namespace WebApi_Controller_EF_Dapper.Endpoints.Products.DTO
{
    public record ProductRequestDTO(
        String Name,
        string Description,
        Decimal Price,
        bool Active,
        Guid CategoryId
    );
}