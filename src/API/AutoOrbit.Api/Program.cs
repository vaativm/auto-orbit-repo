using AutoOrbit.Api;
using AutoOrbit.Api.Middleware;
using AutoOrbit.Api.Shared.Models;
using Microsoft.Extensions.FileProviders;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddAppServices(builder.Configuration);

if (builder.Environment.IsDevelopment())
{
    builder.Services.ConfigSwagger();
}

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(s => s.SwaggerEndpoint("/swagger/v1/swagger.json", "AutoOrbit Api v1"));
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseMiddleware<RequestContextLoggingMiddleware>();
app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseStaticFiles();

ImageLocationOptions imageLoc = builder.Configuration.GetSection(ImageLocationOptions.SectionName).Get<ImageLocationOptions>();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.GetFullPath(imageLoc!.Path)),
    RequestPath = new PathString("/app-images"),
});

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
