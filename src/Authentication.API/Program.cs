using Authentication.Application;
using Authentication.Application.Handlers.Commands;
using Authentication.Application.Handlers.Queries;
using Authentication.Application.ViewModels;
using Authentication.Infrastructure.Repository;
using MediatR;
using Microsoft.Identity.Web;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication()
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterAuthenticationRepository(builder.Configuration);
builder.Services.RegisterAuthenticationApplication();

var app = builder.Build();


#region Apis

app.MapGet("/api/get-authentication/{id}", async (HttpContext httpContext, IMediator mediator, long id) =>
{
    //httpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

    var result = await mediator.Send(new GetAuthenticationQuery(id));
    return result;
});

app.MapPost("/api/authentication/register-user", async (IMediator mediator, AuthenticationViewModel user) =>
{
    try
    {
        return Results.Ok(await mediator.Send(new StoreAuthenticationCommand(user.Mobile, user.Email)));
    }
    catch (Exception ex)
    {
        Log.Error($"Error: message: {ex.Message} ");

        return Results.StatusCode((int)StatusCodes.Status500InternalServerError);
    }
});
//.RequireAuthorization();

#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

//app.MapControllers();

app.Run();
