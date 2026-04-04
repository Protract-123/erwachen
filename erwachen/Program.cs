using System;

namespace erwachen;
    internal static class Program
    {
        private const string Version = "1.0.0";

        private static int Main(string[] args)
        {
            Core.Wake.SendMagicPacket("ABC123ABC123", "255.255.255.255", 9);
            Console.Write("Woke up the (definitely not hardcoded) device!!");
            return 0;
        }
    }
