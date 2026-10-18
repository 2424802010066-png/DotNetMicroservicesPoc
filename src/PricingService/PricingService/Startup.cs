using GlobalExceptionHandler.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PricingService.Configuration;
using PricingService.DataAccess.Marten;
using PricingService.Infrastructure;
using PricingService.Init;
using Steeltoe.Discovery.Client;

namespace PricingService;

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
        services.AddDiscoveryClient(Configuration);
        services.AddControllers()
            .AddNewtonsoftJson(opt => { opt.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto; });

        services.AddMarten(Configuration.GetConnectionString("DefaultConnection"));
        services.AddPricingDemoInitializer();
        services.AddMediatR(options => options.RegisterServicesFromAssemblyContaining<Program>());
        services.AddLoggingBehavior();
        services.AddSwaggerGen();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseGlobalExceptionHandler(cfg => cfg.MapExceptions());

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // UseInitializer() la async Task (nap 4 bieu phi mau vao CSDL). Ban goc goi
        // ma KHONG cho ket qua -> viec nap chay nen, ung dung nhan request ngay khi
        // du lieu chua co. Chay binh thuong thi khong thay vi nguoi dung go cham hon
        // vai giay, nhung PricingService.IntegrationTest gui request ngay sau khi host
        // khoi dong nen doc phai CSDL rong -> tariff null -> HTTP 500.
        // Cho nap xong roi moi mo pipeline thi het.
        app.UseInitializer().GetAwaiter().GetResult();

        // Bai thuc hanh so 5 - Task #2, Buoc 2.
        // De bai huong dan goi them app.UseDiscoveryClient() trong Configure(). Goi do
        // thuoc Steeltoe 2.x; tu Steeltoe 3.0 no da bi go bo - ban 3.2.7 dang dung chi
        // con AddDiscoveryClient / AddServiceDiscovery (kiem chung trong
        // Steeltoe.Discovery.ClientBase.xml). Ly do: AddDiscoveryClient() o
        // ConfigureServices da tu dang ky mot IHostedService lo viec dang ky va huy dang
        // ky instance voi Eureka theo vong doi ung dung, nen khong con can buoc trong
        // pipeline nua. Goi app.UseDiscoveryClient() o day se khong bien dich duoc.
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}