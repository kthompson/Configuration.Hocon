using System;
using System.IO;
using Configuration.Hocon;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Configuration;

public static class HoconConfigurationExtensions
{
    public static IConfigurationBuilder AddHoconFile(this IConfigurationBuilder builder, string path)
    {
        return AddHoconFile(builder, provider: null, path: path, optional: false, reloadOnChange: false);
    }

    public static IConfigurationBuilder AddHoconFile(this IConfigurationBuilder builder, string path, bool optional)
    {
        return AddHoconFile(builder, provider: null, path: path, optional: optional, reloadOnChange: false);
    }

    public static IConfigurationBuilder AddHoconFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange)
    {
        return AddHoconFile(builder, provider: null, path: path, optional: optional, reloadOnChange: reloadOnChange);
    }

    public static IConfigurationBuilder AddHoconFile(this IConfigurationBuilder builder, IFileProvider provider, string path, bool optional, bool reloadOnChange)
    {
        if (provider == null && Path.IsPathRooted(path))
        {
            provider = new PhysicalFileProvider(Path.GetDirectoryName(path));
            path = Path.GetFileName(path);
        }
        var source = new HoconConfigurationSource
        {
            FileProvider = provider,
            Path = path,
            Optional = optional,
            ReloadOnChange = reloadOnChange
        };
        builder.Add(source);
        return builder;
    }
}