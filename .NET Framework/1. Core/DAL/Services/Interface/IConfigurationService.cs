namespace DAL.Services.Interface
{
    public interface IConfigurationService: IDependency
    {
        T GetAppSettingsValue<T>(string key);

        string GetAppSettingsValue(string key);

        string GetConnectionString();

        string GetConnectionString(string key);

        bool HasKey(string key);

    }
}
