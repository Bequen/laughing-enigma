using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace Stag;

class Config {
    public static string ConfigPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "stag", "config.json");

    public String DatabaseUrl { get; set; } = String.Empty;
    public String Secret { get; set; } = String.Empty;
    public bool Initialize { get; set; } = true;

    public Config() {
        /* generate random secret */
        var bytes = RandomNumberGenerator.GetBytes(32);
        Secret = Convert.ToBase64String(bytes);
    }

    public static Config Load() {
        String path = ConfigPath;

        if(File.Exists(path)) {
            using(StreamReader reader = new StreamReader(path)) {
                Config? result = JsonSerializer.Deserialize<Config>(reader.ReadToEnd());
                return result ?? new Config();
            }
        }

        return new Config();
    }

    public void Save() {
        String path = ConfigPath;
        if(!Directory.Exists(Path.GetDirectoryName(path))) {
            Directory.CreateDirectory(path);
        }

        using(StreamWriter writer = new StreamWriter(path)) {
            writer.Write(JsonSerializer.Serialize(this));
        }
    }

    public SymmetricSecurityKey GetSecurityKey() {
        return new SymmetricSecurityKey(Convert.FromBase64String(Secret));
    }
}