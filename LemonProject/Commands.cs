﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosmos.System.Graphics;

namespace LemonProject
{
    public class Cmds
    {
        public static int PixelHeight;
        public static int PixelWidth;
        public struct Command
        {
            public string Name, HelpDesc;
            public function func;
        }

        public static List<Command> cmds = new List<Command>();
        public delegate void function(string[] args);

        public static void Parse(string input)
        {
            string[] args = input.Split(new char[0]);
            string[] cmdargs = { };
            if (input.Contains(" ")) { cmdargs = input.Remove(0, input.IndexOf(' ') + 1).Split(new char[0]); }

            foreach (Command cmd in cmds)
            {
                if (args[0].Equals(cmd.Name))
                {
                    cmd.func(cmdargs);
                    return;
                }
            }

            Utils.Error("Invalid command.");
        }
        
        private static void AddCommand(string name, string desc, function func)
        {
            Command cd = new Command();
            cd.Name = name;
            cd.HelpDesc = desc;
            cd.func = func;
            cmds.Add(cd);
        }
        
        public static void Init()
        {
            AddCommand("print", "insert help description here", print);
            AddCommand("about", "insert help description here", about);
            AddCommand("help", "insert help description here", help);
            AddCommand("shutdown", "insert help description here", shutdown);
            AddCommand("systime", "insert help description here", systime);
            AddCommand("clear", "insert help description here", clear);
            AddCommand("cursor", "insert help description here", cursor);
        }

        #region Misc Commands
        static void print(string[] args)
        {
            if (args.Length < 1)
            {
                Utils.Error("Insufficient arguments.");
            }
            string content = String.Join(" ", args);
            Console.WriteLine(content);
        }
        
        static void help(string[] args)
        {
            if (args.Length < 1)
            {
                Utils.colorCache = Console.ForegroundColor;
                Utils.SetColor(ConsoleColor.Cyan);
                Console.WriteLine("________________________________________");
                Console.WriteLine("---- List of all available commands ----");
                Console.WriteLine();
                Utils.SetColor(ConsoleColor.Blue);
                foreach (Command cmd in cmds)
                {
                    Console.WriteLine(cmd.Name);
                }
                Utils.SetColor(Utils.colorCache);
                Console.WriteLine();
                Console.WriteLine("You can get more specific help for each command by using: HELP <COMMAND_NAME>");
                Console.WriteLine();
            }
            else
            {
                foreach (Command cmd in cmds)
                {
                    if (args[1] == cmd.Name)
                    {
                        Console.WriteLine(cmd.HelpDesc);
                        Console.WriteLine();
                        return;
                    }
                }
            }
        }
        
        static void about()
        {
            Utils.SetColor(ConsoleColor.Yellow);
            Console.WriteLine(@"
___________________________________________________
  _                                   ____   _____ 
 | |                                 / __ \ / ____|
 | |     ___ _ __ ___   ___  _ __   | |  | | (___  
 | |    / _ \ '_ ` _ \ / _ \| '_ \  | |  | |\___ \ 
 | |___|  __/ | | | | | (_) | | | | | |__| |____) |
 |______\___|_| |_| |_|\___/|_| |_|  \____/|_____/
___________________________________________________
");
            Utils.SetColor(ConsoleColor.Green);
            Console.WriteLine("");
            Console.WriteLine("Lemon OS (c) 2021, release 1.2");
            Console.WriteLine("Created by bad-codr and deadlocust");
            Utils.Warn("This is a closed beta version of Lemon OS, we are not responsible for any damages caused by it.");
            Console.WriteLine();
            Utils.SetColor(ConsoleColor.White);
        }
        static void shutdown(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Shutdown machine? [Y/N]");
                ConsoleKeyInfo input = Console.ReadKey(false);
                if (input.KeyChar == 'Y' || input.KeyChar == 'y') { Cosmos.System.Power.Shutdown(); }
                Console.WriteLine();
                return;
            }
            else if (args[1] == "-r")
            {
                Console.WriteLine("Reboot machine? [Y/N]");
                ConsoleKeyInfo input = Console.ReadKey(false);
                if (input.KeyChar == 'Y' || input.KeyChar == 'y') { Cosmos.System.Power.Reboot(); }
                Console.WriteLine();
                return;
            }
            return;
        }
        static void systime()
        {
            Console.Write(Cosmos.HAL.RTC.Hour);
            Console.Write(":");
            Console.Write(Cosmos.HAL.RTC.Minute);
            Console.Write(":");
            Console.WriteLine(Cosmos.HAL.RTC.Second);
        }
        static void clear()
        {
            Console.Clear();
        }
        static void sysinfo()
        {
            var cspeed = Cosmos.Core.CPU.GetCPUCycleSpeed();
            var ram = Cosmos.Core.CPU.GetAmountOfRAM();
            Utils.syetem_message("CPU clock speed: " + cspeed + " Mhz");
            Utils.syetem_message("Total ram: " + ram + " MB");
        }
        static void cursor()
        {
            Utils.cursor();
        }
        #endregion
    }
}
