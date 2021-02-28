using System;
using System.Globalization;
using osu.API.Client.Auth;

namespace snipeyourself.Utils {
    public static class TUICfg {
        public static ClientCredentialsGrant ClientCfg() {
            Console.WriteLine("Please input the Client ID (Number)");
            var input = Console.ReadLine();
            if (!ulong.TryParse(input, out var id)) {
                Console.WriteLine("Client ID Must be a number. Try again.");
                return TUICfg.ClientCfg();
            }
            Console.WriteLine("Please type the Client Secret");
            input = Console.ReadLine();
            Console.Clear();
            Console.WriteLine($"Configuration for Client #{id} completed.");
            return new(id, input);
        }

        public static PlayerInfo PlayerCfg() {
            Console.WriteLine("Please input the Player ID (Number) [seen on https://osu.ppy.sh/users/ID]");
            var input = Console.ReadLine();
            if (!ulong.TryParse(input, out var id)) {
                Console.WriteLine("Player ID Must be a number. Try again.");
                return TUICfg.PlayerCfg();
            }
            Console.Clear();
            Console.WriteLine($"Configuration for Player #{id} completed.");
            return new(id);
        }
    }
}