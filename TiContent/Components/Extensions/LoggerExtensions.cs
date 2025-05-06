// ⠀
// LoggerExtensions.cs
// TiContent
// 
// Created by the_timick on 05.05.2025.
// ⠀

using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace TiContent.Components.Extensions;

public static class LoggerExtensions
{
    public static void LogInformationWithCaller(this ILogger logger, string message = "START", [CallerMemberName] string callerName = "")
    {
        logger.LogInformation("[{CallerName}] {Message}", callerName, message);
    }
}