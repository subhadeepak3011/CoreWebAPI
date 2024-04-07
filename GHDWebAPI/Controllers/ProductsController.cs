using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GHDWebAPI.Data;
using GHDWebAPI.Model;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using NuGet.Protocol;

namespace GHDWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly GHDWebAPIContext _context;
        private readonly ILogger<ProductsController> _logger;   
        public ProductsController(GHDWebAPIContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
         
        }

        /// <summary>
        /// Gets all the products from the DB
        /// </summary>
        /// <returns></returns>
        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct()
        {
            _logger.LogInformation("Fetching all products...");
            return await _context.Product.ToListAsync();
        }

        /// <summary>
        /// Gets the product with the given id from the DB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            _logger.LogInformation("Fetching the product with id: " + id);
            var product = await _context.Product.FindAsync(id);

            if (product == null)
            {
                _logger.LogError("Product Id: " + id + " could not be found");
                return NotFound();
            }

            return product;
        }

        /// <summary>
        /// Updates the product with the given id in the DB
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            try
            { 
                // ... check if this product has valid fields
                if (!IsProductValid(product))
                {
                    var errorModel = DisplayError(ModelState);
                    return new BadRequestObjectResult(errorModel);
                }

                // ... check if the productIds match
                if (id != product.Id)
                {
                    string msg = "Product Id does not match";
                    _logger.LogError(msg);
                    ModelState.AddModelError("", msg);
                    var errorModel = DisplayError(ModelState);
                    return new BadRequestObjectResult(errorModel);
                }

                // ... check if the model state is valid
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Model state is invalid, please check all the fields");
                    return new BadRequestObjectResult(ModelState);
                }

                _logger.LogInformation("Saving product id: " + id + " to the DB...");

                // ... update product
                _context.Entry(product).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product has been updated to the DB");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    _logger.LogError("Product Id does not exist, hence update operation cannot be performed");
                    ModelState.AddModelError("", "Product Id does not exist");
                    var errorModel = DisplayError(ModelState);
                    return new BadRequestObjectResult(errorModel);
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Creates a new product and adds it to the DB
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            try
            {
                // ... check if this product has valid fields
                if (!IsProductValid(product))
                {
                    var errorModel = DisplayError(ModelState);
                    return new BadRequestObjectResult(errorModel);
                }

                // ... check if the model state is valid
                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }

                _logger.LogInformation("Adding product to the DB...");

                _context.Product.Add(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product has been created and added to the DB");

                return CreatedAtAction("GetProduct", new { id = product.Id }, product);

            }
            catch(Exception e)
            {
                // ... handle any exceptions that might cause the API to crash : for instance trying to update the product Id
                var innerException = e.InnerException != null ? e.InnerException.Message : "";
                ModelState.AddModelError(e.Message, innerException);
                var errorModel = DisplayError(ModelState);

                // ... caution!!!
                _logger.LogError(innerException);
                return new BadRequestObjectResult(errorModel);
            }

        }

        /// <summary>
        /// Check if the product being created is a valid one
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        private bool IsProductValid(Product product)
        {
            // ... check if product is null
            if (product == null)
            {
                _logger.LogError("Product is null as it does not have valid fields");
                ModelState.AddModelError("", "Please check that all the fields are entered in the correct format");
                return false;
            }

            // ... check if this product is unique : Combination of Name and Brand defines a unique product
            if (!IsProductUnique(product))
            {
                string msg = "This product is not unique, combination of Name and Brand defines a unique product";
                _logger.LogError(msg);
                ModelState.AddModelError(product.Id.ToString(), msg);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Display a customised error message
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        private ErrorModel DisplayError(ModelStateDictionary modelState)
        {
            var errorModel = new ErrorModel
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = ReasonPhrases.GetReasonPhrase((int)HttpStatusCode.BadRequest),
                Errors = modelState.Values.SelectMany(x => x.Errors, (x,y) =>  y.ErrorMessage ).ToList()
            };

            return errorModel;
        }

        /// <summary>
        /// Deletes the product with the given id from the DB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.Product.FindAsync(id);
                if (product == null)
                {
                    _logger.LogError("Product Id does not exist, hence the product cannot be deleted from the DB");
                    ModelState.AddModelError("", "Product Id does not exist");
                    var errorModel = DisplayError(ModelState);
                    return new BadRequestObjectResult(errorModel);
                }

                _logger.LogInformation("Deleting product id: " + id + " from the DB ...");

                _context.Product.Remove(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product has been deleted from the DB");
            }
            catch (Exception e)
            {
                // ... handle any exceptions that might cause the API to crash : for instance trying to update the product Id
                var innerException = e.InnerException != null ? e.InnerException.Message : "";
                ModelState.AddModelError(e.Message, innerException);
                var errorModel = DisplayError(ModelState);

                // ... caution!!!
                _logger.LogError(innerException);
                return new BadRequestObjectResult(errorModel);
            }
            return NoContent();
        }

        /// <summary>
        /// Check if this product id exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }

        /// <summary>
        /// Check if this product is unique : Combination of Name and Brand defines a unique product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        private bool IsProductUnique(Product product)
        {
            var result = !_context.Product.Any(p => product.Name == p.Name && product.Brand == p.Brand);

            return result;
        }
    }
}
