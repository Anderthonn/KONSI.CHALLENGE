namespace KONSI.CHALLENGE.CACHE
{
    public interface IRedisCache
    {
        string GetJsonData(string cpf);
        void SetJsonData(string cpf, string jsonData);

    }
}