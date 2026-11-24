using System.Text.Json.Serialization;

namespace SharedCookbook.Application.Users.Commands.ExternalLogin;

[JsonConverter(typeof(JsonStringEnumConverter<ExternalLoginProvider>))]
public enum ExternalLoginProvider
{
    Google = 1,
    Apple = 2,
    Facebook = 3,
}
