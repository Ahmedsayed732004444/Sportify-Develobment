global using FluentValidation;
global using Mapster;
global using MapsterMapper;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Diagnostics;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.AspNetCore.Identity.UI.Services;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.WebUtilities;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;
global using Sportiva.Abstractions;
global using Sportiva.Abstractions.Consts;
global using Sportiva.Authentication;
global using Sportiva.Authentication.Filters;
global using Sportiva.Contracts.Authentication;
global using Sportiva.Entities;
global using Sportiva.Enums;
global using Sportiva.Errors;
global using Sportiva.Helpers;
global using Sportiva.Persistence;
global using Sportiva.Settings;
//global using Sportiva.Specifications;
//global using Sportiva.Authentication.Filters;
global using System.ComponentModel.DataAnnotations;
global using System.IdentityModel.Tokens.Jwt;
global using System.Reflection;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;


















//https://www.google.com/maps/place/27%C2%B010'43.4%22N+31%C2%B009'37.7%22E/@27.1787074,31.1578847,17z/data=!3m1!4b1!4m4!3m3!8m2!3d27.1787074!4d31.1604596?hl=en&entry=ttu&g_ep=EgoyMDI2MDQwNy4wIKXMDSoASAFQAw%3D%3D
//هي دي الميثود كاملة:
//csharppublic static double Haversine(double lat1, double lon1, double lat2, double lon2)
//{
//    double R = 6371; // نصف قطر الأرض بالكيلومتر

//    double dLat = (lat2 - lat1) * Math.PI / 180;
//    double dLon = (lon2 - lon1) * Math.PI / 180;

//    double a =
//        Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
//        Math.Cos(lat1 * Math.PI / 180) *
//        Math.Cos(lat2 * Math.PI / 180) *
//        Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

//    double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

//    return Math.Round(R * c, 2); // المسافة بالكيلومتر لحدين عشري
//}
//مثال استخدام:
//csharpdouble userLat = 30.0444;
//double userLng = 31.2357;

//double stadiumLat = 27.1787074;
//double stadiumLng = 31.1578847;

//double distance = Haversine(userLat, userLng, stadiumLat, stadiumLng);

//Console.WriteLine($"المسافة: {distance} كم");
//لو عندك List ملاعب وعايز ترتبهم من الأقرب:
//csharpvar sortedStadiums = stadiums
//    .Select(s => new
//    {
//        Stadium = s,
//        Distance = Haversine(userLat, userLng, s.Latitude, s.Longitude)
//    })
//    .OrderBy(x => x.Distance)
//    .ToList();