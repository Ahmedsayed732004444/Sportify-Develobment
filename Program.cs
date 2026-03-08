using Sportiva;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDependencies(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "careerPath V1");
    });
}

app.MapControllers();

app.Run();