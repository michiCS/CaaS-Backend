using Microsoft.AspNetCore.Mvc.Filters;

namespace CaaS.Api.Filters
{
    public abstract class ParameterizedFilter : IAsyncActionFilter, IOrderedFilter
    {
        protected readonly string propertyString;

        public int Order { get; set; }

        public ParameterizedFilter(string propertyString)
        {
            this.propertyString = propertyString;
        }

        public abstract Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next);
    }
}
