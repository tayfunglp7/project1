using OkulYonetim.Data;

//Uygulamayı kurmaya başla
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// "Bu bir MVC uygulaması, Controller ve View kullanacağım"
builder.Services.AddControllersWithViews();
// 👇 YENİ SATIR: Repository'yi sisteme tanıt
builder.Services.AddScoped<FakulteRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//http:// ile gelen isteği https://'e yönlendir (güvenlik)
app.UseHttpsRedirection();

//Yönlendirme kullanılacak
app.UseRouting();

//Login sistemi olacak
app.UseAuthorization();

//Varlıkların statik haritası olacak.
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Fakulte}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
