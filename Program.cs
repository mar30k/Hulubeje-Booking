using HulubejeBooking.Models.HotelModels;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddHttpClient();
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("HotelBooking", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://api-hulubeje.cnetcommerce.com/api");
});
builder.Services.AddHttpClient("BusBooking", httpClient =>
{
    httpClient.BaseAddress = new Uri("http://192.168.1.25:8092/api/");
    httpClient.DefaultRequestHeaders.Add("x-api-key", "9BE090F9-7F52-4297-93A1-32D03D361DE9");
});
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<HotelListBuffer>();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

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
