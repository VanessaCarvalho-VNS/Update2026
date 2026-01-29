using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using SalesWebMvc.Data;
using SalesWebMvc.Services;



var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<SalesWebMvcContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("SalesWebMVCContext"),
        ServerVersion.AutoDetect(
            builder.Configuration.GetConnectionString("SalesWebMVCContext")
        )
    )
);

// Services
builder.Services.AddScoped<SeedingService>();
builder.Services.AddScoped<SellerService>();
builder.Services.AddScoped<DepartmentService>();



builder.Services.AddControllersWithViews();

var app = builder.Build();

// 🔥 SEED AQUI
using (var scope = app.Services.CreateScope())
{
    var seedingService = scope.ServiceProvider
        .GetRequiredService<SeedingService>();

    seedingService.Seed();
}

// Configure the HTTP request pipeline.

var enUS = new CultureInfo("en-US");
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(enUS),
    SupportedCultures = new List<CultureInfo> { enUS },
    SupportedUICultures = new List<CultureInfo> { enUS }
};

app.UseRequestLocalization(localizationOptions);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
