using GHDWebAPI.Data;
using GHDWebAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GHDWebAPI.Services
{
    /// <summary>
    /// Product service class
    /// </summary>

    public class ProductService : IProductService
    {

        /// <summary>
        /// readonly reference to the DB context
        /// </summary>
        private readonly GHDWebAPIContext _context;

        /// <summary>
        /// Initialise
        /// </summary>
        /// <param name="context"></param>
        public ProductService(GHDWebAPIContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all the products from the DB
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Product> GetAll()
        {
            return _context.Product.ToList();
        }

        /// <summary>
        /// Get product by id from the DB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Product GetById(int id)
        {
            return _context.Product.Where(x => x.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Add a new product to the DB
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Product Add(Product product)
        { 
            var result = _context.Product.Add(product);

            _context.SaveChanges();

            return result.Entity;
        }

        /// <summary>
        /// Update an existing product in the DB
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Product Update(Product product)
        {
            var result = _context.Product.Update(product);

            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();

            return result.Entity;
        }

        /// <summary>
        /// Delete product with the given id from the DB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteById(int id)
        {
            var product = GetById(id);
            var result = _context.Remove(product);
            _context.SaveChanges();
            return result != null ? true : false ;
        }

        /// <summary>
        /// Check if a product exists with the given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }

        /// <summary>
        /// Check if this product is unique - combination of Name and Brand defines a unique product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public bool IsProductUnique(Product product)
        {
            var result = !_context.Product.Any(p => product.Name == p.Name && product.Brand == p.Brand);

            return result;
        }

    }
}
