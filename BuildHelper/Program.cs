using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using System.Resources;
using System.Text;

namespace NativeOS
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> buildfile = new();
            foreach (var item in File.ReadAllText(args[0]).Split(Environment.NewLine))
            {
                var s = item.Split("=");
                buildfile.Add(s[0], s[1]);
            }

            var Dirname = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                @"\" +
                Path.GetRandomFileName();
            Directory.CreateDirectory(Dirname);

            var info = Assembly.GetExecutingAssembly().GetName();
            var name = info.Name;
            using var stream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream($"{name}.BaseCode.zip")!;

            ZipFile.ExtractToDirectory(stream, Dirname);

            var configPath = Path.GetDirectoryName(Path.GetFullPath(args[0]));
            var corlibPath = Path.GetFullPath(configPath + @"\" + Path.GetDirectoryName(buildfile["CORLIB"]));
            var kernelPath = Path.GetFullPath(configPath + @"\" + Path.GetDirectoryName(buildfile["KERNEL"]));
            var osPath = Path.GetFullPath(configPath + @"\" + Path.GetDirectoryName(buildfile["OS"]));
            CopyDirectory(corlibPath, Dirname + @"\" + "Corlib");
            CopyDirectory(kernelPath, Dirname + @"\" + "Kernel");
            CopyDirectory(osPath, Dirname + @"\" + "MOOS",".csproj");

            var a = Environment.CurrentDirectory;
            Environment.CurrentDirectory = Dirname;
            Process.Start("cmd", "/c dotnet publish -p:vm=" + buildfile["VM"].ToLower() +" -r win-x64 -c debug MOOS -p:EmitLegacyAssetsFileItems=true").WaitForExit();
            Environment.CurrentDirectory = a;
            Directory.Delete(Dirname,true);
        }
        static void CopyDirectory(string sourceDir, string destDir,string ignore="")
        {
            // Create the destination directory if it doesn't exist
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            // Get the files in the source directory and copy them to the destination directory
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                if (Path.GetExtension(file) == ignore) continue;
                string destFile = Path.Combine(destDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            // Get the subdirectories in the source directory and copy them to the destination directory
            foreach (string subdir in Directory.GetDirectories(sourceDir))
            {
                string destSubdir = Path.Combine(destDir, Path.GetFileName(subdir));
                CopyDirectory(subdir, destSubdir);
            }


        }
    }
}
