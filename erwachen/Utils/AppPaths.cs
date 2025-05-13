namespace erwachen.Utils;

using System;
using System.IO;

public static class AppPaths
{
    internal static readonly string WriteDirectory;

    static AppPaths()
    {
        string exeDir = AppContext.BaseDirectory;
        string testFilePath = Path.Combine(exeDir, ".write-test");

        if (CanWriteToDirectory(testFilePath))
        {
            WriteDirectory = exeDir;
        }
        else
        {
            WriteDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MyApp");

            Directory.CreateDirectory(WriteDirectory); // Ensure it exists
        }
    }

    private static bool CanWriteToDirectory(string testFilePath)
    {
        try
        {
            File.WriteAllText(testFilePath, "test");
            File.Delete(testFilePath);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
