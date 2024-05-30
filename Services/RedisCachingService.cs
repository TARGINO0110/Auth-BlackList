using Auth_BlackList.Model;
using Auth_BlackList.Services.Interfaces;
using Auth_BlackList.Utils;
using Microsoft.Extensions.Caching.Distributed;

namespace Auth_BlackList.Services
{
    public class RedisCachingService(ILogger<RedisCachingService> logger, IDistributedCache cache) : IRedisCachingService
    {
        private readonly ILogger<RedisCachingService> _logger = logger;
        private readonly IDistributedCache _cache = cache;

        public async Task<string> GetCachingAsync(string key)
        {
            _logger.LogInformation($"[{DateTime.Now:G}] Get Key: {key}");
            return await _cache.GetStringAsync(key);
        }

        public async Task SetCachingAsync(string key, string value)
        {
            _logger.LogInformation($"[{DateTime.Now:G}] Set Key: {key}, Value: {value}");
            await _cache.SetStringAsync(key, value, ResetOptionsCache());
        }

        public async Task RemoveCachingAsync(string key)
        {
            _logger.LogInformation($"[{DateTime.Now:G}] Remove Key: {key}");
            await _cache.RemoveAsync(key);
        }

        public async Task ValidBlackListIpAsync(string ipAddress)
        {
            try
            {
                string infoIp = await GetCachingAsync(ipAddress);

                if (string.IsNullOrEmpty(infoIp))
                {
                    await SetCachingAsync(
                        ipAddress,
                        new DeserializeJson().SerializeObjectJson(
                            new SecurityIpModel
                            {
                                Id = Guid.NewGuid(),
                                IpAddress = ipAddress,
                                DateBlockIp = DateTime.Now,
                                CountForbiddenAccess = 1,
                                BlockAccessApi = BlockTypeAccess.BlockOn
                            }
                        ));
                }
                else
                {
                    SecurityIpModel securityIpModel = new DeserializeJson().DeserializeObjectJson(infoIp);
                    switch (securityIpModel.CountForbiddenAccess)
                    {
                        case 1:
                            securityIpModel.DateBlockIp = DateTime.Now;
                            securityIpModel.CountForbiddenAccess = 2;
                            await SetCachingAsync(ipAddress, new DeserializeJson().SerializeObjectJson(securityIpModel));
                            break;
                        case 2:
                            securityIpModel.DateBlockIp = DateTime.Now;
                            securityIpModel.CountForbiddenAccess = 3;
                            await SetCachingAsync(ipAddress, new DeserializeJson().SerializeObjectJson(securityIpModel));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"[{DateTime.Now:G}] Error inesperado no redis ... {ex.Message}");
                throw;
            }
        }

        private static DistributedCacheEntryOptions ResetOptionsCache()
        {
            DistributedCacheEntryOptions options = new()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(3600),
                SlidingExpiration = TimeSpan.FromSeconds(1200)
            };

            return options;
        }
    }
}
