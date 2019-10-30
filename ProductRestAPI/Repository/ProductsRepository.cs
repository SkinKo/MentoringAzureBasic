using ProductRestAPI.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace ProductRestAPI.Repository
{
    public interface IProductsRepository
    {
        IEnumerable<Product> Get();
        Product Get(int id);
        bool Add(Product product);
        bool Update(int id, Product product);
        bool Delete(int id);
    }

    public class ProductsRepository : IProductsRepository
    {
        private readonly string _connectionString;

        public ProductsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Product> Get()
        {
            using (
                SqlConnection con = new SqlConnection(_connectionString))
            {
                return con.Query<Product>(@"select 
                BusinessEntityID as Id,
                PersonType,
                NameStyle,
                Title,
                FirstName,
                MiddleName,
                LastName,
                Suffix,
                EmailPromotion,
                AdditionalContactInfo,
                Demographics
                    from Person.Person", commandType: CommandType.Text);
            }
        }

        public Product Get(int id)
        {
            using (
                SqlConnection con = new SqlConnection(_connectionString))
            {
                return con.QueryFirstOrDefault<Product>(@"select 
                BusinessEntityID as Id,
                PersonType,
                NameStyle,
                Title,
                FirstName,
                MiddleName,
                LastName,
                Suffix,
                EmailPromotion,
                AdditionalContactInfo,
                Demographics
                    from Person.Person where BusinessEntityID = @id", new { id }, commandType: CommandType.Text);
            }
        }

        public bool Add(Product product)
        {
            using (
                SqlConnection con = new SqlConnection(_connectionString))
            {
                return con.Execute(@"insert into Person.Person (
                                                        BusinessEntityID, 
                                                        PersonType, 
                                                        NameStyle, 
                                                        Title, 
                                                        FirstName, 
                                                        MiddleName, 
                                                        LastName, 
                                                        Suffix, 
                                                        EmailPromotion, 
                                                        AdditionalContactInfo, 
                                                        Demographics, 
                                                        ModifiedDate)
                                        values (@id, 
                                                @personType, 
                                                @nameStyle, 
                                                @title, 
                                                @firstName, 
                                                @middleName, 
                                                @lastName, 
                                                @suffix, 
                                                @emailPromotion, 
                                                @additionalContactInfo, 
                                                @demographics, 
                                                GETDATE())",
                           new
                           {
                               id = product.Id,
                               personType = product.PersonType,
                               nameStyle = product.NameStyle,
                               title = product.Title,
                               firstName = product.FirstName,
                               middleName = product.MiddleName,
                               lastName = product.LastName,
                               suffix = product.Suffix,
                               emailPromotion = product.EmailPromotion,
                               additionalContactInfo = product.AdditionalContactInfo,
                               demographics = product.Demographics
                           }, commandType: CommandType.Text) > 0;
            }
        }

        public bool Update(int id, Product product)
        {
            using (
    SqlConnection con = new SqlConnection(_connectionString))
            {
                return con.Execute(@"update Person.Person 
set PersonType = @personType, 
    NameStyle = @nameStyle, 
    Title = @title, 
    FirstName = @firstName, 
    MiddleName = @middleName, 
    LastName = @lastName, 
    Suffix = @suffix, 
    EmailPromotion = @emailPromotion, 
    AdditionalContactInfo = @additionalContactInfo, 
    Demographics = @demographics, 
    ModifiedDate = GETDATE()
where BusinessEntityID = @id",
                           new
                           {
                               id = product.Id,
                               personType = product.PersonType,
                               nameStyle = product.NameStyle,
                               title = product.Title,
                               firstName = product.FirstName,
                               middleName = product.MiddleName,
                               lastName = product.LastName,
                               suffix = product.Suffix,
                               emailPromotion = product.EmailPromotion,
                               additionalContactInfo = product.AdditionalContactInfo,
                               demographics = product.Demographics
                           }, commandType: CommandType.Text) > 0;
            }
        }

        public bool Delete(int id)
        {
            using (
                SqlConnection con = new SqlConnection(_connectionString))
            {
                return con.Execute(@"delete from Person.Person 
                                        where BusinessEntityID = @id",
                           new { id }, commandType: CommandType.Text) > 0;
            }
        }
    }
}
