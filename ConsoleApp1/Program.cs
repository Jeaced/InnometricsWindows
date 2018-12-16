using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = System.Environment.CurrentDirectory + @"\session.txt";
            string session, password, name, lastName, email;
            string currentActivity = "", newActivity = "";
            long startTime, endTime;
            MyHttpClient.SetUp();

            if (System.IO.File.Exists(path))
            {
                System.IO.StreamReader file = new System.IO.StreamReader(path);
                session = file.ReadLine();
            } else
            {
                Console.WriteLine("Enter your credentials");
                Console.Write("First name: ");
                name = Console.ReadLine();
                Console.Write("Last name: ");
                lastName = Console.ReadLine();
                Console.Write("Email: ");
                email = Console.ReadLine();
                Console.Write("Password: ");
                password = ReadPassword();
                //Console.WriteLine("The password was " + password);
                User user = new User
                {
                    Name = name,
                    LastName = lastName,
                    Email = email,
                    Password = password
                };
                session = MyHttpClient.CreateUserAsync(user).Result;
                System.IO.File.WriteAllText(path, session);
            }

            currentActivity = GetActiveWindowTitle();
            startTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

            while(true)
            {
                //Console.WriteLine(GetActiveWindowTitle());
                newActivity = GetActiveWindowTitle();
                if (!currentActivity.Equals(newActivity)) {
                    currentActivity = newActivity;
                    endTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    Activity activity = new Activity
                    {
                        StartTime = startTime.ToString(),
                        ExecutableName = currentActivity,
                        EndTime = endTime.ToString(),
                        BrowserUrl = "browser_url",
                        BrowserTitle = "browser_title",
                        IpAddress = "ip_address",
                        MacAddress = "mac_address"
                    };
                    MyHttpClient.SendActivityAsync(activity, session);

                }
                System.Threading.Thread.Sleep(1000);
            }
        }



        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        private static string ReadPassword()
        {
            string pass = "";
            do
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Substring(0, (pass.Length - 1));
                        Console.Write("\b \b");
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                }
            } while (true);

            return pass;
        }
    }

}
