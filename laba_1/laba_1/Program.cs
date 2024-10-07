using System;
using System.IO;


namespace GeneticsProject
{
    public struct GeneticData
    {
        public string name;
        public string organism;
        public string formula;
    }

    class Program
    {
        static List<char> letters = new List<char>() { 'A', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'Y' };
        static List<GeneticData> data = new List<GeneticData>();

        static string? GetFormula(string proteinName)
        {
            foreach (GeneticData item in data)
            {
                if (item.name.Equals(proteinName, StringComparison.Ordinal)) return item.formula;
            }
            
            return null;
        }

        static void ReadGeneticData(string filename)
        {
            StreamReader reader = new StreamReader(filename);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] fragments = line.Split('\t');
                GeneticData protein;
                protein.name = fragments[0];
                protein.organism = fragments[1];
                protein.formula = fragments[2];
                data.Add(protein);
               
            }
            reader.Close();
        }

        static void ReadHandleCommands(string filename, StreamWriter writer)
        {
            StreamReader reader = new StreamReader(filename);
            int counter = 0;
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                counter++;
                string[] command = line.Split('\t');

                switch (command[0])
                {
                    case "search":
                        Console.WriteLine($"{counter.ToString("D3")}   {"search"}   {Decoding(command[1])}");
                        writer.WriteLine($"{counter.ToString("D3")}   {"search"}   {Decoding(command[1])}");
                        Console.WriteLine("organism\t\tprotein");
                        writer.WriteLine("organism\t\tprotein");
                        List<int> indices = Search(command[1]);
                        if (indices.Count > 0)
                        {
                            foreach (int index in indices)
                            {
                                Console.WriteLine($"{data[index].organism}    {data[index].name}");
                                writer.WriteLine($"{data[index].organism}    {data[index].name}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("NOT FOUND");
                            writer.WriteLine("NOT FOUND");
                        }
                        Console.WriteLine("========================================================================\n");
                        writer.WriteLine("========================================================================\n");
                        break;

                    case "diff":
                        Console.WriteLine($"{counter.ToString("D3")}   {"diff"}   {command[1]}   {command[2]}");
                        writer.WriteLine(counter.ToString("D3") + "\t" + command[0] + "\t" + command[1] + "\t" + command[2]);
                        Console.WriteLine("amino-acids difference: ");
                        writer.WriteLine("amino-acids difference: ");
                        int diffCount = Diff(command[1], command[2]);
                        switch (diffCount)
                        {
                            case -1:
                                Console.WriteLine("MISSING: " + command[1]);
                                writer.WriteLine("MISSING: " + command[1]);
                                break;
                            case -2:
                                Console.WriteLine("MISSING: " + command[2]);
                                writer.WriteLine("MISSING: " + command[2]);
                                break;
                            case -3:
                                Console.WriteLine("MISSING: " + command[1] + "\t" + command[2]);
                                writer.WriteLine("MISSING: " + command[1] + "\t" + command[2]);
                                break;
                            default:
                                Console.WriteLine(diffCount);
                                writer.WriteLine(diffCount);
                                break;
                        }
                        Console.WriteLine("========================================================================\n");
                        writer.WriteLine("========================================================================\n");
                        break;

                    case "mode":
                        Console.WriteLine($"{counter.ToString("D3")}   {"mode"}   {command[1]}");
                        writer.WriteLine($"{counter.ToString("D3")}   {"mode"}   {command[1]}");
                        int modeIndex = Mode(command[1]);
                        if (modeIndex == -1)
                        {
                            Console.WriteLine("NOT FOUND");
                            writer.WriteLine("NOT FOUND");
                        }
                        else
                        {
                            Console.WriteLine("amino-acid occurs:");
                            writer.WriteLine("amino-acid occurs:");
                            Console.WriteLine($"{letters[modeIndex]}               {numberOfAcids[modeIndex]}");
                            writer.WriteLine($"{letters[modeIndex]}               {numberOfAcids[modeIndex]}");
                        }
                        Console.WriteLine("========================================================================\n");
                        writer.WriteLine("========================================================================\n");
                        break;

                    default:
                        Console.WriteLine($"Unknown command: {command[0]}");
                        writer.WriteLine($"Unknown command: {command[0]}");
                        break;
                }
            }
            reader.Close();
        }
        static bool IsValid(string formula)
        {
            foreach (char ch in formula)
            {
                if (!letters.Contains(ch)) return false;
            }
            return true;
        }

        static string Encoding(string formula)
        {
            string encoded = String.Empty;
            for (int i = 0; i < formula.Length; i++)
            {
                char ch = formula[i];
                int count = 1;
                while (i < formula.Length - 1 && formula[i + 1] == ch)
                {
                    count++;
                    i++;
                }
                if (count > 2) encoded = encoded + count + ch;
                if (count == 1) encoded = encoded + ch;
                if (count == 2) encoded = encoded + ch + ch;
            }
            return encoded;
        }

        static string Decoding(string formula)
        {
            string decoded = String.Empty;
            for (int i = 0; i < formula.Length; i++)
            {
                if (char.IsDigit(formula[i]))
                {
                    char letter = formula[i + 1];
                    int conversion = formula[i] - '0';
                    for (int j = 0; j < conversion - 1; j++) decoded = decoded + letter;
                }
                else decoded = decoded + formula[i];
            }
            return decoded;
        }

        static List<int> Search(string amino_acid)
        {
            List<int> list = new List<int>();
            string decoded = Decoding(amino_acid);
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].formula.Contains(decoded))
                {
                    list.Add(i);
                }
            }
            return list;
        }

        static int Diff(string name1, string name2)
        {
            int size, counter = 0;
            string formula1 = Decoding(GetFormula(name1)); string formula2 = Decoding(GetFormula(name2));
            if (formula1 == "" && formula2 == "")
            {
                return -3;
            }
            else if (formula2 == "")
            {
                return -2;
            }
            else if (formula1 == "")
            {
                return -1;
            }
            if (formula1.Length > formula2.Length)
            {
                size = formula2.Length;
                counter += formula1.Length - formula2.Length;
            }
            else
            {
                size = formula1.Length;
                counter += formula2.Length - formula1.Length;
            }
            for (int i = 0; i < size; i++)
            {
                if (formula1[i] != formula2[i]) counter++;
            }
            return counter;
        }

        static List<int> numberOfAcids = new List<int>(new int[20]);
        static int Mode(string name)
        {
            string formula = GetFormula(name);
            int number = 0, index = 0;
            if (formula == "") return -1;
            for (int i = 0; i < 20; i++)
            {
                numberOfAcids[i] = 0;
            }
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < formula.Length; j++)
                {
                    if (formula[j] == letters[i]) numberOfAcids[i]++;
                }
                if (numberOfAcids[i] > number) number = numberOfAcids[i];
            }
            for (int i = 0; i < 20; i++)
            {
                if (numberOfAcids[i] == number)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Yuliya Stas\nGenetic Searching");
            StreamWriter writer = new StreamWriter("gendata.txt");
            writer.WriteLine("Yuliya Stas\nGenetic Searching");
            ReadGeneticData("sequences.1.txt");
            Console.WriteLine("========================================================================");
            writer.WriteLine("========================================================================");
            ReadHandleCommands("commands.1.txt", writer);           
            writer.Close();
        }
    }
}