using Flunt.Validations;
using WebApi_Controller_EF_Dapper.AppDomain.Database.BaseEntity;

namespace WebApi_Controller_EF_Dapper.Domain.Database.Entities.Product
{
    public class Category : Entity
    {
        public string Name { get; private set; }
        public bool Active { get; private set; }

        private void Validate()
        {
            //validacao com flunt
            var contract = new Contract<Category>()
                .IsNotNullOrEmpty(Name, "Name", "Nome é obrigatório")
                .IsGreaterOrEqualsThan(Name, 3, "Name");

            //.IsNotNullOrEmpty(CreatedBy, "CreatedBy", "O usuario criador é obrigatório")
            //.IsNotNullOrEmpty(EditedBy, "EditedBy", "O usuario alterador é obrigatório");

            AddNotifications(contract);
        }

        public Category()
        {
            // use uum construtor vazio sempre que operar com o ef
        }

        public void AddCategory(string name, string createdBy)
        {
            Name = name;
            Active = true;

            //Audity ------------------------------------
            CreatedBy = createdBy;
            CreatedOn = DateTime.Now;

            Validate();
        }

        public void EditInfo(string name, bool active, string editedBy)
        {
            Active = active;
            Name = name;

            //Audity ------------------------------------
            EditedBy = editedBy;
            EditedOn = DateTime.Now;

            Validate();
        }

        public static implicit operator Category(Task<Category?> v)
        {
            throw new NotImplementedException();
        }
    }
}