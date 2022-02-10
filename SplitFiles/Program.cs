Console.WriteLine("Program for split many files to subsets");

Console.WriteLine("Horev Ivan 2022 - xopeb.com");

var directory = GetDirectory();

if (Directory.Exists(directory).Not())
{
    Console.WriteLine("Directory not found");

    return;
}

var files = GetFiles(directory);

files = Order(files);

var set = Split();

Apply(directory, set);

/* ==== METHODS ==== */

void Apply(string directory, IDictionary<int, IEnumerable<string>> set)
{
    foreach (var key in set.Keys)
    {
        var directoryName = getDirectoryName(key);

        var newDirectory = Path.Combine(directory, directoryName);

        if (Directory.Exists(newDirectory).Not()) Directory.CreateDirectory(newDirectory);

        foreach (var filePath in set[key])
        {
            var file = new FileInfo(filePath);

            var newFilePath = Path.Combine(newDirectory, file.Name);

            Console.Write($"{file.Name} -> {directoryName}");

            File.Move(filePath, newFilePath);

            Console.WriteLine(" OK");
        }
    }
}

string getDirectoryName(int index)
{
    return (index + 1).ToString();
}

IDictionary<int, IEnumerable<string>> Split()
{
    var type = GetArgumentString("type", defaultValue: "rotation");

    switch (type)
    {
        case "rotation": return SplitByRotation(files);

        default:
            {
                Console.WriteLine($"Unknown type: {type}");

                return SplitByRotation(files);
            }
    }
}

string? GetDirectory()
{
    var directory = GetArgumentString("d", defaultValue: null);

    if (directory == null)
    {
        Console.WriteLine("Enter targer directory: ");

        directory = Console.ReadLine();
    }

    return directory;
}

IEnumerable<string> Order(IEnumerable<string> files)
{
    var order = GetArgumentString("order", defaultValue: "name");

    switch (order)
    {
        case "name": return OrderByName(files);

        default:
            {
                Console.WriteLine($"Unknown order: {order}");

                return OrderByName(files);
            }
    }
}

IEnumerable<string> OrderByName(IEnumerable<string> files) => files.OrderBy(f => f);

/// <summary> Split by rotation
///           For example with count 2: ABCDEF => AD, BE, CF
/// </summary>
IDictionary<int, IEnumerable<string>> SplitByRotation(IEnumerable<string> files)
{
    var count = GetArgumentInt32("count", defaultValue: 25);

    var countOfSets = (files.Count() / count) + (files.Count() % count > 0 ? 1 : 0);

    var set = new Dictionary<int, List<string>>();

    var index = 0;

    foreach (var file in files)
    {
        var key = index++ % countOfSets;

        if (set.ContainsKey(key).Not()) set[key] = new List<string>();

        set[key].Add(file);
    }

    return Convert(set);
}

IDictionary<int, IEnumerable<string>> Convert(Dictionary<int, List<string>> sets)
{
    return sets.ToDictionary(kv => kv.Key, kv => kv.Value as IEnumerable<string>);
}

string? GetArgumentString(string name, string? defaultValue = null)
{
    var value = GetArgumentValue(name);

    if (value != null) return value;

    Console.WriteLine($"{name}={value}");

    return defaultValue;
}

int GetArgumentInt32(string name, int defaultValue)
{
    var valueStr = GetArgumentString(name);

    if (valueStr != null && int.TryParse(valueStr, out int value)) return value;

    return defaultValue;
}

/// <summary> Get variable from command line for key with prefix -
///           For example, program runned with parameters: -d /root/dir01
///           then method returns: /root/dir01
///           for argument name: d
/// </summary>
string? GetArgumentValue(string argumentName)
{
    var arguments = Environment.GetCommandLineArgs();

    var key = $"-{argumentName}";

    var index = Array.IndexOf(arguments, key);

    if (index >= 0 && arguments.Length > index) return arguments[index + 1]; // returns next argument after key

    return null;
}

/// <summary> Gets set of files
/// </summary>
IEnumerable<string> GetFiles(string directory)
{
    var files = Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly);

    Console.WriteLine($"Finded files: {files.Count()}");

    return files;
}

/* ==== CLASSES ==== */

/// <summary> Helper methods
/// </summary>
public static class Common
{
    /// <summary> Inversion bool value
    /// </summary>
    public static bool Not(this bool value) => !value;
}