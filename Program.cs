<<<<<<< HEAD
﻿namespace SportivaModels
=======
using Career_Path;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDependencies(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();
if (app.Environment.IsDevelopment())
>>>>>>> cef9d36 (add new arc)
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
}
