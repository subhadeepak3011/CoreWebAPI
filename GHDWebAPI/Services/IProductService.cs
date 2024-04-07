using GHDWebAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace GHDWebAPI.Services
{
    /// <summary>
    /// Interface to the ProductService class
    /// </summary>
    public interface IProductService
    {
       
        public IEnumerable<Product>  GetAll(); 

        public Product GetById(int id);

        public Product Add(Product product);

        public Product Update(Product product);

        public bool DeleteById(int id);  

        bool ProductExists(int id);

        bool IsProductUnique(Product product);


    }
}
