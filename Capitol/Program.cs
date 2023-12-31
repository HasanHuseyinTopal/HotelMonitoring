using DataAccessLayer.Abstract;
using DataAccessLayer.Concrate;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddScoped<IUnitOfWorkDal, UnitOfWorkDal>();


builder.Services.AddDbContext<CpContext>(context => context.UseSqlServer(builder.Configuration.GetConnectionString("ConStr"), builder => builder.EnableRetryOnFailure()));

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<CpContext>().AddDefaultTokenProviders();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization(opt => opt.AddPolicy("ADMN", pol =>
{
    pol.RequireRole("ADMN");
}));
builder.Services.Configure<IdentityOptions>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireDigit = false;
    opt.Password.RequiredLength = 3;
    opt.Password.RequireDigit = false;
});
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

app.UseExceptionHandler("/Error");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute("Default", "{controller=Visitor}/{action=DailyVisitors}/{id?}");

app.Run();
