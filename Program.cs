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

    public static class ProcessExtension
    {


        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);

        public static void Suspend(this Process process)
        {
            foreach (ProcessThread thread in process.Threads)
            {
                var pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                if (pOpenThread == IntPtr.Zero)
                {
                    break;
                }
                SuspendThread(pOpenThread);
            }
        }
        public static void Resume(this Process process)
        {
            foreach (ProcessThread thread in process.Threads)
            {
                var pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                if (pOpenThread == IntPtr.Zero)
                {
                    break;
                }
                ResumeThread(pOpenThread);
            }
        }
    }

    internal class Program
    {


        static byte[] vaild_data = { 0x80, 0x07, 0x00, 0x00, 0x38, 0x04, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x80, 0x3F, 0x80, 0x07, 0x00, 0x00, 0x38, 0x04, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 };
        static byte[] screen_219_data = { 0x00, 0x0A, 0x00, 0x00, 0x38, 0x04, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x80, 0x3F, 0x80, 0x07, 0x00, 0x00, 0x38, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 };
        static byte[] screen_219_4K_data = { 0x00, 0x0A, 0x00, 0x00, 0x38, 0x04, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x80, 0x3F, 0x80, 0x07, 0x00, 0x00, 0x38, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 };

        //static long base_address = 0x00007FF79F5AADB0;
        //static IntPtr base_address;
        //static Dictionary<string, string> config = new Dictionary<string, string>();

        //static string config_path = Environment.CurrentDirectory + "\\GE3HairTail.ini";


        static string _219_path = Environment.CurrentDirectory + "\\4K.txt";
        static string _debug_path = Environment.CurrentDirectory + "\\debug.txt";

        [DllImport("kernel32.dll",SetLastError =true)]
        static extern bool ReadProcessMemory(IntPtr hProcess,
    IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
          byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        static extern uint GetLastError();

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
        ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

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

            Console.WriteLine("Start Game.");

            if (!File.Exists(p.StartInfo.FileName))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                //throw new Exception($"ERROR:{vaild_data[i].ToString("X2")}!={read_data[i].ToString("X2")} At {i}");
                Console.WriteLine($"ERROR Game, Please check game or report this problem.");
                Console.WriteLine("DEBUG:Press Any Key To Exit Console.");
                Console.ReadKey();
                return;
            }

            p.Start();


            while (true)
            {
                try
                {
                    var time = p.StartTime;
                    break;
                }
                catch (Exception) {

                    
                }
            };
            Console.WriteLine("Game Running!");
            p.Suspend();

            //Console.WriteLine($"BaseAddress:{p.MainModule.BaseAddress.ToString("X16")}");


            //base_address = IntPtr.Add(p.MainModule.BaseAddress,target_address);
            //Console.WriteLine($"Address:{p.MainModule.BaseAddress.ToString("X16")}+{target_address.ToString("X8")}->{base_address.ToString("X16")}");


            SigScanSharp scan = new SigScanSharp(p.Handle);
            scan.SelectModule(p.MainModule);
            Console.WriteLine($"Search:\n{GetHexBytes(vaild_data)}");
            long timer;
            ulong base_address = scan.FindPattern(GetHexBytes(vaild_data), out timer);
            
            Console.WriteLine($"Found address at {base_address.ToString("X16")},cost:{timer}s!");

            var target = OpenProcess(ProcessAccessFlags.All, false,p.Id);

            byte[] read_data = new byte[32];
            IntPtr readbyte;


            var r_b = ReadProcessMemory(target, (IntPtr)base_address, read_data, read_data.Length, out readbyte);

            Console.WriteLine($"Read At:{base_address.ToString("X16")},Out:{readbyte},Bool:{r_b}");

            if (!r_b)
            {
                Console.WriteLine($"Last Error:{GetLastError()}");
            }
            

            Console.ForegroundColor = ConsoleColor.Green;


            bool isVaild = true;



            for (int i = 0;i<vaild_data.Length;i++)
            {
                if (vaild_data[i] == read_data[i])
                {
                    Console.Write($"{read_data[i].ToString("X2")} ");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"{read_data[i].ToString("X2")} ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    isVaild = false;

                    

                }
            }

            Console.WriteLine();

            if (!isVaild)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                //throw new Exception($"ERROR:{vaild_data[i].ToString("X2")}!={read_data[i].ToString("X2")} At {i}");
                Console.WriteLine($"ERROR Data, Please check game or report this problem.");
                Console.WriteLine("DEBUG:Press Any Key To Exit Console.");
                //p.Kill();
                Console.ReadKey();
                
                return;
            }


            Console.ForegroundColor = normal_color;

            

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
            

            


            WriteProcessMemory(target, (IntPtr)base_address, write_data, 32, ref writebyte);

            p.Resume();

            ReadProcessMemory(target, (IntPtr)base_address, read_data, 32, out readbyte);

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


        static string GetHexBytes(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();

            for(int i=0;i<bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("X2"));
                if (i+1 == bytes.Length)
                {

                }
                else
                {
                    sb.Append(' ');
                }
            }



            return sb.ToString();



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
