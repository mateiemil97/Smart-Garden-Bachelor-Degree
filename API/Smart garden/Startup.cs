using System;
using System.Reflection;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Smart_garden.Entites;
using Smart_garden.Repository;
using Smart_garden.Repository.BoardsKeyRepository;
using Smart_garden.Repository.FCMTokenRepository;
using Smart_garden.Repository.MeasurementRepository;
using Smart_garden.Repository.ScheduleRepository;
using Smart_garden.Repository.SensorPortRepository;
using Smart_garden.Repository.SensorRepository;
using Smart_garden.Repository.SystemRepository;
using Smart_garden.Repository.SystemStateRepository;
using Smart_garden.Repository.ZoneRepository;
using Smart_garden.UnitOfWork;

namespace Smart_garden
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //Inject settings

            // services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            var assembly = Assembly.Load("PushNotification");
            services.AddMvc().AddApplicationPart(assembly).AddControllersAsServices();
            services.AddHttpClient();
            services.AddDbContext<SmartGardenContext>(connection =>
                connection.UseSqlServer(Configuration.GetConnectionString("Connection")));

            var mappingConfig = new MapperConfiguration(a => {
                a.AddProfile(new MapperProfile());
            });

            services.AddDefaultIdentity<User>().AddEntityFrameworkStores<SmartGardenContext>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            });
            
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            // Intialize repositories
            services.AddScoped<IRepository<User>, Repository<User>>();
            
            services.AddScoped<IRepository<Entites.IrigationSystem>, Repository<Entites.IrigationSystem>>();
            services.AddScoped<IIrigationSystemRepository, IrigationSystemRepository>();

            services.AddScoped<IRepository<Sensor>, Repository<Sensor>>();
            services.AddScoped<ISensorRepository, SensorRepository>();
            services.AddScoped<ISystemStateRepository, SystemStateRepository>();
            services.AddScoped<IBoardsKeysRepository, BoardsKeysRepository>();
            services.AddScoped<IScheduleRepository, ScheduleRepository>();
            services.AddScoped<IZoneRepository,ZoneRepository>();
            services.AddScoped<ISensorPortRepository, SensorPortRepository>();
            services.AddScoped<IMeasurementRepository, MeasurementRepository>();
            services.AddScoped<IFCMTokenRepository, FCMTokenRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            services.AddCors(options =>
            {
                options.AddPolicy("cors",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            //Jwt Authentication

            var key = Encoding.UTF8.GetBytes(Configuration["ApplicationSettings:JWT_Secret"].ToString());

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x => {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseCors("cors");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
