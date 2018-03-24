using Microsoft.Extensions.Configuration;

namespace Configuration.Hocon
{
    public class HoconConfigurationSource : FileConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            FileProvider = FileProvider ?? builder.GetFileProvider();
            return new HoconConfigurationProvider(this);
        }
    }
}