using Flunt.Validations;
using WebApi_Controller_EF_Dapper.AppDomain.Database.BaseEntity;

namespace WebApi_Controller_EF_Dapper.Domain.Database.Entities.Product
{
    public class Product : Entity
    {
        // usar private no set restringe o acesso
        // aos metodos somente pelo construtor da classe

        public string Name { get; private set; }
        public string Description { get; private set; }
        public Decimal Price { get; private set; }
        public bool Active { get; private set; } = true;

        //Categorias -------------------------------------------------
        public Guid CategoryId { get; private set; }

        public Category Category { get; private set; }

        //Pedidos ----------------------------------------------------
        public ICollection<Order> Orders { get; private set; }

        public Product()
        {
            // use uum construtor vazio sempre que operar com o ef
        }

        private void Validate()
        {
            //validacao com flunt
            var contract = new Contract<Product>()
                .IsNotNullOrEmpty(Name, "Name", "Nome é obrigatório")
                .IsGreaterOrEqualsThan(Name, 3, "Name")
                .IsGreaterOrEqualsThan(Price, 1, "Price")
                .IsNotNull(Category, "Category", "Category not found")
                .IsNotNullOrEmpty(Description, "Description")
                .IsGreaterOrEqualsThan(Description, 3, "Description");

            //.IsNotNullOrEmpty(CreatedBy, "CreatedBy", "O usuario criador é obrigatório")
            //.IsNotNullOrEmpty(EditedBy, "EditedBy", "O usuario alterador é obrigatório");

            AddNotifications(contract);
        }

        public void AddProduct(string name,
                               string description,
                               Decimal price,
                               bool active,
                               Category category,
                               string createdBy)

        {
            Name = name;
            Description = description;
            Category = category;
            Price = price;
            Active = active;

            //Audity ------------------------------------
            CreatedBy = createdBy;
            CreatedOn = DateTime.Now;

            Validate();
        }

        public void EditProduct(string name,
                                Decimal price,
                                bool active,
                                Category category,
                                string editedBy)
        {
            Name = name;
            Price = price;
            Category = category;
            Active = active;

            //Audity ------------------------------------
            EditedBy = editedBy;
            EditedOn = DateTime.Now;

            Validate();
        }
    }
}