    using LibraryManagementSystem.Data;
    using LibraryManagementSystem.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using QuestPDF.Infrastructure;


    var builder = WebApplication.CreateBuilder(args);
    //DPF in a Excel .....

    QuestPDF.Settings.License = LicenseType.Community;


    // Add services to the container.
    builder.Services.AddControllersWithViews();
    builder.Services.ConfigureApplicationCookie( options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });


    //Database connection---------------------Strat

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<ApplicationUser,IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            /*
                "DefaultConnection": "Server=DESKTOP-UIPEBPH\\SQLEXPRESS;Database=LibraryManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
            */
    //Database connection---------------------End


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseRouting();

    app.UseAuthentication(); // Login jnny add 
    app.UseAuthorization();

    app.MapStaticAssets();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();


    app.Run();
