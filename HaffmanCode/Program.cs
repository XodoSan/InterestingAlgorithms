using System.Data;

internal class Program
{
    private static void Main()
    {
        Console.WriteLine("Haffman code realization: ");
        Console.Write("Do you have encrypt or decrypt: ");
        string choice = Console.ReadLine()!;

        string text = "";
        try
        {
            text = File.ReadAllText(Directory.GetCurrentDirectory() + '/' + "data.txt");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        var computeNumResult = ComputeNumOccurrences(text);
        var currentGraphBranch = ComputeGraphBranch(computeNumResult);
        var currentSimbolChiphers = ComputeSimbolCiphers(currentGraphBranch);

        switch (choice)
        {
            case "encrypt":
                Console.Write("Enter message to encrypting: ");
                var word = Console.ReadLine()!.ToLower();

                var encryptedText = GetEncryptedString(currentSimbolChiphers, word);
                Console.Write("Encrypted message: ");
                Console.WriteLine(encryptedText);
                break;
            case "decrypt":
                Console.Write("Enter encrypting message: ");
                encryptedText = Console.ReadLine()!;

                var decryptedText = GetDecryptedString(currentSimbolChiphers, encryptedText);
                Console.Write("Decrypted message: ");
                Console.WriteLine(decryptedText);
                break;
            default:
                Console.WriteLine($"{choice} is not correct command");
                break;
        }
    }

    //Computing a num occurences every simbol in the text
    public static Dictionary<string, int> ComputeNumOccurrences(string text)
    {
        return text
            .ToLower()
            .GroupBy(x => x)
            .ToDictionary(x => x.Key.ToString(),
                          x => x.Count());
    }

    //Computing a cipher of every simbol by graph of summing this
    public static Dictionary<string, int> ComputeGraphBranch(Dictionary<string, int> numOccurrences)
    {
        var result = new Dictionary<string, int>();
        while (numOccurrences.Count > 0)
        {
            var minDic = numOccurrences.OrderBy(x => x.Value).ThenByDescending(x => x.Key.Length).First();
            numOccurrences.Remove(minDic.Key);
            result.Add(minDic.Key, 0);

            if (numOccurrences.Count == 0)
                break;

            var minDic2 = numOccurrences.OrderBy(x => x.Value).ThenByDescending(x => x.Key.Length).First();
            numOccurrences.Remove(minDic2.Key);
            result.Add(minDic2.Key, 1);

            numOccurrences.Add(minDic.Key + minDic2.Key, minDic.Value + minDic2.Value);
        }

        return result;
    }

    public static Dictionary<char, string> ComputeSimbolCiphers(Dictionary<string, int> graphBranches)
    {
        var maxElement = graphBranches.LastOrDefault();
        return maxElement
            .Key
            .ToCharArray()
            .ToDictionary(c => c, c => string.Join("", graphBranches
                                       .Where(x => x.Key.Contains(c) && x.Key != maxElement.Key)
                                       .Select(x => x.Value).Reverse()));
    }

    public static string GetEncryptedString(Dictionary<char, string> charCodes, string word)
    {
        string result = "";
        string code = "";
        foreach (var simbol in word)
        {
            charCodes.TryGetValue(simbol, out code);
            result += code;
        }

        return result;
    }

    public static string GetDecryptedString(Dictionary<char, string> charCodes, string encryptedText)
    {
        string result = "";
        string code = "";
        int counter = 0;
        while (true)
        {
            code += encryptedText[counter];
            if (charCodes.Select(x => x.Value).Contains(code))
            {
                var currentCode = charCodes.FirstOrDefault(x => x.Value == code);
                code = "";
                result += currentCode.Key;
            }
            counter++;
            if (counter == encryptedText.Length)
                break;
        }

        return result;
    }
}