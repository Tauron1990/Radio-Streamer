using System;
using System.IO;
using System.Linq;
using NuGet;

namespace NugetPacker
{
    static class Program
    {
        static void Main()
        {
            Console.Title = "Nuget Packer";
            Console.WriteLine("Prepare...");

            string packages = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Packages");
            if (Directory.Exists(packages)) 
                Directory.Delete(packages, true);
            Directory.CreateDirectory(packages);
            Console.WriteLine();

            foreach (var directory in Directory.EnumerateDirectories(AppDomain.CurrentDomain.BaseDirectory))
            {
                try
                {
                    Console.Write("Packing... ");
                    if(directory.EndsWith("Packages")) continue;

                    var pack = new PackageBuilder(Directory.EnumerateFiles(directory).First(s => s.EndsWith(".nuspec")),
                                                  directory, null, true);
                    Console.Write(pack.Id + "... ");

                    using (var stream = new FileStream(Path.Combine(packages, pack.Id + ".nupkg"), FileMode.Create))
                    {
                        pack.Save(stream);
                    }

                    Directory.Delete(directory, true);

                    Console.WriteLine("Done");
                }
                catch (Exception e)
                {
                    Console.WriteLine();
                    Console.Write("Error: ");
                    Console.WriteLine(e);
                    Console.WriteLine("Press enter.");
                    Console.ReadKey();
                }
            }

            Console.WriteLine();
            Console.WriteLine("Finished");
            Console.WriteLine("Press Enter...");
            Console.ReadKey();
        }
    }
}
