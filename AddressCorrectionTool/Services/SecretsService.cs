public class SecretsService
{
    private readonly IConfiguration _configuration;

    public SecretsService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetSecret(string key)
    {
        return _configuration[key];
    }
}