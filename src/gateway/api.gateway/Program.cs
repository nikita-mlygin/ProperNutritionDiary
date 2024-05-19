using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var cert = new X509Certificate2("/https/dt.pfx", "pass");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder
    .Services.AddReverseProxy()
    .ConfigureHttpClient(
        (ctx, handler) =>
        {
            handler.SslOptions = new SslClientAuthenticationOptions
            {
                ClientCertificates = [cert],
                RemoteCertificateValidationCallback = (a, b, c, d) => true
            };
        }
    )
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
    
builder.Host.UseSerilog(
    (context, configuration) => configuration.ReadFrom.Configuration(context.Configuration)
);

var app = builder.Build();

app.UseCors(builder => {
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
});

app.UseSwagger();
app.UseSwaggerUI();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapReverseProxy();

app.Run();
