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
app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "careerPath V1");
});

app.MapHub<Sportiva.Hubs.NotificationHub>("/hubs/notifications");
app.MapHub<Sportiva.Hubs.ChatHub>("/hubs/chat");

app.MapControllers();

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