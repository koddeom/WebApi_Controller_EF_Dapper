namespace WebApi_Controller_EF_Dapper.Endpoints.Products.DTO
{
    public record ProductResponseDTO(
        Guid Id,
        String Name,
        string Description,
        Decimal Price,
        bool Active
    );
}