using Microsoft.AspNetCore.Mvc.Filters;

namespace Infrastructure.Filters
{
    public class MovieShopHeader : ResultFilterAttribute
    {
        private readonly string _name;
        private readonly string _value;

        public MovieShopHeader(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Headers.Add(_name, new[] {_value});
            base.OnResultExecuting(context);
        }
    }
}