using System;

namespace EcoEnergyManagement.Core.Logging.Formatting
{
    interface ILogFormatter
    {
        string FormatMessage(string message, string memberName, string filePath, int lineNumber);
        string FormatMessage(Exception ex, string memberName, string filePath, int lineNumber);
    }
}
