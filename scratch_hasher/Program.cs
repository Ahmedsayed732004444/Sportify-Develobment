using System;
using Microsoft.AspNetCore.Identity;
using Sportiva.Entities;

var user = new ApplicationUser { UserName = "admin@sportiva.com", Email = "admin@sportiva.com" };
var hasher = new PasswordHasher<ApplicationUser>();
string hash = hasher.HashPassword(user, "Admin123!");
Console.WriteLine("HASH_RESULT_START");
Console.WriteLine(hash);
Console.WriteLine("HASH_RESULT_END");
