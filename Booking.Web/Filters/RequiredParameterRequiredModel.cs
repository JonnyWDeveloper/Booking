using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Booking.Web.Filters
{
    public class RequiredParameterRequiredModel : ActionFilterAttribute
    {
        private string parameterName;
        public RequiredParameterRequiredModel(string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentException($"'{nameof(parameterName)}' cannot be null or whitespace.", nameof(parameterName));
            }

            this.parameterName = parameterName;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.RouteData.Values[parameterName] == null)
            {
                context.Result = new NotFoundResult();
            }
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if(context.Result is ViewResult viewResult)
            {
                //var r = (ViewResult)context.Result;
                //var rr = context.Result as ViewResult;
                if (viewResult.Model is null)
                {
                    context.Result = new NotFoundResult();
                }
            }
        }
    }
}
