using System.Reflection.PortableExecutable;

namespace Driver.Helper;

public static class FileHelper
{
    public static void CreateAndWriteToFile(string path, string content)
    {
        var directory = Path.GetDirectoryName(path);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(path))
        {
            File.Create(path).Close();
        }

        File.WriteAllText(path, content);
    }
}