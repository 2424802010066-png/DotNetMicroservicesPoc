var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// --- Task #2.1: dang ky MediatR vao DI container ---
// De bai goi y "services.AddMediatR() trong lop Startup, phuong thuc ConfigureServices".
// .NET 10 dung mo hinh minimal hosting nen khong con lop Startup; cho dang ky tuong duong
// la builder.Services ngay trong Program.cs (xem ghi chu trong BAOCAO.md).
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(AuthService.Api.AssemblyMarker).Assembly);
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

