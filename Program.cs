﻿using System;
using System.IO;
using z80;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Initializing memory");
            var ram = new byte[65536];
            Array.Clear(ram, 0, ram.Length);

            Console.WriteLine("Loading memory");
            try
            {
                File.ReadAllBytes("code.bin").CopyTo(ram, 0);
            }
            catch (System.Exception)
            {
                Console.WriteLine("ERROR: Unable to read code.bin");
                Console.WriteLine("Press RETURN to exit.");
                while(Console.Read() == -1) {}
                return;
            }
            var mem = new Memory(ram, 0);

            Console.WriteLine("Initializing I/O ports");
            var ports = new SimplePorts();

            Console.WriteLine("Initializing CPU");
            var cpu = new Z80(mem, ports);

            Console.WriteLine("Starting CPU" + Environment.NewLine);
            Console.WriteLine("Press 'T' to terminate emulation" + Environment.NewLine);
            //Console.WriteLine("Press 'S' to single step and 'R' to run continuously" + Environment.NewLine);

            ConsoleKeyInfo keystroke;
            bool singleStep = true;
            while(!cpu.Halt)
            {
                Console.WriteLine(cpu.DumpState());
                keystroke = Console.ReadKey(true);

                if(keystroke.Key == ConsoleKey.T)
                {
                    Console.WriteLine("Terminating emulation" + Environment.NewLine);
                    break;
                }

                cpu.Parse();
                
            }

            Console.WriteLine("Press RETURN to exit.");
            while(Console.Read() == -1) {}
        }
    }

    public class SimplePorts : IPorts 
    {
        public byte Data => 0x00;

        public bool MI => false;

        public bool NMI => false;

        public byte ReadPort(ushort address)
        {
            address &= 0x00FF;          ///Ignore upper byte, unused by port simulator
            byte returnValue = 0x00;
            switch (address)
            {
                default:
                    Console.WriteLine("Reading from unmapped port 0x{0:X4}", address);
                    break;
            }
            return returnValue;
        }

        public void WritePort(ushort address, byte value)
        {
            address &= 0x00FF;      ///Ignore upper byte, unused by port simulator
            switch (address)
            {
                case 0x00:      ///PORT 0x00, CHAR OUTPUT
                    Console.Write((char)value);
                    break;
                    
                default:
                Console.WriteLine("Writing to unmapped port 0x{0:X4}, data 0x{1:X4}", address, value);
                    break;
            }
        }
    }
}
