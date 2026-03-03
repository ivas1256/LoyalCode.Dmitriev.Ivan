using LoyalCode.Dmitriev.Ivan.Infrustructure;
using LoyalCode.Dmitriev.Ivan.RatesLoader;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddInfrastructure(builder.Configuration);

var host = builder.Build();
host.Run();
