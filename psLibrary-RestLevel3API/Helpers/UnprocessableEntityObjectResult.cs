using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace psLibrary_RestLevel3API.Helpers
{
    public class UnprocessableEntityObjectResult : ObjectResult
    {
        //public UnprocessableEntityObjectResult(object error): base(error) // this will return all ModelState props
        //{
        //    StatusCode = 422;

        //}

        public UnprocessableEntityObjectResult(ModelStateDictionary modelState) : base(new SerializableError(modelState))
        {
            if (modelState == null)
            {
                throw new ArgumentNullException(nameof(modelState));
            }

            StatusCode = 422;
        }
    }
}
