using Auth_BlackList.Model;
using Auth_BlackList.Services.Interfaces;
using Auth_BlackList.Utils;

namespace Auth_BlackList.Security
{
    public class ClientRequestIpFilterMiddleware(ILogger<ClientRequestIpFilterMiddleware> logger, IRedisCachingService redisCachingService, RequestDelegate next)
    {
        private readonly ILogger<ClientRequestIpFilterMiddleware> _logger = logger;
        private readonly IRedisCachingService _redisCachingService = redisCachingService;
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                ConnectionInfo infoConnection = context.Connection;
                if (infoConnection is null)
                {
                    _logger.LogInformation("Requisição negada, informação de conexão obrigatoria ...");
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Forbidden");
                }

                _logger.LogDebug("Info Remote IpAddress: {RemoteIpAddress}", infoConnection.RemoteIpAddress);

                var infoIp = await _redisCachingService.GetCachingAsync(infoConnection.RemoteIpAddress.ToString());

                if (!string.IsNullOrEmpty(infoIp))
                {
                    DateTime timeExpire;
                    SecurityIpModel securityIpModel = new DeserializeJson().DeserializeObjectJson(infoIp);

                    switch (securityIpModel.CountForbiddenAccess)
                    {
                        case 1:
                            timeExpire = securityIpModel.DateBlockIp.AddMinutes(30);
                            if (timeExpire >= DateTime.Now)
                            {
                                _logger.LogWarning($"[{DateTime.Now:G}] Forbidden Request from IP: BLOCKED-STAGE 1 {infoConnection.RemoteIpAddress} - Expire: {timeExpire:G}");
                                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            }
                            break;
                        case 2:
                            timeExpire = securityIpModel.DateBlockIp.AddMinutes(60);
                            if (timeExpire >= DateTime.Now)
                            {
                                _logger.LogWarning($"[{DateTime.Now:G}] Forbidden Request from IP: BLOCKED-STAGE 2 {infoConnection.RemoteIpAddress} - Expire: {timeExpire:G}");
                                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            }
                            break;
                        case 3:
                            timeExpire = securityIpModel.DateBlockIp.AddMinutes(120);
                            if (timeExpire >= DateTime.Now)
                            {
                                _logger.LogWarning($"[{DateTime.Now:G}] Forbidden Request from IP: BLOCKED-STAGE 3 {infoConnection.RemoteIpAddress} - Expire: {timeExpire:G}");
                                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            }
                            break;
                        default:
                            _logger.LogWarning($"Forbidden Request from IP: BLOCKED-PERMANENT {infoConnection.RemoteIpAddress}");
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            break;
                    }
                    return;
                }

                await _next(context);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
