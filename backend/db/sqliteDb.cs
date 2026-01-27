using System;
using System.Collections;
using System.Data.SQLite;
using Dapper;

namespace backend.db;

public class SqliteDb
{
    const string path = "./backend.db";
    readonly string constr;
    public SqliteDb()
    {
        constr = $"Data Source={path}";

        using var con = new SQLiteConnection(constr);
        con.Open();

        con.Execute("CREATE TABLE IF NOT EXISTS users (Id INTEGER PRIMARY KEY AUTOINCREMENT, Username TEXT UNIQUE, Password TEXT)");
        con.Execute("CREATE TABLE IF NOT EXISTS categories (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT UNIQUE)");
        con.Execute("CREATE TABLE IF NOT EXISTS tasks (Id INTEGER PRIMARY KEY AUTOINCREMENT, IdCategory INTEGER, Title TEXT, Description Text, CreatedBy INTEGER)");
    }

    public void Execute(string sql, object? param = null)
    {
        using var con = new SQLiteConnection(constr);
        con.Open();
        con.Execute(sql, param);
    }
    public IEnumerable<T> Query<T>(string sql, object? param = null)
    {
        using var con = new SQLiteConnection(constr);
        con.Open();
        return con.Query<T>(sql, param);
    }
    public dynamic QueryFirst(string sql, object? param = null)
    {
        using var con = new SQLiteConnection(constr);
        con.Open();
        return con.QueryFirst(sql, param);
    }
    public T QueryFirst<T>(string sql, object? param = null)
    {
        using var con = new SQLiteConnection(constr);
        con.Open();
        return con.QueryFirst<T>(sql, param);
    }
}