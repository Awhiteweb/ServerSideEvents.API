using MassTransit;
using System.Text.Json;
using ServerSideEvents.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ItemService>();
builder.Services.AddCors(options => {
	options.AddPolicy("Any", policy => 
		policy.AllowAnyHeader()
			  .AllowAnyMethod()
			  .AllowAnyOrigin());
});
builder.Services.AddMassTransit(x => {
	x.AddConsumer<InformationConsumer>();
	x.AddConsumer<WarningConsumer>();
	x.UsingInMemory((ctx, cfg) => {
		cfg.ConfigureEndpoints(ctx);
	});
});
builder.Services.AddHostedService<Worker>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Any");
// app.UseHttpsRedirection();

app.MapGet("/events", async (HttpContext ctx, ItemService itemService, CancellationToken ct) => {
	var token = ctx.Request.Headers.Authorization.First();
	Console.WriteLine("Received connection for " + token);
	ctx.Response.Headers.Append("Content-Type", "text/event-stream");
	while(!ct.IsCancellationRequested) {
		var item = await itemService.WaitForNewItem(token);
		if(item != null) {
			await ctx.Response.WriteAsync($"event: {item.GetType().Name}\n", ct);
			await ctx.Response.WriteAsync("data: ", ct);
			await JsonSerializer.SerializeAsync(ctx.Response.Body, item.Item, cancellationToken: ct);
			await ctx.Response.WriteAsync("\n\n", ct);
			await ctx.Response.Body.FlushAsync(ct);
		}
	}
})
.WithName("Events")
.WithOpenApi();

app.Run();
