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
using GHDWebAPI.Services;

namespace GHDWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        #region INITIALISE

        /// <summary>
        /// readonly reference to the IProductService
        /// </summary>
        private readonly IProductService _service;

        /// <summary>
        /// readonly reference to the ILogger
        /// </summary>
        private readonly ILogger _logger;   
        public ProductsController(IProductService service, ILogger logger)
        {
            _service = service;
            _logger = logger;
         
        }

        #endregion

        #region READ

        /// <summary>
        /// Gets all the products from the DB
        /// </summary>
        /// <returns></returns>
        // GET: api/Products
        [HttpGet]
        public IEnumerable<Product> GetAllProducts()
        {
            _logger.LogInformation("Fetching all products...");
            return  _service.GetAll();
        }

        /// <summary>
        /// Gets the product with the given id from the DB
        /// </summary>
        /// <param name="id">product id</param>
        /// <returns></returns>
        // GET: api/Products/5
        [HttpGet("{id}")]
        public ActionResult<Product> GetProduct(int id)
        {
            _logger.LogInformation("Fetching the product with id: " + id);
            var product = _service.GetById(id);

            if (product == null)
            {
                _logger.LogError("Product Id: " + id + " could not be found");
                ModelState.AddModelError("", "Product Id does not exist");
                var errorModel = DisplayError(ModelState);
                return new BadRequestObjectResult(errorModel);
            }

            return product;
        }

        #endregion

        #region CREATE

        /// <summary>
        /// Creates a new product and adds it to the DB
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ActionResult<Product> PostProduct(Product product)
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

                _service.Add(product);

                _logger.LogInformation("Product has been created and added to the DB");

                return CreatedAtAction("GetProduct", new { id = product.Id }, product);

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

        }

        #endregion

        #region UPDATE

        /// <summary>
        /// Updates the product with the given id in the DB
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public ActionResult<Product> PutProduct(int id, Product product)
        {
            try
            {
                // ... check if product is null
                if (product == null)
                {
                    _logger.LogError("Product is null as it does not have valid fields");
                    ModelState.AddModelError("", "Please check that all the fields are entered in the correct format");
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
                _service.Update(product);

                _logger.LogInformation("Product has been updated to the DB");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_service.ProductExists(id))
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

        #endregion

        #region DELETE

        /// <summary>
        /// Deletes the product with the given id from the DB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public ActionResult DeleteProduct(int id)
        {
            try
            {
                var product = _service.GetById(id);
                if (product == null)
                {
                    _logger.LogError("Product Id does not exist, hence the product cannot be deleted from the DB");
                    ModelState.AddModelError("", "Product Id does not exist");
                    var errorModel = DisplayError(ModelState);
                    return new BadRequestObjectResult(errorModel);
                }

                _logger.LogInformation("Deleting product id: " + id + " from the DB ...");

               _service.DeleteById(product.Id);

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

        #endregion

        #region PRIVATE METHODS

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
            if (!_service.IsProductUnique(product))
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
                Errors = modelState.Values.SelectMany(x => x.Errors, (x, y) => y.ErrorMessage).ToList()
            };

            return errorModel;
        }
    }

    #endregion
}
