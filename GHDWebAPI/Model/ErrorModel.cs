using Microsoft.AspNetCore.Mvc.ModelBinding;
using static Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary;

namespace GHDWebAPI.Model
{
    public class ErrorModel
    {
        public ErrorModel()
        {
            
        }

        public string Title { get; set; }

        public int Status { get; set; }

        public List<string>  Errors { get; set; }

    }
}
