﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GW2EIParser.Exceptions;

namespace GW2EIParser
{
    public class ConsoleProgram
    {
        public ConsoleProgram(IEnumerable<string> logFiles)
        {
            foreach (string file in logFiles)
            {
                ParseLog(file);
            }
        }

        private void ParseLog(object logFile)
        {
            var row = new GridRow(logFile as string, "Ready to parse")
            {
                BgWorker = new System.ComponentModel.BackgroundWorker()
                {
                    WorkerReportsProgress = true
                }
            };
            try
            {
                ProgramHelper.DoWork(row);
            }
            catch (CancellationException ex)
            {
                Console.WriteLine(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            catch (Exception)
            {
                Console.WriteLine("Something terrible has happened");
            }

        }
    }
}
