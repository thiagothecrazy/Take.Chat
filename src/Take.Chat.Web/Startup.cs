using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Take.Chat.Core.Interfaces;
using Take.Chat.Core.Services;
using Take.Chat.Infrastructure.Data;


namespace Take.Chat
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddControllersWithViews().AddJsonOptions(o => { });
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllers();
            services.AddMvc();
            services.AddHealthChecks();

            services.AddScoped<IUsuarioServico, UsuarioServico>();
            services.AddScoped<ISalaServico, SalaServico>();
            services.AddScoped<IBatePapoServico, BatePapoServico>();

            services.AddSingleton<IMensagemServico, MensagemServico>();
            services.AddSingleton<IBatePapoRepositorio, BatePapoRepositorio>();
            services.AddSingleton<IWebSocketRepositorio, WebSocketRepositorio>();

            //ContainerSetup.InitializeWeb(Assembly.GetExecutingAssembly(), services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseWebSockets();
            app.UseMiddleware<CustomMiddleware>();

            app.UsePathBase("/Take.Chat");
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllerRoute("default", "{controller=BatePapo}/{action=Index}");
            });
        }
    }
}
