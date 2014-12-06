using System;
using System.DirectoryServices;
using System.IO;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.Misc;
using Un4seen.BassAsio;
using Un4seen.BassWasapi;

namespace ConsoleApplication
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WindowWidth = 150;
            Console.Title = "Test Application";

            DirectoryEntry entry = new DirectoryEntry(Uri.UriSchemeFile + "://" + Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            foreach (var directoryEntry in entry.Children)
            {
                Console.WriteLine(directoryEntry);
            }
        }
    }
}
