global using Microsoft.EntityFrameworkCore;
global using webappproject.Data;
global using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using webappproject.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<Context>(options =>
options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add repository and services
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<RolService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<BanService>();

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.Name = "NetCoreMvc.Auth";
    options.LoginPath = "/Login/Index";
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed database with roles and admin user if they don't exist
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<Context>();
    context.Database.EnsureCreated();
    
    // Add default roles if they don't exist
    if (!context.Rols.Any())
    {
        context.Rols.AddRange(
            new webappproject.Models.Rol { Name = "Admin" },
            new webappproject.Models.Rol { Name = "User" },
            new webappproject.Models.Rol { Name = "Premium" }
        );
        context.SaveChanges();
    }
    
    // Add admin user if doesn't exist
    if (!context.Users.Any(u => u.Email == "admin@loveconnect.com"))
    {
        var adminRole = context.Rols.FirstOrDefault(r => r.Name == "Admin");
        if (adminRole != null)
        {
            var adminUser = new webappproject.Models.User
            {
                Name = "Admin",
                Surname = "LoveConnect",
                Email = "admin@loveconnect.com",
                Password = "Admin123!", // Strong password for demo
                Gender = "Other",
                RolId = adminRole.Id,
                Biography = "System Administrator - LoveConnect Dating Platform",
                Tag1 = "Security",
                Tag2 = "Management",
                Age = 30,
                FirstLogIn = false,
                IsActive = true,
                CreatedDate = DateTime.Now,
                LastLoginDate = DateTime.Now,
                ProfileUrl = "admin-profile"
            };
            context.Users.Add(adminUser);
            context.SaveChanges();
            
            Console.WriteLine("=== ADMIN CREDENTIALS CREATED ===");
            Console.WriteLine("Email: admin@loveconnect.com");
            Console.WriteLine("Password: Admin123!");
            Console.WriteLine("=====================================");
        }
    }
    
    // Add some demo users if database is empty
    if (context.Users.Count() <= 1) // Only admin exists
    {
        var userRole = context.Rols.FirstOrDefault(r => r.Name == "User");
        var premiumRole = context.Rols.FirstOrDefault(r => r.Name == "Premium");
        if (userRole != null)
        {
            var names = new[] { "Emma", "Olivia", "Sophia", "Isabella", "Charlotte", "Amelia", "Mia", "Harper", "Evelyn", "Abigail", 
                               "James", "William", "Benjamin", "Lucas", "Henry", "Alexander", "Mason", "Michael", "Ethan", "Daniel" };
            var surnames = new[] { "Wilson", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", 
                                  "Anderson", "Taylor", "Thomas", "Hernandez", "Moore", "Martin", "Jackson", "Thompson", "White", "Lopez" };
            var bios = new[] { 
                "Love traveling and exploring new cultures! 🌍",
                "Coffee enthusiast and book lover ☕📚", 
                "Adventure seeker and nature lover 🏔️",
                "Passionate about art and photography 📸",
                "Fitness enthusiast and yoga instructor 🧘‍♀️",
                "Foodie and cooking enthusiast 👩‍🍳",
                "Music lover and guitar player 🎸",
                "Dog lover and volunteer 🐕",
                "Science nerd and stargazer 🔬⭐",
                "Entrepreneur and tech enthusiast 💻",
                "Beach lover and surfer 🏄‍♂️",
                "Movie buff and popcorn enthusiast 🍿",
                "Hiker and outdoor adventurer 🥾",
                "Artist and creative soul 🎨",
                "Chef and flavor explorer 👨‍🍳"
            };
            
            var tags1 = new[] { "Travel", "Fitness", "Art", "Music", "Books", "Technology", "Cooking", "Photography", "Sports", "Movies" };
            var tags2 = new[] { "Coffee", "Adventure", "Yoga", "Dancing", "Gaming", "Nature", "Fashion", "Wine", "Hiking", "Pets" };
            var genders = new[] { "Male", "Female" };
            var emojis = new[] { "😊", "🌟", "💃", "🎨", "🏃‍♀️", "📖", "🎵", "🌺", "✨", "🦋", "🔥", "💫", "🌈", "⚡", "🎭" };
            
            var demoUsers = new List<webappproject.Models.User>();
            var random = new Random();
            
            // Create 15 demo users for better matches/discover experience
            for (int i = 0; i < 15; i++)
            {
                var user = new webappproject.Models.User
                {
                    Name = names[i % names.Length],
                    Surname = surnames[i % surnames.Length],
                    Email = $"demo{i + 1}@loveconnect.com",
                    Password = "demo123",
                    Gender = genders[i % genders.Length],
                    RolId = (i % 4 == 0 && premiumRole != null) ? premiumRole.Id : userRole.Id, // Every 4th user is premium
                    Biography = bios[i % bios.Length],
                    Tag1 = tags1[i % tags1.Length],
                    Tag2 = tags2[i % tags2.Length],
                    Age = random.Next(22, 35),
                    FirstLogIn = false,
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-random.Next(1, 365)),
                    LastLoginDate = DateTime.Now.AddDays(-random.Next(1, 7)),
                    ProfileUrl = $"demo-user-{i + 1}",
                    ImagePath = "", // Remove profile images
                    SliderValue1 = random.Next(18, 25),
                    SliderValue2 = random.Next(25, 40)
                };
                demoUsers.Add(user);
            }
            
            context.Users.AddRange(demoUsers);
            context.SaveChanges();
            
            Console.WriteLine($"=== {demoUsers.Count} DEMO USERS CREATED ===");
            Console.WriteLine("Demo users for testing Matches/Discover pages created successfully!");
            Console.WriteLine("=====================================");
        }
    }
}

app.Run();
