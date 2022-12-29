using System;

namespace DbGui.Models;

public abstract class LoggerBase
{
    public abstract void Log(string message);
    public abstract void Log(Exception exception);
}

public class ConsoleLogger : LoggerBase
{
    public override void Log(string message)
    {
        Console.WriteLine(message);
    }

    public override void Log(Exception exception)
    {
        Console.WriteLine($"{exception.Source}, {exception.Message}");
    }
}

public static class LogHelper
{
    private static LoggerBase logger = null;

    public static void Log(string message)
    {
        logger = new ConsoleLogger();
        logger.Log(message);
    }
}