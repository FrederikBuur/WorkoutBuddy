using System.ComponentModel;
using System.Text.Json.Serialization;

namespace WorkoutBuddy.Authentication;

public class LoginInfo
{
    [JsonPropertyName("username")]
    public string Username { get; set; } = "";

    [JsonPropertyName("password")]
    public string Password { get; set; } = "";

}
