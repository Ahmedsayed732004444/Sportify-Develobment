using Microsoft.AspNetCore.HttpOverrides;
using Sportiva;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
//app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "careerPath V1");
});

app.MapControllers();

// ... (باقي أكواد الـ Middleware السابقة مثل app.MapControllers)

// إنشاء نطاق (Scope) لتشغيل الخدمات لمرة واحدة عند إقلاع السيرفر
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    try
//    {
//        // استدعاء دالة بناء حساب المدير
//        await AdminSeeder.SeedAdminAsync(services);
//    }
//    catch (Exception ex)
//    {
//        // تسجيل أي خطأ قد يحدث أثناء العملية
//        var logger = services.GetRequiredService<ILogger<Program>>();
//        logger.LogError(ex, "حدث خطأ أثناء إنشاء حساب المدير الافتراضي.");
//    }
//}

app.Run();

app.Run();



//IClubService — كل حاجة بتتعلق بيه
//ISubscriptionPlanService — الـ plans لازم تتعمل قبل الـ club subscriptions
//IClubSubscriptionService — بعد الـ plans
//ICourtService — بيتبع الـ club
//ITimeSlotService — بيتبع الـ court
//IBookingService — محتاج court + time slot
//IReviewService — محتاج booking
//IMembershipUpgradeService — مستقل نسبياً
//IFriendlyMatchService — محتاج court
//IMatchJoinRequestService — بيتبع الـ match
//ITournamentService — أضخم feature، بيتبع court كمان
//INotificationService — cross-cutting، يتعمل قبل ما تشتغل على الـ real-time features
//IMessagingService — آخر حاجة، مستقلة تماماً