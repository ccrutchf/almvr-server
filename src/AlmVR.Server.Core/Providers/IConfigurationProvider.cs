using System.Threading.Tasks;

namespace AlmVR.Server.Core.Providers
{
    public interface IConfigurationProvider
    {
        Task<T> GetConfigurationAsync<T>()
            where T : class;
        Task SetConfigurationAsync<T>()
            where T : class;
    }
}
