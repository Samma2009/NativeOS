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
            Dictionary<string, string> buildfile = new()
            {
                {"CORLIB","Corlib\\Corlib.shproj"},
                {"KERNEL","Kernel\\Kernel.shproj"},
                {"OS","OS\\(GENERIC OS NAME).csproj"},
                {"RAMDISK","Ramdisk"},
                {"VM","VMWare"},
            };
            foreach (var item in File.ReadAllText(args[0]).Split(Environment.NewLine))
            {
                var s = item.Split("=");
                buildfile[s[0]] = s[1];
            }
            var Dirname = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +@"\CompilerData";

            var configPath = Path.GetDirectoryName(Path.GetFullPath(args[0]));
            var corlibPath = Path.GetFullPath(configPath + @"\" + Path.GetDirectoryName(buildfile["CORLIB"]));
            var kernelPath = Path.GetFullPath(configPath + @"\" + Path.GetDirectoryName(buildfile["KERNEL"]));
            var osPath = Path.GetFullPath(configPath + @"\" + Path.GetDirectoryName(buildfile["OS"]));
            var rdisk = Path.GetFullPath(configPath + @"\" + Path.GetDirectoryName(buildfile["RAMDISK"]));
            CopyDirectory(corlibPath, Dirname + @"\" + "Corlib");
            CopyDirectory(kernelPath, Dirname + @"\" + "Kernel");
            CopyDirectory(Dirname + @"\" + "MOOSDATA", Dirname + @"\" + "MOOS");
            CopyDirectory(osPath, Dirname + @"\" + "MOOS",".csproj");
            CopyDirectory(rdisk, Dirname + @"\" + "Ramdisk");

            Environment.CurrentDirectory = Dirname;
            Process.Start("cmd", "/c dotnet publish -p:vm=" + buildfile["VM"].ToLower() +" -r win-x64 -c debug MOOS -p:EmitLegacyAssetsFileItems=true").WaitForExit();

            Directory.Delete("Corlib", true);
            Directory.Delete("Kernel", true);
            Directory.Delete("MOOS", true);
            Directory.Delete("Ramdisk", true);

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
