using System.Xml.Linq;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using System;
using System.Diagnostics;
namespace Question_Getter_Front_End
{
    class main
    {
        static void Main()
        {
            Wnlib.WNCommon.path = "C:\\Program Files\\Word Net\\dict\\";
            FileSystemWatcher fsw = new FileSystemWatcher("C:\\QGData\\");
            fsw.Filter = "QGRequest.txt";
            fsw.NotifyFilter = NotifyFilters.LastWrite;
            fsw.Changed += new FileSystemEventHandler(OnChanged);
            fsw.EnableRaisingEvents = true;
            Console.WriteLine("monitoring filesystem...");
            Global_var.GlobalVar = true;
            while (Global_var.GlobalVar) ;
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            ((FileSystemWatcher)source).EnableRaisingEvents = false;
            Console.WriteLine("received request...");
            int n;
            int x;
            int y;
            bool canOpen = true;
            string request;
            string action;
            string title;
            TextWriter writer;
            WikiInterface w = new WikiInterface();
            Console.WriteLine("opening file...");
            while (!canOpen)
            {
                canOpen = true;
                try
                {
                    FileStream stream = File.Open("C:\\QGData\\QGRequest.txt", FileMode.Open, FileAccess.ReadWrite);
                    stream.Close();
                }
                catch (IOException)
                {
                    canOpen = false;
                }
            }
            canOpen = false;
            while (!canOpen)
            {
                canOpen = true;
                try
                {
                    FileStream stream = File.Open("C:\\QGData\\QGRequest.txt", FileMode.Open, FileAccess.ReadWrite);
                    stream.Close();
                }
                catch (IOException)
                {
                    canOpen = false;
                }
            }
            FileStream MyStream = File.Open("C:\\QGData\\QGRequest.txt", FileMode.Open, FileAccess.ReadWrite);
            TextReader reader = new StreamReader(MyStream);
            request = reader.ReadToEnd();
            request = request.Trim();
            action = request.Substring(0, request.IndexOf('?'));
            if (action.Equals("getNumBackLinks"))
            {

                Console.WriteLine("request is for Back Links, getting requested numbers...");
                x = request.IndexOf('&') + 1;
                y = request.IndexOf('&', x);
                writer = new StreamWriter(MyStream,System.Text.Encoding.UTF8);
                writer.WriteLine();
                while (y > 0)
                {
                    title = request.Substring(x, y - x);
                    n = w.getNumBackLinks(title);
                    writer.WriteLine(title + ":" + n);
                    x = y + 1;
                    y = request.IndexOf('&', x);
                }
                title = request.Substring(x);
                n = w.getNumBackLinks(title);
                writer.WriteLine(title + ":" + n);
                writer.Close();
                Console.WriteLine("numbers saved in QGRequest.txt...");
                MyStream.Close();
                File.Create("C:\\QGData\\move");
                Process p = new Process();
                p.StartInfo.FileName = "C:\\QGData\\finish.bat";
                p.Start();
                while (!File.Exists("C:\\QGData\\fin"));
                    File.Delete("C:\\QGData\\move");
                    File.Delete("C:\\QGData\\fin");
                ((FileSystemWatcher)source).EnableRaisingEvents = true;
                Console.WriteLine("monitoring filesystem...");
            }
            else if (action.Equals("randomTest"))
            {
                Console.WriteLine("performing random test...");
                string write;
                StreamWriter sw;
                for (x = 1; x <= 10; x++)
                {
                    write = w.getSite(WikiInterface.randUrl, true);
                    (sw = new StreamWriter("C:\\QGData\\wiki" + x + ".txt")).Write(write);
                    sw.Close();
                    Console.WriteLine((x * 10) + "% finished...");
                }
                Console.WriteLine("test complete");
                MyStream.Close();
                File.Create("C:\\QGData\\move");
                Process p = new Process();
                p.StartInfo.FileName = "C:\\QGData\\finish.bat";
                p.Start();
                while (!File.Exists("C:\\QGData\\fin")) ;
                File.Delete("C:\\QGData\\move");
                File.Delete("C:\\QGData\\fin");
                ((FileSystemWatcher)source).EnableRaisingEvents = true;
                Console.WriteLine("monitoring filesystem...");
            }
            else if (action.Equals("getTopic"))
            {
                Console.WriteLine("getting types...");
                string type;
                x = request.IndexOf('&') + 1;
                y = request.IndexOf('&', x);
                writer = new StreamWriter(MyStream);
                writer.WriteLine();
                while (y > 0)
                {
                    title = request.Substring(x, y - x);
                    type = w.sortTopic(title);
                    writer.WriteLine(title + ":" + type);
                    x = y + 1;
                    y = request.IndexOf('&', x);
                }
                title = request.Substring(x);
                type = w.sortTopic(title);
                writer.WriteLine(title + ":" + type);
                writer.Close();
                Console.WriteLine("types saved in QGRequest.txt...");
                MyStream.Close();
                File.Create("C:\\QGData\\move");
                Process p = new Process();
                p.StartInfo.FileName = "C:\\QGData\\finish.bat";
                p.Start();
                while (!File.Exists("C:\\QGData\\fin")) ;
                File.Delete("C:\\QGData\\move");
                File.Delete("C:\\QGData\\fin");
                ((FileSystemWatcher)source).EnableRaisingEvents = true;
                Console.WriteLine("monitoring filesystem...");
            }
            else if (action.Equals("getSite"))
            {
                Console.WriteLine("Getting site...");
                string name = request.Substring(request.IndexOf("?")+2);
                string site = w.getSite(WikiInterface.wUrl+"action=query&format=xml&titles="+name,true);
                MyStream.Close();
                File.Delete("C:\\QGData\\QGRequest.txt");
                StreamWriter write = File.CreateText("C:\\QGData\\QGRequest.txt");
                write.Write(site);
                write.Close();
                Process p = new Process();
                File.Create("C:\\QGData\\move");
                p.StartInfo.FileName = "C:\\QGData\\finish.bat";
                p.Start();
                Console.WriteLine("waiting for signal from back end...");
                while (!File.Exists("C:\\QGData\\fin")) ;
                File.Delete("C:\\QGData\\move");
                File.Delete("C:\\QGData\\fin");
                ((FileSystemWatcher)source).EnableRaisingEvents = true;
                Console.WriteLine("monitoring filesystem...");
            }
            else if (action.Equals("getBackLinks"))
            {
                writer = new StreamWriter(MyStream);
                foreach (string[] str in w.getBackLinks(request.Substring(request.IndexOf("?") + 2)))
                {
                    foreach (string str2 in str)
                    {
                        if(str2!=null)
                            writer.WriteLine(str2);
                    }
                }
                writer.Close();
                MyStream.Close();
                Process p = new Process();
                File.Create("C:\\QGData\\move");
                p.StartInfo.FileName = "C:\\QGData\\finish.bat";
                p.Start();
                Console.WriteLine("waiting for signal from back end...");
                while (!File.Exists("C:\\QGData\\fin")) ;
                File.Delete("C:\\QGData\\move");
                File.Delete("C:\\QGData\\fin");
                ((FileSystemWatcher)source).EnableRaisingEvents = true;
                Console.WriteLine("monitoring filesystem...");

            }
            else
            {
                File.Create("C:\\QGData\\move");
                Process p = new Process();
                p.StartInfo.FileName = "C:\\QGData\\finish.bat";
                p.Start();
                while (!File.Exists("C:\\QGData\\fin")) ;
                File.Delete("C:\\QGData\\move");
                File.Delete("C:\\QGData\\fin");
                ((FileSystemWatcher)source).EnableRaisingEvents = true;
                Console.WriteLine("monitoring filesystem...");
                Global_var.GlobalVar = false;
                Console.WriteLine("closing...");
                Console.WriteLine("Goodbye");
            }
        }
    }

    public static class Global_var
    {
        private static bool global_bool = false;

        public static bool GlobalVar
        {
            get { return global_bool; }
            set { global_bool = value; }
        }
    }
}