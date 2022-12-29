using System;

namespace DbGui.Models;

public class DbException : Exception
{
    public DbException()
    {
        LogHelper.Log("something");
    }

    public DbException(string message) : base($"Could not set columns {message}")
    {
        LogHelper.Log(message);
    }
}