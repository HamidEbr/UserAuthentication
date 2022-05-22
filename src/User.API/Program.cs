using MediatR;
using Microsoft.Identity.Web;
using Serilog;
using User.Application;
using User.Application.Handlers.Commands;
using User.Application.Handlers.Queries;
using User.Application.ViewModels;
using User.Infrastructre.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication()
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterUserRepository(builder.Configuration);
builder.Services.RegisterUserApplication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
//var scopeRequiredByApi = app.Configuration["AzureAd:Scopes"];

#region Apis

app.MapPost("/api/authentication/register-user", async (IMediator mediator, UserViewModel user) =>
{
    try
    {
        return Results.Ok(await mediator.Send(new StoreUserCommand(user.AuthenticationId, user.FirstName, user.LastName)));
    }
    catch (Exception ex)
    {
        Log.Error($"Error: message: {ex.Message} ");

        return Results.StatusCode((int)StatusCodes.Status500InternalServerError);
    }
});

app.MapGet("/api/get-user/{id}", async (HttpContext httpContext, IMediator mediator, long id) =>
{
    //httpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
    var result = await mediator.Send(new GetUserQuery(id));
    return result;
});
//.RequireAuthorization();

#endregion

app.Run();