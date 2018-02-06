using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoExportRedeem
{
    class DirectoryModule
    {
        private string username;

        private string password;

        private string targetDrive;

        private string targetDriveSlash;

        private string localFolder;

        private string localFolderSlash;

        private string networkPath;

        private string networkPathSlash;

        public bool isCreateMapDrive;

        public DirectoryModule(string localFolder, string networkPath, string username, string password)
        {
            if (!localFolder.EndsWith(@"\"))
            {
                this.localFolderSlash = localFolder + @"\";
                this.localFolder = localFolder;
            }
            else
            {
                this.localFolderSlash = localFolder;
                this.localFolder = localFolder.Substring(0, localFolder.Length - 1);
            }

            if (!networkPath.EndsWith(@"\"))
            {
                this.networkPathSlash = networkPath + @"\";
                this.networkPath = networkPath;
            }
            else
            {
                this.networkPathSlash = networkPath;
                this.networkPath = networkPath.Substring(0, networkPath.Length - 1);
            }

            for (int i = 0; i < 5; i++)
            {
                bool isOK = false;
                switch (i)
                {
                    case 0: if (!Directory.Exists("P:"))
                        {
                            this.targetDrive = "P:";
                            this.targetDriveSlash = @"P:\";
                            isOK = true;
                        }
                        break;
                    case 1: if (!Directory.Exists("Q:"))
                        {
                            this.targetDrive = "Q:";
                            this.targetDriveSlash = @"Q:\";
                            isOK = true;

                        }
                        break;
                    case 2: if (!Directory.Exists("R:"))
                        {
                            this.targetDrive = "R:";
                            this.targetDriveSlash = @"R:\";
                            isOK = true;

                        }
                        break;
                    case 3: if (!Directory.Exists("S:"))
                        {
                            this.targetDrive = "S:";
                            this.targetDriveSlash = @"S:\";
                            isOK = true;

                        }
                        break;
                    case 4: if (!Directory.Exists("T:"))
                        {
                            this.targetDrive = "T:";
                            this.targetDriveSlash = @"T:\";
                            isOK = true;

                        }
                        break;
                    default:
                        break;
                }
                if (isOK)
                {
                    break;
                }
            }


            this.username = username;
            this.password = password;

            this.isCreateMapDrive = false;

            DeleteTempFolder();
            CreateDirectory(this.localFolder);
        }

        public bool isExistNetworkPath()
        {

            if (Directory.Exists(this.networkPathSlash) || Directory.Exists(this.networkPath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CreateDirectory(string path)
        {
            if (path.EndsWith(@"\"))
            {
                path = path.Substring(0, path.Length - 1);
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public int TryCreateMapDrive()
        {
            if (String.IsNullOrEmpty(this.username) || String.IsNullOrEmpty(this.password))
            {
                return 2;
            }
            else
            {
                string command2 = "NET USE " + this.targetDrive + " ";
                command2 += "\"" + this.networkPath + "\"" + " /user:" + this.username + " " + this.password;
                //Console.WriteLine(command2);
                int exitCodeNetUse = ExecuteCommand(command2, 15000);
                if (exitCodeNetUse != 0)
                {
                    return 1;
                }
                else
                {
                    this.isCreateMapDrive = true;
                    return 0;
                }
            }

        }

        //add new 1/25/2017
        //might not use
        public void SetTargetDriveExtra()
        {
            this.targetDrive = this.networkPath;
            this.targetDriveSlash = this.networkPathSlash;
        }

        public bool MoveAllExportFile()
        {
            if (this.isCreateMapDrive)
            {
                string command = "xcopy " + "\"" + this.localFolder + "\"" + " " + "\"" + this.targetDriveSlash + "\"" + @" /Y /S";
                int exitCode = ExecuteCommand(command, 180000);
                Thread.Sleep(2000);
                if (exitCode != 0)
                {
                    string command2 = "NET USE " + this.targetDrive + " /delete";
                    ExecuteCommand(command2, 15000);
                    Management.Instance.WriteLog(System_Type.Transfer_Data, "== TRANSFER NOT SUCCESSFUL");
                    return false;
                }
                else
                {
                    string command2 = "NET USE " + this.targetDrive + " /delete";
                    ExecuteCommand(command2, 15000);
                    Management.Instance.WriteLog(System_Type.Transfer_Data, "== TRANSFER SUCCESSFUL");
                    return true;
                }

            }
            else
            {
                string command = "xcopy " + "\"" + this.localFolder + "\"" + " " + "\"" + this.networkPathSlash + "\"" + @" /Y /S";
                int exitCode = ExecuteCommand(command, 180000);
                Thread.Sleep(2000);
                if (exitCode != 0)
                {
                    Management.Instance.WriteLog(System_Type.Transfer_Data, "== TRANSFER NOT SUCCESSFUL");
                    return false;
                }
                else
                {
                    Management.Instance.WriteLog(System_Type.Transfer_Data, "== TRANSFER SUCCESSFUL");
                    return true;
                }
            }
        }

        public void DeleteTempFolder()
        {
            if (Directory.Exists(this.localFolder))
            {
                DeleteDirectory(this.localFolder);
            }
        }

        public void DeleteDirectory(string path)
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                DeleteDirectory(directory);
            }
            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
        }

        private int ExecuteCommand(string command, int timeout)
        {
            var processInfo = new ProcessStartInfo("cmd.exe", "/C " + command)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                //RedirectStandardOutput = true,
                //RedirectStandardInput = true,
                //RedirectStandardError = true,
                WorkingDirectory = "C:\\",
            };

            var process = Process.Start(processInfo);
            process.WaitForExit(timeout);
            var exitCode = process.ExitCode;
            process.Close();
            return exitCode;
        }
    }
}
