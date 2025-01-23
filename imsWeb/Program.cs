using imsWeb.Services.ProductRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using imsWeb.Data;
using CsvHelper;
using System.Globalization;
using imsWeb.Models;
using imsWeb.Services.OrderRepo;
using imsWeb.Support;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using CsvHelper.Configuration;
using System.IO;
using System.Linq;
using System;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddDbContext<ProductContext>(options =>
   // options.UseSqlite(builder.Configuration.GetConnectionString("ProductContext") ?? throw new InvalidOperationException("Connection string 'ProductContext' not found.")));

//add the database
builder.Services.AddDbContext<ProductContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        var dbPath = System.IO.Path.Join(path, "product.db");
        options.UseSqlite($"Data Source={dbPath}");
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
    else
    {
         var cs = builder.Configuration.GetConnectionString("ProductContext");
        options.UseSqlServer(cs, sqlServerOptionsAction: sqlOptions =>
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(6),
                errorNumbersToAdd: null
            )
        );
    }
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddHttpContextAccessor();

// Configure the HTTP request pipeline.
builder.Services.ConfigureSameSiteNoneCookies();

// Add session services
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true; // Necessary for GDPR compliance
        options.IdleTimeout = TimeSpan.FromMinutes(20); // Adjust timeout as needed
    });

    builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect("Auth0", options =>
{
    options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}";
    options.ClientId = builder.Configuration["Auth0:ClientId"];
    options.ClientSecret = builder.Configuration["Auth0:ClientSecret"];
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.SaveTokens = true; 

    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("read:products");

    options.CallbackPath = new PathString("/callback");
    options.ClaimsIssuer = "Auth0";

    options.SaveTokens = true;

    options.Events = new OpenIdConnectEvents
    {
        OnRedirectToIdentityProviderForSignOut = context =>
        {
            var logoutUri = $"https://{builder.Configuration["Auth0:Domain"]}/v2/logout?client_id={builder.Configuration["Auth0:ClientId"]}";
            context.Response.Redirect(logoutUri);
            context.HandleResponse();

            return Task.CompletedTask;
        }
    };
});  

var app = builder.Build();

if(builder.Environment.IsDevelopment()){

// Use this code to insert products into the database once
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ProductContext>();
    // Check if the database is empty, then insert data from CSV files
    if (!dbContext.Product.Any())
    {
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,  // Ignore missing fields
            HeaderValidated = null    // Ignore header validation issues
        };
        // Load suppliers into a dictionary for mapping
        var suppliers = dbContext.Suppliers.ToDictionary(s => s.Name!, s => s.SupplierId);
        // Load products from the CSV file and insert them into the database
        var csvFilePath = "wwwroot/csvFiles/products_data.csv";  // Adjust this path if necessary
        using (var reader = new StreamReader(csvFilePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {var productsFromCsv = csv.GetRecords<Product>().ToList();

            

            dbContext.Product.AddRange(productsFromCsv);
        }

        // Load categories from the CSV file and insert them into the database
        var categoryCsvFilePath = "wwwroot/csvFiles/category_data.csv";  // Adjust this path if necessary
        using (var reader = new StreamReader(categoryCsvFilePath))
        using (var csv = new CsvReader(reader, csvConfig))
        {
            var categories = csv.GetRecords<Category>().ToList();
            dbContext.Categories.AddRange(categories);
        }

        // Load suppliers from the CSV file and insert them into the database
        var supplierCsvFilePath = "wwwroot/csvFiles/supplier_data.csv";  // Adjust this path if necessary
        using (var reader = new StreamReader(supplierCsvFilePath))
        using (var csv = new CsvReader(reader, csvConfig))
        {
            var supplier = csv.GetRecords<Supplier>().ToList();
            dbContext.Suppliers.AddRange(supplier);
        }

        // Save changes to the database after all entities are added
        dbContext.SaveChanges();
    }

}

}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
