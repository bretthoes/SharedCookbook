using System.Security.Cryptography;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Common.Models;

namespace SharedCookbook.Infrastructure.Identity;

public class ExternalLoginService : IExternalLoginService
{
    private const string GoogleLoginProvider = "Google";
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly string _googleClientId;

    public ExternalLoginService(
        UserManager<ApplicationUser> userManager,
        IOptions<GoogleAuthOptions> options)
    {
        _userManager = userManager;
        _googleClientId = options.Value.ClientId
            ?? throw new InvalidOperationException("Authentication:Google:ClientId must be configured.");
    }

    public async Task<Result<string>> LoginWithGoogleAsync(string idToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(idToken))
            return Result<string>.Failure(["ID token is required."]);

        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(
                idToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [_googleClientId],
                });
        }
        catch (InvalidJwtException)
        {
            return Result<string>.Failure(["Invalid or expired Google token."]);
        }

        var email = payload.Email;
        var subject = payload.Subject;

        if (string.IsNullOrEmpty(email))
            return Result<string>.Failure(["Google token did not contain email."]);

        var user = await _userManager.FindByLoginAsync(GoogleLoginProvider, subject);
        if (user != null)
            return Result<string>.Success(user.Id);

        user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            var addLoginResult = await _userManager.AddLoginAsync(
                user,
                new UserLoginInfo(GoogleLoginProvider, subject, GoogleLoginProvider));
            if (!addLoginResult.Succeeded)
                return Result<string>.Failure(addLoginResult.Errors.Select(e => e.Description).ToArray());
            return Result<string>.Success(user.Id);
        }

        user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
        };

        var password = GenerateSecureRandomPassword();
        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
            return Result<string>.Failure(createResult.Errors.Select(e => e.Description).ToArray());

        var linkResult = await _userManager.AddLoginAsync(
            user,
            new UserLoginInfo(GoogleLoginProvider, subject, GoogleLoginProvider));
        if (!linkResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            return Result<string>.Failure(linkResult.Errors.Select(e => e.Description).ToArray());
        }

        return Result<string>.Success(user.Id);
    }

    private static string GenerateSecureRandomPassword()
    {
        const int length = 32;
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
        var bytes = new byte[length * 4];
        RandomNumberGenerator.Fill(bytes);
        var result = new char[length];
        for (var i = 0; i < length; i++)
            result[i] = validChars[(int)(BitConverter.ToUInt32(bytes, i * 4) % validChars.Length)];
        return new string(result);
    }
}
