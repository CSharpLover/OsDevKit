using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OsDevKit
{
    public static class Compiler
    {
        public static string Log = "";

        public static void Compile()
        {
            foreach (var i in Directory.GetFiles("Factory\\Build"))
            {
                File.Delete(i);
            }


            if(Global.CurrentProjectFile == null)
            {
                return;
            }
            Global.OutPut = "";
            Log = "";
            foreach (var i in Global.CurrentProjectFile.Files)
            {
                if(i.EndsWith(".c"))
                {
                    StartProcces("Factory\\CompileC.bat", "" + Path.GetFullPath( Path.Combine(Global.CurrentProjectFilePath, "files" ,i)) + " " + new FileInfo(i).Name.Split('.').First() + " " + Path.GetFullPath(Path.Combine(Global.CurrentProjectFilePath,"files", "include")) , "Factory");
                }
                if (i.EndsWith(".c++") || i.EndsWith(".cpp"))
                {
                    StartProcces("Factory\\CompileC++.bat", "" + Path.GetFullPath(Path.Combine(Global.CurrentProjectFilePath, "files", i)) + " " + new FileInfo(i).Name.Split('.').First() + " " + Path.GetFullPath(Path.Combine(Global.CurrentProjectFilePath, "files", "include")), "Factory");
                }
                if (i.EndsWith(".bas"))
                {
                    StartProcces("Factory\\CompileFreeBasic.bat", "" + Path.GetFullPath(Path.Combine(Global.CurrentProjectFilePath, "files", i)) + " " + new FileInfo(i).Name.Split('.').First() + " " + Path.GetFullPath(Path.Combine(Global.CurrentProjectFilePath, "files", "include")), "Factory");
                }
                if (i.EndsWith(".asm"))
                {
                    StartProcces("Factory\\CompileAsm.bat", "" + Path.GetFullPath(Path.Combine(Global.CurrentProjectFilePath, "files", i)) + " " + new FileInfo(i).Name.Split('.').First(), "Factory");

                }
            }
            StartProcces("Factory\\Link.bat","", Path.GetFullPath("Factory"));
            StartProcces("Factory\\BuildBootImage.bat", "", Path.GetFullPath("Factory"));
            var img = Path.Combine(Global.CurrentProjectFilePath, "Bin", "Boot.img");
            File.Delete(img);
            File.Copy("Factory\\Boot.img",img);
            Application.DoEvents();
            StartProcces("Factory\\Qemu.bat", img, Path.GetFullPath("Factory"),false);

        }

        public static void StartProcces(string name, string args, string path, bool waitfor = true)
        {


            System.Diagnostics.Process p = new System.Diagnostics.Process();
            // p.StartInfo.WorkingDirectory = new DirectoryInfo( path).FullName ;




             p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

            p.StartInfo.FileName = name;
            p.StartInfo.Arguments = args;

            p.Start();
            if (waitfor)
            {
                p.WaitForExit();
            }
            else
            {
                return;
            }

            var x =  p.StandardOutput.ReadToEnd() + "\n" + p.StandardError.ReadToEnd();
            var ret = "";
            string opt = "-w -m32 -Wall -O -fstrength-reduce  -finline-functions -fomit-frame-pointer -nostdinc -fno-builtin -I " + args.Split(' ').Last() +" -c -fno-strict-aliasing -fno-common -fno-stack-protector";
            foreach (var i in x.Replace("\r\n", "\n").Split('\n'))
            {
                var l = i.Split('>').Last();
                if (!l.StartsWith("cd ") && !l.StartsWith("del ") && !l.StartsWith("set") && !string.IsNullOrEmpty(l))
                {
                    ret += l.Replace(opt ,"{opt}") + Environment.NewLine;
                }
            }
           
            Global.OutPut += "----------------------------------------\n" + ret + "\n\n";
        }
    }
}
