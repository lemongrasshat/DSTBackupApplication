using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using System.IO;
/*
 * Author: Lemongrasshat (lemongrasshat@gmail.com)
 * Program: Command line backup application for Don't starve together.
 *Todo:
 * Shortcut key setting to take backup while program is running in background
 */
namespace backupdstapplication
{
    class DSTBACKUP
    {
        private List<string> Clusternames = new List<string>();
        private Dictionary<string, int> savenames = new Dictionary<string, int>();
        private string Currentusername = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        string[] CleanUsername;

        //Code for copying save folder to backup folder.
        //Source : https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }
        // Function to find total no Cluster of saves from documents folder
        private int CheckBackupSaves()
        {
            
            CleanUsername = this.Currentusername.Split('\\');
            int ClusterCount = 0;
            int directoryCount = System.IO.Directory.GetDirectories(@"C:\Users\"+this.CleanUsername[1]+@"\Documents\Klei\DoNotStarveTogether").Length;
            string[] directoryInfo = System.IO.Directory.GetDirectories(@"C:\Users\" + this.CleanUsername[1] + @"\Documents\Klei\DoNotStarveTogether");

            for (int i = 0; i < directoryCount; i++)
            {
                string clean = Regex.Replace(directoryInfo[i], @"C:\\.*[^0-9]", " ");

                if (!String.Equals(clean, "backup") && !String.IsNullOrWhiteSpace(clean))
                {
                    ClusterCount++;
                    clean=clean.Trim();
                    this.Clusternames.Add(clean);
                }
            }
            if (ClusterCount <= 0)
            {
                return 0;
            }
            else
            {
                return ClusterCount;
            }
        }
        private void FindSaveClusters()
        {
            string line;
            int TotalClusters=this.Clusternames.Count;
            for (int i = 0; i<TotalClusters; i++)
            {
                Console.WriteLine("For Cluster" + (i+1) +" Following files are present ---->"+ "\n");
                Console.WriteLine("--------------------------------------------------------");
                int ClusterCount = System.IO.Directory.GetDirectories(@"C:\Users\" + this.CleanUsername[1] + @"\Documents\Klei\DoNotStarveTogether\"+ this.Clusternames[i]+@"\").Length;
                string[] directoryInfo = System.IO.Directory.GetDirectories(@"C:\Users\" + this.CleanUsername[1] + @"\Documents\Klei\DoNotStarveTogether\" + this.Clusternames[i] + @"\");
                for (int x = 0; x < directoryInfo.Length; x++)
                {
                    if (directoryInfo[x].Contains("Cluster_"))
                    {
                        string ClusterNumber = directoryInfo[x].Substring(directoryInfo[x].IndexOf("Cluster"));
                        ClusterNumber=Regex.Replace(ClusterNumber, @"[a-zA-Z_]*", " ");
                        try
                        {
                            StreamReader reader = new StreamReader(directoryInfo[x] + @"\cluster.ini");
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (line.Contains("cluster_name"))
                                {
                                    Console.WriteLine(Convert.ToInt32(ClusterNumber)+ ":"+line +"\n");
                                    savenames.Add(line, Convert.ToInt32(ClusterNumber));
                                    break;
                                }
                            }
                        }
                        catch (FileNotFoundException )
                        {
                            Console.WriteLine("No Save File for : " + directoryInfo[x]+ "\n");
                        }
                        
                    }
                }
                Console.WriteLine("--------------------------------------------------------"+"\n");
            }
            Console.WriteLine("Choose Cluster to take backup from:");
            int Clusterchoice =Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Choose Save file : ");
            int Savechoice = Convert.ToInt32(Console.ReadLine());
            if (Clusterchoice <= 0)
            {
                Console.WriteLine("Invalid Input");
            }
            else
            {
                string source = @"C:\Users\" + this.CleanUsername[1] + @"\Documents\Klei\DoNotStarveTogether\" + this.Clusternames[(Clusterchoice - 1)] + @"\Cluster_" + Savechoice;
                string destination = @"C:\Users\Shree\Desktop\DSTBACKUP\"+ this.Clusternames[(Clusterchoice - 1)] +"_Cluster_" +Savechoice+"_"+ DateTime.Now.ToString("yyyyMMddHHmmssfff");
                DirectoryCopy(source,destination,true);
            }
        }

        static void Main(string[] args)
        {
            int SaveCount;
            DSTBACKUP mainobj = new DSTBACKUP();

            mainobj.greetings();
            SaveCount = mainobj.CheckBackupSaves();
            if (SaveCount == 0)
            {
                Console.WriteLine("No Save games exist!"+ "\n");
            }
            else
            {
                Console.WriteLine("Total saved game folder found:" + SaveCount +"\n");
                mainobj.FindSaveClusters();
            }
            Console.Write("Copying successful!");
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }

        private void greetings()
        {
            Console.WriteLine(@"
░░▄▀ ░░▄▀ ░░▄▀ ░░▄▀ ░░▄▀ ░░▄▀ ░░▄▀ ░░▄▀ ░░▄▀ ░░▄▀ ░░▄▀ ░░▄▀ ░░▄▀ ░░▄▀ ░░▄▀
▄▀░░ ▄▀░░ ▄▀░░ ▄▀░░ ▄▀░░ ▄▀░░ ▄▀░░ ▄▀░░ ▄▀░░ ▄▀░░ ▄▀░░ ▄▀░░ ▄▀░░ ▄▀░░ ▄▀░░");
            Console.WriteLine(@"
▒█▀▀▄ ▒█▀▀▀█ ▀▀█▀▀ 　 ▒█▀▀█ ░█▀▀█ ▒█▀▀█ ▒█░▄▀ ▒█░▒█ ▒█▀▀█ 　 ▒█▀▀█ █░░█ 
▒█░▒█ ░▀▀▀▄▄ ░▒█░░ 　 ▒█▀▀▄ ▒█▄▄█ ▒█░░░ ▒█▀▄░ ▒█░▒█ ▒█▄▄█ 　 ▒█▀▀▄ █▄▄█ 
▒█▄▄▀ ▒█▄▄▄█ ░▒█░░ 　 ▒█▄▄█ ▒█░▒█ ▒█▄▄█ ▒█░▒█ ░▀▄▄▀ ▒█░░░ 　 ▒█▄▄█ ▄▄▄█");
            Console.WriteLine(@"
█░░ █▀▀ █▀▄▀█ █▀█ █▄░█ █▀▀ █▀█ ▄▀█ █▀ █▀ █░█ ▄▀█ ▀█▀
█▄▄ ██▄ █░▀░█ █▄█ █░▀█ █▄█ █▀▄ █▀█ ▄█ ▄█ █▀█ █▀█ ░█░");
            Console.WriteLine(@"
░░▄▀ ░░▄▀ ░░▄▀ ░░▄▀ ░░▄▀ ░░▄▀ ░░▄▀ ░░▄▀ ░░▄▀ ░░▄▀ ░░▄▀ ░░▄▀ ░░▄▀ ░░▄▀ ░░▄▀
▄▀░░ ▄▀░░ ▄▀░░ ▄▀░░ ▄▀░░ ▄▀░░ ▄▀░░ ▄▀░░ ▄▀░░ ▄▀░░ ▄▀░░ ▄▀░░ ▄▀░░ ▄▀░░ ▄▀░░");
        }
    }
}
