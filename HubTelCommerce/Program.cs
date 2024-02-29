using HubTelCommerce.Configuration;
using HubTelCommerce.Data;
using HubTelCommerce.DbContexts;
using HubTelCommerce.Models;
using HubTelCommerce.Providers;
using HubTelCommerce.Repositories;
using HubTelCommerce.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Text;

namespace HubTelCommerce
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            var builder = WebApplication.CreateBuilder(args);
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] [{SourceContext}] {Message}{NewLine}{Exception}")
                .CreateBootstrapLogger();
            try
            {
                builder.Host.UseSerilog();
                // Add services to the container.
                ConfigureServices(builder.Services,builder.Configuration);
                var app = builder.Build();
                // Configure the HTTP request pipeline.
                Configure(app);
                app.Run();

            }
            catch(Exception ex)
            {
                string type = ex.GetType().Name;
                if (!type.Equals("StopTheHostException", StringComparison.Ordinal))
                {
                    Log.Fatal(ex, "Something serious happened, Failed to start.");
                }
            }
            finally
            {
                Log.CloseAndFlush();
            }
           
        }

        public static void ConfigureServices(IServiceCollection services,IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            services.AddSingleton(jwtOptions);
            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddMvc(op => op.EnableEndpointRouting = false);
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddScoped<ICustomerIdProvider,CustomerIdProvider>();
            services.AddScoped<ICartRepository,CartRepository>();
            Action<DbContextOptionsBuilder>? optionsAction = (options) => options.UseNpgsql(configuration.GetConnectionString("Database"));
            services.AddDbContext<CommerceDbContext>(optionsAction);
            services.AddDbContext<AuthDbContext>(optionsAction);
            services.AddIdentityCore<Customer>().AddEntityFrameworkStores<AuthDbContext>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => 
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
                });

                services.AddAuthorization();
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 1;

            });

        }

        public static void Configure(WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
              
                app.UseExceptionHandler("/Home/Error");
               
                app.UseHsts();
            }
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
           
            app.UseMvc();
            SeedData.Initialize(app.Services);
        }
    }
}