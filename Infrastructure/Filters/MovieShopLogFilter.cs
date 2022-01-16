using System;
using ApplicationCore.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Filters
{
    public class MovieShopLogFilter : IActionFilter
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<MovieShopLogFilter> _logger;

        public MovieShopLogFilter(ICurrentUserService currentUserService, ILogger<MovieShopLogFilter> logger)
        {
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // context.HttpContext.Response.Headers.Add("ipaddress", _currentUserService.RemoteIpAddress);
            //var feature = context.HttpContext.Features.Get<IHttpConnectionFeature>();
            //var LocalIPAddr = feature?.LocalIpAddress?.ToString();
            var message = $"{_currentUserService.RemoteIpAddress} visited at {DateTime.UtcNow.ToLongTimeString()}";
            _logger.LogInformation(message);
            _logger.LogCritical(message);

            // Log Remote address
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}