using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Services;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllersWithViews();

            var connString = builder.Configuration.GetConnectionString("Default");
            builder.Services.AddDbContext<BookDbContext>(opt => opt.UseSqlServer(connString));
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped<IBCoverStorage, BCoverStorage>();

            var app = builder.Build();


            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Books}/{action=Index}/{id?}")
                .WithStaticAssets();

            //if (app.Environment.IsDevelopment())
            //{
            //    using var scope = app.Services.CreateScope();
            //    var db = scope.ServiceProvider.GetRequiredService<BookDbContext>();
            //    await DbSeeder.SeedAsync(db);
            //}

            app.Run();
        }
    }
}
