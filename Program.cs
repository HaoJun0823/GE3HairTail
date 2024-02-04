using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace GE3HairTail
{
    internal class Program
    {


        static byte[] vaild_data = { 0x80, 0x07, 0x00, 0x00, 0x38, 0x04, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x80, 0x3F, 0x80, 0x07, 0x00, 0x00, 0x38, 0x04, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 };
        static byte[] screen_219_data = { 0x00, 0x0A, 0x00, 0x00, 0x38, 0x04, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x80, 0x3F, 0x80, 0x07, 0x00, 0x00, 0x38, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 };
        static byte[] screen_219_4K_data = { 0x00, 0x0A, 0x00, 0x00, 0x38, 0x04, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x80, 0x3F, 0x80, 0x07, 0x00, 0x00, 0x38, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 };

        static long base_address = 0x00007FF79F5AADB0;

        //static Dictionary<string, string> config = new Dictionary<string, string>();

        //static string config_path = Environment.CurrentDirectory + "\\GE3HairTail.ini";


        static string _219_path = Environment.CurrentDirectory + "\\4K.txt";
        static string _debug_path = Environment.CurrentDirectory + "\\debug.txt";

        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess,
    IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
          byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        static void Main(string[] args)
        {

            Console.WriteLine("GE3HaierTail (Support 21:9 Screen) By:HaoJun0823 https://www.haojun0823.xyz/");
            Console.WriteLine($"Enable 4K 21:9 -> Create A TXT File At {_219_path}");
            Console.WriteLine($"DEBUG Create A TXT File At {_debug_path}");

            if (File.Exists(_debug_path))
            {
                Console.WriteLine("DEBUG ON!");
            }

            var normal_color = Console.ForegroundColor;

            Process p = new Process();

            p.StartInfo.FileName = Environment.CurrentDirectory + "\\ge3.exe";

            foreach (string arg in args)
            {

                p.StartInfo.Arguments += arg;

            }


            Console.WriteLine($"Call {p.StartInfo.FileName}{p.StartInfo.Arguments}");


            p.Start();

            //SuspendThread(p.Handle);

            byte[] read_data = new byte[32];
            IntPtr readbyte;

            ReadProcessMemory(p.Handle, (IntPtr)base_address, read_data, 32, out readbyte);

            Console.WriteLine($"Read At:{base_address.ToString("X16")}");

            Console.ForegroundColor = ConsoleColor.Green;

            for (int i = 0;i<vaild_data.Length;i++)
            {
                if (vaild_data[i] == read_data[i])
                {
                    Console.Write($"{read_data[i].ToString("X2")} ");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine();
                    throw new Exception($"ERROR:{vaild_data[i].ToString("X2")}!={read_data[i].ToString("X2")} At {i}");

                }
            }

            Console.ForegroundColor = normal_color;

            Console.WriteLine();

            Console.WriteLine("PASS!");

            int writebyte = 0;
            var write_data = screen_219_data;
            if (File.Exists(_219_path))
            {
                Console.Write("Loaded:4K 21:9 ");
                write_data = screen_219_4K_data;
            }
            else
            {
                Console.Write("Loaded:2K 21:9 ");
            }
            

            


            WriteProcessMemory(p.Handle, (IntPtr)base_address, write_data, 32, ref writebyte);


            ReadProcessMemory(p.Handle, (IntPtr)base_address, read_data, 32, out readbyte);

            Console.WriteLine($"Write At:{base_address.ToString("X16")}");

            Console.ForegroundColor = ConsoleColor.Blue;

            for (int i = 0; i < vaild_data.Length; i++)
            {
                if (vaild_data[i] != read_data[i])
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;

                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }

                Console.Write($"{read_data[i].ToString("X2")} ");
            }

            Console.ForegroundColor = normal_color;

            Console.WriteLine();


            Console.WriteLine("Done!");

            if (File.Exists(_debug_path))
            {
                Console.WriteLine("DEBUG:Press Any Key To Exit Console.");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("INFO:Console Will Be Close After 3s!");
                Thread.Sleep(3000);
            }
            
            
            



        }

        //static void ReadConfig()
        //{
        //    Dictionary<string,string> local_config = new Dictionary<string, string>();


        //    if (!File.Exists(config_path))
        //    {
                
        //    }

        //    var strings = File.ReadAllLines(config_path);

        //    foreach(var i in strings)
        //    {

        //        var kv = i.Split('=');

        //        if (local_config.ContainsKey(kv[0]))
        //        {
        //            local_config[kv[0]] = kv[1];
        //        }
        //        else
        //        {
        //            local_config.Add(kv[0], kv[1]);
        //        }

        //    }



        //}

        //static void WriteConfig()
        //{

        //    List<string> list = new List<string>();

        //    list.Add("DEBUG=False");
        //    list.Add("");


        //    File.WriteAllLines(config_path,);
        //}

    }
}
