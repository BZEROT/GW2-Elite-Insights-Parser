﻿using LuckParser.Controllers;
using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuckParser
{
    public class ConsoleProgram
    {
        public ConsoleProgram(string[] args)
        {
            if (Properties.Settings.Default.ParseOneAtATime)
            {
                foreach (string file in args)
                {
                    ParseLog(file);
                }
            }
            else
            {
                List<Task> tasks = new List<Task>();

                foreach (string file in args)
                {
                    tasks.Add(Task.Factory.StartNew(ParseLog, file));
                }

                Task.WaitAll(tasks.ToArray());
            }
        }

        private void ParseLog(object logFile)
        {
            GridRow row = new GridRow(logFile as string, "")
            {
                BgWorker = new System.ComponentModel.BackgroundWorker()
                {
                    WorkerReportsProgress = true
                }
            };
            row.Metadata.FromConsole = true;

            FileInfo fInfo = new FileInfo(row.Location);
            if (!fInfo.Exists)
            {
                throw new CancellationException(row, new FileNotFoundException("File does not exist", fInfo.FullName));
            }

            Controller1 control = new Controller1();

            if (fInfo.Extension.Equals(".evtc", StringComparison.OrdinalIgnoreCase) ||
                fInfo.Name.EndsWith(".evtc.zip", StringComparison.OrdinalIgnoreCase))
            {
                //Process evtc here
                control.ParseLog(row, fInfo.FullName);

                //Creating File
                //save location
                DirectoryInfo saveDirectory;
                if (Properties.Settings.Default.SaveAtOut || Properties.Settings.Default.OutLocation == null)
                {
                    //Default save directory
                    saveDirectory = fInfo.Directory;
                }
                else
                {
                    //Customised save directory
                    saveDirectory = new DirectoryInfo(Properties.Settings.Default.OutLocation);
                }

                string bossid = control.getBossData().getID().ToString();
                string result = "fail";

                if (control.getLogData().getBosskill())
                {
                    result = "kill";
                }

                SettingsContainer settings = new SettingsContainer(Properties.Settings.Default);

                string outputType = Properties.Settings.Default.SaveOutHTML ? "html" : "csv";
                string outputFile = Path.Combine(
                    saveDirectory.FullName,
                    $"{fInfo.Name}_{HTMLHelper.GetLink(bossid + "-ext")}_{result}.{outputType}"
                );

                using (FileStream fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        if (Properties.Settings.Default.SaveOutHTML)
                        {
                            HTMLBuilder builder = new HTMLBuilder(control.GetParsedLog(), settings);
                            builder.CreateHTML(sw);
                        }
                        else
                        {
                            CSVBuilder builder = new CSVBuilder(control.GetParsedLog(), settings);
                            builder.CreateCSV(sw, ",");
                        }
                    }
                }

            }
            else
            {
                throw new CancellationException(row, new InvalidDataException("Not EVTC"));
            }
        }
    }
}
