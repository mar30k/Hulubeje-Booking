using HulubejeBooking.Models.HotelModels;
using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Controllers;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddHttpClient();
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("CnetHulubeje", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://api-hulubeje.cnetcommerce.com/api");
});
builder.Services.AddHttpClient("Payment", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://payment.cnetcommerce.com");
});
builder.Services.AddHttpClient("BusBooking", httpClient =>
{
    httpClient.BaseAddress = new Uri("http://196.189.126.8:8008/api/");
    httpClient.DefaultRequestHeaders.Add("x-api-key", "9BE090F9-7F52-4297-93A1-32D03D361DE9");
});
builder.Services.AddAuthentication("cnet.erp.v6")
     .AddCookie("cnet.erp.v6", options =>
     {
         options.AccessDeniedPath = "/account/denied";
         options.LoginPath = "/login";
     });
builder.Services.AddHttpClient("HulubejeBooking", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://v7-hulubeje.cnetcommerce.com/api/");
    httpClient.DefaultRequestHeaders.Add("x-api-key", "5D5EAFF4-D29A-485B-BDB9-785EF86FFFAE");
});
builder.Services.AddHttpClient("HulubejeCache", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://cache.cnetcommerce.com/api/cache/");
    httpClient.DefaultRequestHeaders.Add("x-api-key", "3e1a8b15-ygqa-3965-l5es-509a88f53477");
});
builder.Services.AddHttpClient("MovieDb", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://api.themoviedb.org/3/");
});
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<HotelListBuffer>();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthenticationManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();


app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
