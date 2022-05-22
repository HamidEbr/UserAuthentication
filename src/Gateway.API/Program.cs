using MediatR;
using Microsoft.Identity.Web;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Polly;
using Serilog;
using System.Reflection;
using UserAuthentication.Application;
using UserAuthentication.Application.Handlers.Commands;
using UserAuthentication.Application.Handlers.Queries;
using UserAuthentication.Application.ViewModels;

var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
builder.Services.AddAuthentication()
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterUserAuthenticationApplication();
builder.Services.AddOcelot().AddPolly(); //for resilient gateway 

var app = builder.Build();

#region Apis

app.MapPost("/authentication/register", async (IMediator mediator, UserAuthViewModel user) =>
{
    try
    {
        return Results.Ok(await mediator.Send(new RegisterUserCommand(user.Mobile, user.Email, user.FirstName, user.LastName)));
    }
    catch (Exception ex)
    {
        Log.Error($"Error: message: {ex.Message} ");

        return Results.StatusCode((int)StatusCodes.Status500InternalServerError);
    }
});

app.MapGet("/get-user/{id}", async (HttpContext httpContext, IMediator mediator, long id) =>
{
    //httpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

    var result = await mediator.Send(new GetUserAuthQuery(id));
    return result;
});

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

app.Run();