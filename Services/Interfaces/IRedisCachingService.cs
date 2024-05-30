namespace Auth_BlackList.Services.Interfaces
{
    public interface IRedisCachingService
    {
        Task<string> GetCachingAsync(string key);
        Task SetCachingAsync(string key, string value);
        Task RemoveCachingAsync(string key);
        Task ValidBlackListIpAsync(string ipAddress);
    }
}
