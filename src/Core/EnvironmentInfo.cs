using System;
using System.Runtime.InteropServices; 

namespace Core;         

public sealed record EnvironmentReport( 
    string OsDescription, 
    string OsVersion,                
    string ProcessArchitecture, 
    string ClrVersion,               
    string FrameworkDescription, 
    string DetectedRid,              
    string ReportedRid,              
    string BaseDirectory, 
    string CurrentDirectory,
    string BuildNote               // ДОДАНО: Поле для умовної компіляції
); 

public static class EnvironmentInfo 
{ 
    public static EnvironmentReport Collect()
    {
        // Визначення примітки залежно від цільового фреймворку (TFM)
#if NET10_0_OR_GREATER 
        const string buildNote = "збірка під net10.0"; 
#else 
        const string buildNote = "збірка під net8.0"; 
#endif

        return new EnvironmentReport( 
            RuntimeInformation.OSDescription, 
            Environment.OSVersion.ToString(),
            RuntimeInformation.ProcessArchitecture.ToString(), 
            Environment.Version.ToString(),
            RuntimeInformation.FrameworkDescription, 
            DetectRid(), 
            RuntimeInformation.RuntimeIdentifier, 
            AppContext.BaseDirectory,
            Environment.CurrentDirectory,
            buildNote              // ДОДАНО: Передаємо у рекорд
        );
    }

    private static string DetectRid() 
    { 
        string os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win" : 
                    RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux" : 
                    RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "osx" : "unknown"; 

        string arch = RuntimeInformation.ProcessArchitecture switch { 
            Architecture.X64 => "x64", 
            Architecture.X86 => "x86", 
            Architecture.Arm64 => "arm64", 
            Architecture.Arm => "arm", 
            _ => "unknown" 
        }; 
        return $"{os}-{arch}"; 
    } 
}
