namespace EnigmaEncryptor
{
    public class EnigmaEncryptor
    {
        private const int AsciiOffset = 65;

        private static readonly int[][] Rotors =
        [
            new [] {'E', 'K', 'M', 'F', 'L', 'G', 'D', 'Q', 'V', 'Z', 'N', 'T', 'O', 'W', 'Y', 'H', 'X', 'U', 'S', 'P', 'A', 'I', 'B', 'R', 'C', 'J'}.Select(c => c - AsciiOffset).ToArray(),
            new [] {'A', 'J', 'D', 'K', 'S', 'I', 'R', 'U', 'X', 'B', 'L', 'H', 'W', 'T', 'M', 'C', 'Q', 'G', 'Z', 'N', 'P', 'Y', 'F', 'V', 'O', 'E'}.Select(c => c - AsciiOffset).ToArray(),
            new [] {'B', 'D', 'F', 'H', 'J', 'L', 'C', 'P', 'R', 'T', 'X', 'V', 'Z', 'N', 'Y', 'E', 'I', 'W', 'G', 'A', 'K', 'M', 'U', 'S', 'Q', 'O'}.Select(c => c - AsciiOffset).ToArray(),
            new [] {'E', 'S', 'O', 'V', 'P', 'Z', 'J', 'A', 'Y', 'Q', 'U', 'I', 'R', 'H', 'X', 'L', 'N', 'F', 'T', 'G', 'K', 'D', 'C', 'M', 'W', 'B'}.Select(c => c - AsciiOffset).ToArray(),
        ];

        private static readonly int[] ReflectorA = new []
        {
            'E', 'J', 'M', 'Z', 'A', 'L', 'Y', 'X', 'V', 'B', 'W', 'F', 'C', 'R', 'Q', 'U', 'O', 'N', 'T', 'S', 'P', 'I', 'K', 'H', 'G', 'D'
        }.Select(c => c - AsciiOffset).ToArray();

        private static readonly int[] Notches =
            new[] { 'Q', 'E', 'V' }.Select(c => (c - AsciiOffset)).ToArray();

        private static int[] RotorPositions;
        private static IDictionary<char, char> Plugboard;

        static void Main(string[] args)
        {
            var plainText = args[0].ToUpper();
            RotorPositions = args[1].ToUpper().ToCharArray().Select(c => c - AsciiOffset).ToArray();

            var plugboardStr = args[2].ToUpper();
            Plugboard = new Dictionary<char, char>(
                Enumerable.Range(0, plugboardStr.Length / 2)
                    .SelectMany(i => new List<KeyValuePair<char, char>>()
                    {
                        KeyValuePair.Create(plugboardStr[i * 2], plugboardStr[(i * 2) + 1]),
                        KeyValuePair.Create(plugboardStr[(i * 2) + 1], plugboardStr[i * 2]),
                    }));

            var cipherText = new string(plainText.Select(Encrypt).ToArray());
            Console.WriteLine(cipherText);
        }

        private static char Encrypt(char input)
        {
            RotorPositions = AdvanceRotors(RotorPositions);

            var p = input;
            if (Plugboard.TryGetValue(input, out var v))
            {
                p = v;
            }

            var c = p - AsciiOffset;
            for (var i = 0; i < Rotors.Length; i++)
            {
                c = Rotors[i][(c + RotorPositions[i]) % Rotors[i].Length];
            }

            c = ReflectorA[c];

            for (var i = Rotors.Length - 1; i >= 0; i--)
            {
                c = ((Array.IndexOf(Rotors[i], c) - RotorPositions[i]) + Rotors[i].Length) % Rotors[i].Length;
            }

            p = (char)(c + AsciiOffset);
            if (Plugboard.TryGetValue(p, out var w))
            {
                p = w;
            }

            return p;
        }

        private static int[] AdvanceRotors(int[] rotorPositions)
        {
            if (rotorPositions[0] == Notches[0])
            {
                if (rotorPositions[1] == Notches[1])
                {
                    if (rotorPositions[2] == Notches[2])
                    {
                        rotorPositions[3] += 1;
                    }

                    rotorPositions[2] += 1;
                }

                rotorPositions[1] += 1;
            }

            rotorPositions[0] += 1;

            rotorPositions[0] %= Rotors[0].Length;
            rotorPositions[1] %= Rotors[1].Length;
            rotorPositions[2] %= Rotors[2].Length;
            rotorPositions[3] %= Rotors[3].Length;

            return rotorPositions;
        }
    }
}
