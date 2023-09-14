using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using ToDoAndNotes2.Authorization;
using ToDoAndNotes2.Data;
using ToDoAndNotes2.Services;

var builder = WebApplication.CreateBuilder(args);

#region || Local. All secrets must be kept in User Secrets or appsettings.json
/*  Test connection string
    {
      "ConnectionStrings:ToDoAndNotes2": "Server=(localdb)\\\\mssqllocaldb;Database=ToDoAndNotes2_db;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
*/
#endregion
#region || To deploy (AzureKeyVault configuration). All secrets must be kept in a vault
//Environment.SetEnvironmentVariable("KEYVAULT_ENDPOINT", "https://todoandnotes2vault.vault.azure.net/");
//string? keyVaultEndpoint = Environment.GetEnvironmentVariable("KEYVAULT_ENDPOINT");
//var secretClient = new SecretClient(new(keyVaultEndpoint), new DefaultAzureCredential());
//builder.Configuration.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
#endregion

// DB
var connectionString = builder.Configuration.GetConnectionString("ToDoAndNotes2") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Google Authentication
builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    });

// Authorization handlers.
builder.Services.AddScoped<IAuthorizationHandler, IsFolderOwnerAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, IsTaskOwnerAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, IsNoteOwnerAuthorizationHandler>();

// EmailSender
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);

builder.Services.AddRazorPages();
builder.Services.ConfigureApplicationCookie(o =>
{
    o.ExpireTimeSpan = TimeSpan.FromDays(5);
    o.SlidingExpiration = true;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();