using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Smart_garden.Entites;
using Smart_garden.Repository;
using Smart_garden.Repository.SensorRepository;
using Smart_garden.Repository.SystemRepository;
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddDbContext<SmartGardenContext>(connection =>
                connection.UseSqlServer(Configuration.GetConnectionString("Connection")));

            var mappingConfig = new MapperConfiguration(a => {
                a.AddProfile(new MapperProfile());
            });

            services.AddDefaultIdentity<User>().AddEntityFrameworkStores<SmartGardenContext>();

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            // Intialize repositories
            services.AddScoped<IRepository<User>, Repository<User>>();
            
            services.AddScoped<IRepository<Entites.IrigationSystem>, Repository<Entites.IrigationSystem>>();
            services.AddScoped<IIrigationSystemRepository, IrigationSystemRepository>();

            services.AddScoped<IRepository<Sensor>, Repository<Sensor>>();
            services.AddScoped<ISensorRepository, SensorRepository>();


            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();



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

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
