using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Parse
{
    static class Program
    {
        static void Main(string[] args)
        {
            var input = args[0];
            var output = args[1];
            if (!File.Exists(input))
            {
                Console.Error.WriteLine($"{input} not found");
                return;
            }
            if (File.Exists(output))
            {
                Console.Error.WriteLine($"{output} already exists");
                return;
            }
            using (var reader = new StreamReader(input))
            {
                var namesByYear = new Dictionary<string, Dictionary<int, int>>(100_000);
                var peopleByYear = new Dictionary<int, int>(100);
                reader.ReadLine(); //Skip header
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        var parts = line.Trim().Split(',');
                        var count = int.Parse(parts[1]);
                        var year = int.Parse(parts[2]);
                        peopleByYear[year] = peopleByYear.GetValueOrDefault(year) + count;
                        var fullName = parts[0].Standardize();
                        var names = fullName.Split(' ');
                        if (names.Length > 1)
                            foreach (var name in names)
                                AddName(namesByYear, name, year, count);
                        AddName(namesByYear, fullName, year, count);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"Invalid record '{line}'. {ex.Message}");
                    }
                }
                Write(output, namesByYear, peopleByYear);
            }
        }

        static void Write(string output, Dictionary<string, Dictionary<int, int>> namesByYear, Dictionary<int, int> peopleByYear)
        {
            using (var writer = new StreamWriter(output))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                JsonSerializer.Create().Serialize(jsonWriter, new
                {
                    namesByYear = namesByYear.ToDictionary(x => x.Key,
                        x => x.Value.Select(v => new[] {v.Key, v.Value}).ToArray()),
                    peopleByYear
                });
            }
        }

        static void AddName(Dictionary<string, Dictionary<int, int>> namesByYear, string name, int year, int count)
        {
            var byName = namesByYear.GetValueOrDefault(name);
            if (byName == null)
            {
                byName = new Dictionary<int, int>();
                namesByYear[name] = byName;
            }
            byName[year] = byName.GetValueOrDefault(year) + count;
        }

        public static string Standardize(this string name) =>
            Regex.Replace(name, "\\s+", " ")
                .Trim()
                .ToLower()
                .Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u');

        static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;
            dictionary.TryGetValue(key, out value);
            return value;
        }
    }
}
