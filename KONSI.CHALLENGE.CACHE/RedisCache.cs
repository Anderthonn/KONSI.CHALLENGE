using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace KONSI.CHALLENGE.CACHE
{
    public class RedisCache : IRedisCache
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IConfiguration _configuration;
        public RedisCache(IConfiguration configuration)
        {
            _configuration = configuration;
            _redis = ConnectionMultiplexer.Connect(_configuration.GetSection("RedisConnection").GetSection("RedisAddress").Value ?? "");
        }

        public string GetJsonData(string cpf)
        {
            try
            {
                IDatabase db = _redis.GetDatabase();
                RedisValue jsonData = db.StringGet(cpf);

                return jsonData.HasValue ? jsonData.ToString() : "";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void SetJsonData(string cpf, string jsonData)
        {
            try
            {
                IDatabase db = _redis.GetDatabase();
                db.StringSet(cpf, jsonData, TimeSpan.FromHours(1));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}