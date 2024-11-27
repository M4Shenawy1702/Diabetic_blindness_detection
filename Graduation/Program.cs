using CheckEyePro.Core.Mapping;
using CheckEyePro.Core.Interfaces;
using CheckEyePro.Core.IServices;
using CheckEyePro.Core.Models;
using CheckEyePro.Core.Settingse;
using CheckEyePro.EF.DBContext;
using CheckEyePro.EF.Repositories;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderManagementSystem.Core.IRepository;
using OrderManagementSystem.EF.Repository;
using OrderManagementSystem.EF.Services;
using Stripe;
using Stripe.Checkout;
using System.Text;
using CheckEyePro.EF.Services;
using CheckEyePro.Core.Middleware;

namespace CheckEyePro.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                 options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                    );
            // Add services to the container.
            TypeAdapterConfig.GlobalSettings.Scan(typeof(MappingConfig).Assembly);

            builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));


            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddScoped<IPrediction, PredictionService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IAuthInterface, AuthRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            //builder.Services.AddSingleton<GlobalExceptionHandlerMiddleware>();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            // Configure Stripe
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            builder.Services.AddScoped<IJWTTokenGenerator, JWTTokenGenerator>();
            builder.Services.AddScoped<SessionService>();
            builder.Services.AddScoped<PaymentIntentService>();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddCors();
            builder.Services.AddRouting(options => options.LowercaseUrls = true);
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
