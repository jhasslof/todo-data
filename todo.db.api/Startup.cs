using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace todo.db.api
{
    public class Startup
    {
        IWebHostEnvironment _webHostEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _webHostEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            if (_webHostEnvironment.IsEnvironment("Local"))
            {
                services.AddDbContext<ITodoDbContext, TodoDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("LocalConnection")));
            }
            else
            {
                services.AddDbContext<ITodoDbContext, TodoDbContext>(options =>
                {
                    SqlAuthenticationProvider.SetProvider(
                        SqlAuthenticationMethod.ActiveDirectoryDeviceCodeFlow,
                        new CustomAzureSQLAuthProvider()
                    );
                    var sqlConnection = new SqlConnection(Configuration.GetConnectionString("AzureConnection"));
                    options.UseSqlServer(sqlConnection);
                });
            }

            services.AddScoped<IFeatureFlags>(p => new FeatureFlags(p.GetRequiredService<IConfiguration>(), p.GetRequiredService<ITodoDbContext>()));
            services.AddControllers();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsEnvironment("Local"))
            {
                app.UseDeveloperExceptionPage();
            }
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
