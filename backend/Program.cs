using backend.auth;
using backend.db;
using Microsoft.AspNetCore.Authentication;
using common.Models;
using System.ComponentModel.DataAnnotations;

namespace backend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton<SqliteDb>();
        builder.Services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("Basic", options => { });
        builder.Services.AddAuthorization();
        var app = builder.Build();
        
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGet("/login", () => { }).RequireAuthorization();

        app.MapGet("/category", () =>
        {
            CategoryModel[] cat = [
                new(){Id=1, Name="Do zrobienia"},
                new(){Id=2, Name="W trakcie"},
                new(){Id=3, Name="Gotowe"}
            ];
            return cat;
        }).RequireAuthorization();
        app.MapPost("/category", (string name) =>
        {
            return Results.Ok(1);    
        }).RequireAuthorization();
        app.MapPut("/category", (CategoryModel cat) =>
        {
            return Results.Ok(cat);
        }).RequireAuthorization();
        app.MapDelete("/category", (int id) =>
        {
            return Results.Ok($"Deleted category with id={id}");    
        }).RequireAuthorization();

        app.MapGet("/task", (int? catId) =>
        {
            TaskModel[] tasks;
            if (catId == null)
            {
                tasks = [
                    new TaskModel() { Id = 1, Assignees = [1, 2], IdCategory = 1, CreatedBy = 1, Description = "Desc", Title = "tytuł" },
                    new TaskModel() { Id = 2, Assignees = [1, 2], IdCategory = 2, CreatedBy = 1, Description = "Desc", Title = "tytuł" },
                    new TaskModel() { Id = 3, Assignees = [1, 2], IdCategory = 3, CreatedBy = 1, Description = "Desc", Title = "tytuł" }
                ];

            }
            else
            {
                tasks = [
                    new TaskModel() { Id = 1, Assignees = [1, 2], IdCategory = (int)catId, CreatedBy = 1, Description = "Desc", Title = "tytuł" },
                    new TaskModel() { Id = 2, Assignees = [1, 2], IdCategory = (int)catId, CreatedBy = 1, Description = "Desc", Title = "tytuł" },
                    new TaskModel() { Id = 3, Assignees = [1, 2], IdCategory = (int)catId, CreatedBy = 1, Description = "Desc", Title = "tytuł" }
                ];
            }
            return tasks;
        }).RequireAuthorization();
        app.MapPost("/task", (TaskModel task) =>
        {
            return 1;
        }).RequireAuthorization();
        app.MapPut("/task", (TaskModel task) =>
        {
            return Results.Ok();
        }).RequireAuthorization();

        app.MapGet("/task/{id}", (int id) =>
        {
            return new TaskModel() { Id = id, Assignees = [1, 2], IdCategory = 1, CreatedBy = 1, Description = "Desc", Title = "tytuł" };
        }).RequireAuthorization();
        app.MapDelete("/task/{id}", (int id) =>
        {
            return Results.Ok();
        }).RequireAuthorization();

        app.MapGet("/user", (SqliteDb db) =>
        {
            IEnumerable<UserModel> users = db.Query<UserModel>("SELECT Id, Username FROM users");
            return users;
        }).RequireAuthorization();
        app.MapPost("/user", (LoginModel login, SqliteDb db) =>
        {
            db.Execute("INSERT INTO users(Username, Password) VALUES (@Username, @Password)", login);
            return Results.Ok();
        });

        app.MapPut("/user/{username}", (string username, string password, SqliteDb db) =>
        {
            db.Execute("UPDATE users SET Password = @password WHERE Username=@username", new { password = password, username = username });
            return Results.Ok();
        }).RequireAuthorization();
        app.MapDelete("/user/{username}", (string username, SqliteDb db) =>
        {
            db.Execute("DELETE FROM users WHERE Username=@username", new { username = username });
            return Results.Ok();
        }).RequireAuthorization();

        app.Run();
    }
}
