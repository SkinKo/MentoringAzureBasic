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
                ProductID as Id,
                Name,
                ProductNumber,
                Color,
                StandardCost,
                ListPrice,
                Size,
                Weight,
                ProductCategoryID,
                ProductModelID,
                SellStartDate,
                SellEndDate,
                DiscontinuedDate
                    from SalesLT.Product", commandType: CommandType.Text);
            }
        }

        public Product Get(int id)
        {
            using (
                SqlConnection con = new SqlConnection(_connectionString))
            {
                return con.QueryFirstOrDefault<Product>(@"select 
                ProductID as Id,
                Name,
                ProductNumber,
                Color,
                StandardCost,
                ListPrice,
                Size,
                Weight,
                ProductCategoryID,
                ProductModelID,
                SellStartDate,
                SellEndDate,
                DiscontinuedDate
                    from SalesLT.Product where ProductID = @id", new { id }, commandType: CommandType.Text);
            }
        }

        public bool Add(Product product)
        {
            using (
                SqlConnection con = new SqlConnection(_connectionString))
            {
                return con.Execute(@"insert into SalesLT.Product (
                                                        ProductID,
                                                        Name,
                                                        ProductNumber,
                                                        Color,
                                                        StandardCost,
                                                        ListPrice,
                                                        Size,
                                                        Weight,
                                                        ProductCategoryID,
                                                        ProductModelID,
                                                        SellStartDate,
                                                        SellEndDate,
                                                        DiscontinuedDate 
                                                        ModifiedDate)
                                        values (@id, 
                                                @name, 
                                                @productNumber, 
                                                @color, 
                                                @standardCost, 
                                                @listPrice, 
                                                @size, 
                                                @weight, 
                                                @productCategoryID, 
                                                @productModelID, 
                                                @sellStartDate, 
                                                @sellEndDate, 
                                                @discontinuedDate,  
                                                GETDATE())",
                           new
                           {
                               id = product.Id,
                               name = product.Name,
                               productNumber = product.ProductNumber,
                               color = product.Color,
                               standardCost = product.StandardCost,
                               listPrice = product.ListPrice,
                               size = product.Size,
                               weight = product.Weight,
                               productCategoryID = product.ProductCategoryID,
                               productModelID = product.ProductModelID,
                               sellStartDate = product.SellStartDate,
                               sellEndDate = product.SellEndDate,
                               discontinuedDate = product.DiscontinuedDate
                           }, commandType: CommandType.Text) > 0;
            }
        }

        public bool Update(int id, Product product)
        {
            using (
    SqlConnection con = new SqlConnection(_connectionString))
            {
                return con.Execute(@"update SalesLT.Product
set Name = @name, 
    ProductNumber = @productNumber, 
    Color = @color, 
    StandardCost = @standardCost, 
    ListPrice = @listPrice, 
    Size = @size, 
    Weight = @weight, 
    ProductCategoryID = @productCategoryID, 
    ProductModelID = @productModelID, 
    SellStartDate = @sellStartDate, 
    SellEndDate = @sellEndDate, 
    DiscontinuedDate = @discontinuedDate, 
    ModifiedDate = GETDATE()
where ProductID = @id",
                           new
                           {
                               id = product.Id,
                               name = product.Name,
                               productNumber = product.ProductNumber,
                               color = product.Color,
                               standardCost = product.StandardCost,
                               listPrice = product.ListPrice,
                               size = product.Size,
                               weight = product.Weight,
                               productCategoryID = product.ProductCategoryID,
                               productModelID = product.ProductModelID,
                               sellStartDate = product.SellStartDate,
                               sellEndDate = product.SellEndDate,
                               discontinuedDate = product.DiscontinuedDate
                           }, commandType: CommandType.Text) > 0;
            }
        }

        public bool Delete(int id)
        {
            using (
                SqlConnection con = new SqlConnection(_connectionString))
            {
                return con.Execute(@"delete from SalesLT.Product 
                                        where ProductID = @id",
                           new { id }, commandType: CommandType.Text) > 0;
            }
        }
    }
}
