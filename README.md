
# To Do & Notes 2
## Technology stack that was used
ASP.NET Core, Razor Pages, EF, ASP.NET Core Identity, SendGrid, Google OAuth 2.0, Secret Manager tool, Azure Key Vault, Dependency injection and Options patterns, C#, Bootstrap, HTML, CSS, JS.
### Frameworks
 > [ASP.NET Core](https://learn.microsoft.com/uk-ua/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-3.1) is a cross-platform, high-performance, [open-source](https://github.com/dotnet/aspnetcore) framework for building modern, cloud-enabled, Internet-connected apps.

> [Razor Pages](https://learn.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-7.0&tabs=visual-studio) server-side, page-focused framework that enables building dynamic, data-driven web sites with clean separation of concerns. It can makes coding page-focused scenarios easier and more productive than using controllers and views.

 > [Entity Framework (EF) Core](https://learn.microsoft.com/en-us/ef/core/) is a lightweight, extensible, [open source](https://github.com/dotnet/efcore) and cross-platform version of the popular Entity Framework data access technology.
 
 > [Bootstrap](https://getbootstrap.com/) is a free and [open-source](https://github.com/twbs/bootstrap/tree/v5.3.1) CSS framework directed at responsive, mobile-first front-end web development. It contains HTML, CSS and (optionally) JavaScript-based design templates for typography, forms, buttons, navigation, and other interface components.

### API
 > [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-7.0&tabs=visual-studio) is an API that supports user interface (UI) login functionality, manages users, passwords, profile data, roles, claims, tokens, email confirmation, and more.
```C#
Activator.CreateInstance<IdentityUser>();
var result = await _userManager.CreateAsync(user, Input.Password);
```

 > [SendGrid](https://sendgrid.com/) is a cloud-based SMTP provider that allows you to send email without having to maintain email servers.
```C#
var client = new SendGridClient(apiKey);
var response = await client.SendEmailAsync(msg);
```
> [Google OAuth 2.0](https://developers.google.com/identity/protocols/oauth2) -  Google APIs use the [OAuth 2.0 protocol](https://tools.ietf.org/html/rfc6749) for authentication and authorization. Google supports common OAuth 2.0 scenarios such as those for web server, client-side, installed, and limited-input device applications. [ASP.NET Core](https://learn.microsoft.com/uk-ua/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-3.1) contains [middleware](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.Google) to support Google's OpenId and OAuth 2.0 authentication workflows.
```C#
builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    });
```
```C#
var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
```
### Security
> [Secret Manager tool](The%20Secret%20Manager%20tool%20stores%20sensitive%20data%20during%20the%20development%20of%20an%20ASP.NET%20Core%20project.%20In%20this%20context,%20a%20piece%20of%20sensitive%20data%20is%20an%20app%20secret.%20App%20secrets%20are%20stored%20in%20a%20separate%20location%20from%20the%20project%20tree.) stores sensitive data during the development of an ASP.NET Core project. In this context, a piece of sensitive data is an app secret. App secrets are stored in a separate location from the project tree.
```C#
dotnet user-secrets set "SendGridKey" "******"
```
> [Azure Key Vault](https://learn.microsoft.com/en-us/azure/key-vault/general/basic-concepts) is a cloud service for securely storing and accessing secrets. A secret is anything that you want to tightly control access to, such as API keys, passwords, certificates, or cryptographic keys.
```C#
var secretClient = new SecretClient(new(keyVaultEndpoint), new DefaultAzureCredential())
```
> [User data protected by authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/secure-data?view=aspnetcore-7.0)  (IAuthorizationService + IAuthorizationHandler)
```C#
var isAuthorized = await AuthorizationService.AuthorizeAsync(User, note, UserOperations.FullAccess);
```
### Patterns
- Dependency injection
- Options
> setting options
```C#
public class AuthMessageSenderOptions
{
    public string? SendGridKey { get; set; }
    public string? SendGridFromEmail { get; set; }
}
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);
```
> getting options with DI
```
public AuthMessageSenderOptions Options { get; }
public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor,
                        ILogger<EmailSender> logger)
    {
        Options = optionsAccessor.Value;
        _logger = logger;
    }
```
> using options
```
await Execute(Options.SendGridKey, subject, message, toEmail);
```
## Screenshots
### User login
![Login][1]
### The application sent a confirmation request to the user's email and the user received it
![ConfirmEmail][2]
### Main full view
![IndexFull][3]
### Main folder view
![IndexByFolder][4]
### Main folder view (mobile)
![IndexByFolder(mobile)][5]
### Task edit
![TasksEdit][6]
### Bin
![Bin][7]
### Account managment
![Account][8]

[1]: Screenshots/Login.png "Login"
[2]: Screenshots/ConfirmEmail.png "ConfirmEmail"
[3]: Screenshots/IndexFull.png "IndexFull"
[4]: Screenshots/IndexByFolder.png "IndexByFolder"
[5]: Screenshots/IndexByFolder(mobile).png "IndexByFolder(mobile)"
[6]: Screenshots/TasksEdit.png "TasksEdit"
[7]: Screenshots/Bin.png "Bin"
[8]: Screenshots/Account.png "Account"
