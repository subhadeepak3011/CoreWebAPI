using Microsoft.AspNetCore.Mvc.ModelBinding;
using static Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary;

namespace GHDWebAPI.Model
{
    /// <summary>
    /// Class to display customised error messages
    /// </summary>
    public class ErrorModel
    {
        /// <summary>
        /// Initialise
        /// </summary>
        public ErrorModel()
        {
            Title = string.Empty;
            Errors = [];
        }

        /// <summary>
        /// Title of the error
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Status code of the error
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// List of descriptive error messages
        /// </summary>
        public List<string>  Errors { get; set; }

    }
}
