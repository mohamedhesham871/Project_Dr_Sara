
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Project_Dr_Sara.DbContexts;
using Project_Dr_Sara.Dtos;
using Project_Dr_Sara.Models;
using Project_Dr_Sara.Repo;
using Project_Dr_Sara.Repo.IRepo;
using Project_Dr_Sara.Services;
using Project_Dr_Sara.Services.IServices;
using System.Text;

namespace Project_Dr_Sara
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddSwaggerGen();
            builder.Services.AddEndpointsApiExplorer();


            //Add DbContext and Identity services here
            builder.Services.AddDbContext<DbContexts.AppDbContexts>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
           
            //Register Identity
            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
            })
          .AddEntityFrameworkStores<AppDbContexts>()
          .AddDefaultTokenProviders();

            // Make Token Valid for 1 Hour When Forget Password
            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(1);
            });
            //Token
            var JWTtoken = builder.Configuration.GetSection("JWT").Get<JWT>();
            //Add Authentication and set Configuraions 
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(Options =>
                 Options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,


                     ValidIssuer = JWTtoken?.Issuer,
                     ValidAudience = JWTtoken?.Audience,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTtoken?.SecretKey))
                 });
            builder.Services.AddAuthorization();//VIP to Authorize work

            //Add Register 
            builder.Services.AddScoped<IAuthService, AuthenticationServices>();
            builder.Services.AddScoped<IPatientService, PatientService>();
            builder.Services.AddScoped<IDoctorService, DoctorService>();
            builder.Services.AddScoped<IAppointment, AppointmentService>();
            builder.Services.AddScoped<Ispecializtion, SpecializationServices>();
            builder.Services.AddScoped<IAppointmentRepo, AppointmentRepo>();
            builder.Services.AddScoped<IPatientRepo, PatientRepo>();
            builder.Services.AddScoped<IDoctorRepo, DoctorRepo>();
            builder.Services.AddScoped<ISpecilizationRepo, SpcilaizationRepo>();




            builder.Services.AddCors(options=>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors("AllowAll");

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();
         

            app.Run();
        }
    }
}
