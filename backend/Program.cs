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

        app.MapGet("/category", (SqliteDb db) =>
        {
            var cat = db.Query<CategoryModel>("SELECT Id, Name FROM categories");
            return cat;
        }).RequireAuthorization();
        app.MapPost("/category", (string name, SqliteDb db) =>
        {
            db.Execute("INSERT INTO categories(Name) VALUES (@name)", new {name = name});
            return Results.Ok();    
        }).RequireAuthorization();
        app.MapPut("/category", (CategoryModel cat, SqliteDb db) =>
        {
            db.Execute("UPDATE categories SET Name = @Name WHERE Id=@Id", cat);
            return Results.Ok();
        }).RequireAuthorization();
        app.MapDelete("/category", (int id, SqliteDb db) =>
        {
            db.Execute("DELETE FROM categories WHERE Id=@id", new { id = id });
            return Results.Ok($"Deleted category with id={id}");    
        }).RequireAuthorization();

        app.MapGet("/task", (int? catId, SqliteDb db) =>
        {
            IEnumerable<TaskModel> tasks;

            if (catId == null)
                tasks = db.Query<TaskModel>("SELECT Id, IdCategory, Title, Description, CreatedBy FROM tasks");
            else
                tasks = db.Query<TaskModel>("SELECT Id, IdCategory, Title, Description, CreatedBy FROM tasks WHERE IdCategory = @catId", new { catId = catId });
            return tasks;
        }).RequireAuthorization();
        app.MapPost("/task", (TaskModel task, SqliteDb db) =>
        {
            db.Execute("INSERT INTO tasks(IdCategory, Title, Description, CreatedBy) VALUES (@IdCategory, @Title, @Description, @CreatedBy)", task);
            return Results.Ok();
        }).RequireAuthorization();
        app.MapPut("/task", (TaskModel task, SqliteDb db) =>
        {
            db.Execute("UPDATE tasks SET IdCategory = @IdCategory, Title = @Title, Description = @Description, CreatedBy = @CreatedBy WHERE Id=@Id", task);
            return Results.Ok();
        }).RequireAuthorization();

        app.MapGet("/task/{id}", (int id, SqliteDb db) =>
        {
            return db.QueryFirst<TaskModel>("SELECT Id, IdCategory, Title, Description, CreatedBy FROM tasks WHERE Id = @id", new { id = id });
        }).RequireAuthorization();
        app.MapDelete("/task/{id}", (int id, SqliteDb db) =>
        {
            db.Execute("DELETE FROM tasks WHERE Id=@id", new { id = id });
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
