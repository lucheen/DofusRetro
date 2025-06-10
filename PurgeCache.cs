using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace DofusRetroCacheCleaner
{
    class Program
    {

        private static readonly string[] PossiblePaths = new[]
        {
            // Windows
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Ankama", "Dofus Retro", "cache"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dofus Retro", "cache"),
            // macOS
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library", "Application Support", "Ankama", "Dofus Retro", "cache"),
            // Linux
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".config", "Ankama", "Dofus Retro", "cache"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".dofusretro", "cache")
        };

        private static string CachePath;

        private static int ClearIntervalMinutes = 10;

        static Program()
        {
            CachePath = PossiblePaths.FirstOrDefault(Directory.Exists);
            if (string.IsNullOrEmpty(CachePath))
            {
                CachePath = PossiblePaths[0];
                Console.WriteLine($"No cache folder found automatically. Using default path: {CachePath}");
            }
            else
            {
                Console.WriteLine($"Detected cache folder: {CachePath}");
            }
        }

        static void Main(string[] args)
        {
     
            if (args.Length > 0)
            {
                var customPath = args[0];
                if (Directory.Exists(customPath))
                {
                    CachePath = customPath;
                    Console.WriteLine($"Using custom cache path: {CachePath}");
                }
                else
                {
                    Console.WriteLine($"Custom path not found. Continuing with: {CachePath}");
                }
            }

       
            Console.Write($"Enter clear interval in minutes (default {ClearIntervalMinutes}): ");
            var input = Console.ReadLine();
            if (int.TryParse(input, out var minutes) && minutes > 0)
            {
                ClearIntervalMinutes = minutes;
            }
            else if (!string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine($"Invalid input. Using default interval of {ClearIntervalMinutes} minute(s).");
            }

            Console.WriteLine($"Cache will be cleared every {ClearIntervalMinutes} minute(s).");

     
            var timer = new Timer(ClearCache, null, TimeSpan.Zero, TimeSpan.FromMinutes(ClearIntervalMinutes));

            Console.WriteLine("Dofus Retro cache cleaner started. Press Enter to stop.");
            Console.ReadLine();
        }

        private static void ClearCache(object state)
        {
            try
            {
                if (!Directory.Exists(CachePath))
                {
                    Console.WriteLine($"Cache path not found: {CachePath}");
                    return;
                }

                foreach (var entry in Directory.EnumerateFileSystemEntries(CachePath))
                {
                    try
                    {
                        if (File.Exists(entry))
                            File.Delete(entry);
                        else if (Directory.Exists(entry))
                            Directory.Delete(entry, true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting {entry}: {ex.Message}");
                    }
                }

                Console.WriteLine($"Cache cleared at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during cache clear: {ex.Message}");
            }
        }
    }
}
