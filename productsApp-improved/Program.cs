using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using productsApp_improved.Data;
using productsApp_improved.Hubs;
using WebPush;

var builder = WebApplication.CreateBuilder(args);
//lang
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Add services to the container.
builder.Services.AddControllersWithViews()
 .AddViewLocalization(); //lang









//lang
// Program.cs
var supportedCultures = new[] { "en-US", "ar-SA" };



var localizationOptions = new RequestLocalizationOptions()
 .SetDefaultCulture("en-US")
 .AddSupportedCultures(supportedCultures)
 .AddSupportedUICultures(supportedCultures);









//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//add to admin 
builder.Services.AddDefaultIdentity<IdentityUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<AppDbContext>();

// for signalR to use email rather than userId
builder.Services.AddSingleton<IUserIdProvider, EmailBasedUserIdProvider>();

// add to signalR
builder.Services.AddSignalR();
// access denied page

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Home/AccessDenied";
});

// for audit logging (interceptor)
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    // Use your default SQL connection configurations profile string
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

    // Fetch the HttpContextAccessor service we just registered above
    var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

    // Attach the interceptor to your database transactions pipeline pipeline
    options.AddInterceptors(new AuditLoggingInterceptor(httpContextAccessor));
});


//sessions for cart
builder.Services.AddSession();
// Add services to the container.
//builder.Services.AddControllersWithViews();

// service workers push noti
builder.Services.Configure<VapidDetails>(
    builder.Configuration.GetSection("VAPID"));
var app = builder.Build();
//lang
app.UseRequestLocalization(localizationOptions);

// add to admin
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "Admin", "Customer","Vendor" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
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
app.UseStaticFiles();

app.UseRouting();

// sessions
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Home}/{id?}");

// add to signalR
app.MapHub<ChatHub>("/ChatHub");
app.MapHub<OrderNotiHub>("/OrderNotiHub"); 
app.Run();
