using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using TestResult.Models;

namespace TestResult.Controllers
{
    public class FileSystemController
    {
        string nl = Environment.NewLine;

        /// <summary>
        /// saves the given AppRun, which was read from a logfile, to the database
        /// </summary>
        /// <param name="appRun"></param>
        private void SaveToDatabase(AppRun appRun)
        {
            UnitTestLogEntities context = new UnitTestLogEntities();

            // don't save apprun in db if already exists
            var runAlreadyInDb = from st in context.AppRuns
                                 where st.StartTime == appRun.StartTime
                                     && st.ServerName == appRun.ServerName && st.Alias == appRun.Alias
                                 select st;

            if (runAlreadyInDb.Count() > 0)
                return;

            context.AppRuns.Add(appRun);
            context.SaveChanges();
        }

        /// <summary>
        /// checks directories given in settings
        /// </summary>
        public void checkDirectories()
        {
            String[] paths = new string[] { @"\\VMWREWETCDEV\Apps", @"\\VMWREWETCREL\Apps" };
            // create another list for the files for better debugging
            List<string> files = new List<string>();
            foreach (string path in paths)
            {
                // ignore if directory is not found
                if (!Directory.Exists(path))
                    continue;

                foreach (string filename in Directory.GetFiles(path, "*.log", SearchOption.AllDirectories))
                {
                    // only process unittest log files
                    if (!filename.ToLower().Contains("unittest"))
                        continue;

                    files.Add(filename);
                }
            }

            foreach (string filename in files)
            {
                processFile(filename);
            }
        }

        private void processFile(string filename)
        {
            string backupDir = @"\\VMWREWETCDEV\Test$\Backup\";
            try
            {
                List<AppRun> runs = ReadLogFile(filename);
                bool isSuccessFullyProcessed = false;
                foreach (var run in runs)
                {
                    if (String.IsNullOrWhiteSpace(run.ServerName) || String.IsNullOrWhiteSpace(run.AppArea))
                    {
                        TimeSpan timeDiff = DateTime.Now - File.GetLastWriteTime(filename);

                        // if last writetime of file was longer than one day ago, delete file so it doesn't get processed next time
                        if (timeDiff.Days > 1)
                        {
                            try
                            {
                                File.Delete(filename);
                            }
                            catch
                            {
                                // don't do anything if file is locked
                            }
                        }
                    }
                    else
                    {
                        SaveToDatabase(run);
                        isSuccessFullyProcessed = true;
                    }
                }

                if (!isSuccessFullyProcessed)
                    return;

                // move successfully processed file to backup dir
                if (!Directory.Exists(backupDir))
                    Directory.CreateDirectory(backupDir);
                File.Move(filename, backupDir + Path.GetFileName(filename));

                // write log
                writeLog("successfully processed file [" + filename + "]", true);
            }
            catch (Exception ex)
            {
                // log exception
                writeLog("an error occured while processing file [" + filename + "]:", true);
                writeLog(ex.Message + nl + ex.StackTrace, false);

                // log inner exception if available
                if (ex.InnerException != null)
                    writeLog("inner exception: " + nl + ex.InnerException.Message + nl + ex.InnerException.StackTrace, false);
            }
        }

        /// <summary>
        /// writes message to logfile in given directory
        /// </summary>
        /// <param name="cleanPath"></param>
        /// <param name="message"></param>
        /// <param name="writeDateTime"></param>
        public void writeLog(string message, bool writeDateTime)
        {
            string logFile = @"\\VMWREWETCDEV\Test$\Backup\ServiceLog.txt";

            // try 5 times to write the log
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    using (StreamWriter file = new StreamWriter(logFile, true))
                    {
                        string starter = writeDateTime ? DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " " : 
                            "                    ";
                        file.WriteLine(starter + message);
                    }
                }
                catch
                {
                    // sleep for 1 second and try to write log again
                    Thread.Sleep(1000);
                }
            }
        }

        /// <summary>
        /// processes the given unittest logfile
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private List<AppRun> ReadLogFile(string filename)
        {
            List<AppRun> allRuns = new List<AppRun>();

            AppRun dummyRun = new AppRun();
            TestSuiteRun currentSuite = new TestSuiteRun();
            TestCaseRun currentCase = new TestCaseRun();
            string allErrorLines = "";
            string currentSuiteError = "";

            foreach (string line in File.ReadAllLines(filename))
            {
                // TestCaseRun
                if (line.StartsWith("      "))
                {
                    // start of testcase
                    if (line.Contains(" gestartet"))
                    {
                        currentCase = new TestCaseRun();
                        currentCase.Name = line.Substring(0, line.IndexOf(" gestartet")).Trim();
                        currentSuite.TestCaseRuns.Add(currentCase);
                        //reset current error
                        allErrorLines = "";
                    }
                    // end of testcase
                    else if (line.Contains(" beendet [Dauer: "))
                    {
                        int strStart = line.IndexOf("[Dauer: ") + 8;
                        string duration = line.Substring(strStart, line.IndexOf("]") - strStart).Trim();
                        currentCase.Duration = ConvertDurationString(duration);

                        if (String.IsNullOrWhiteSpace(allErrorLines))
                        {
                            allErrorLines = "";
                            continue;
                        }

                        foreach (string error in allErrorLines.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            TestError err = new TestError();
                            err.Message = error;

                            // remove unnecessary text
                            if (err.Message.Contains(" ist ungleich berechneten Wert ") && err.Message.Contains(", expected: <"))
                                err.Message = err.Message.Substring(0, err.Message.IndexOf(", expected: <") - 1).Trim();
                            
                            currentCase.TestErrors.Add(err);
                        }
                    }
                    // testerror
                    else
                    {
                        string line2 = line.Replace("#Testfall fehlgeschlagen: ", "");
                        line2 = line2.Replace("#Testfall fehlerhaft: ", "");
                        allErrorLines += line2.Trim() + Environment.NewLine;
                    }
                }
                // TestSuiteRun
                else if (line.StartsWith("   "))
                {
                    if (line.Contains(" gestartet mit Alias "))
                    {
                        currentSuite = new TestSuiteRun();
                        int strStart = line.IndexOf(" gestartet mit Alias ");
                        currentSuite.Name = line.Substring(0, strStart).Trim();

                        string dbType = line.Substring(line.IndexOf("[") + 1, line.IndexOf("]") - line.IndexOf("[") - 1).Trim();
                        string alias = line.Substring(strStart + 22, line.IndexOf("[") - strStart - 24).Trim();
                        currentSuiteError = "";

                        List<AppRun> run = allRuns.Where(x => (x.Alias == alias && x.DbType == dbType)).ToList();
                        if (run.Count > 0)
                            run[0].TestSuiteRuns.Add(currentSuite);
                        else
                        {
                            AppRun newRun = new AppRun();
                            newRun.Alias = alias;
                            newRun.DbType = dbType;
                            allRuns.Add(newRun);
                            newRun.TestSuiteRuns.Add(currentSuite);
                        }
                    }
                    else if (line.Contains(" beendet [Dauer: "))
                    {
                        int strStart = line.IndexOf("[Dauer: ") + 8;
                        //TC_BsGui_BusinessObjectMassInsertHelper_01 beendet [Dauer: 1,48 Sekunden]
                        string duration = line.Substring(strStart, line.IndexOf("]") - strStart).Trim();
                        currentSuite.Duration = ConvertDurationString(duration);

                        if (!String.IsNullOrWhiteSpace(currentSuiteError))
                        {
                            TestCaseRun cr;
                            if (currentSuite.TestCaseRuns.Count == 0)
                            {
                                cr = new TestCaseRun();
                                if (currentSuiteError.Contains("TearDown"))
                                    cr.Name = "TearDown";
                                else
                                    cr.Name = "SetUp";

                                cr.Duration = currentSuite.Duration;
                                currentSuite.TestCaseRuns.Add(cr);
                            }
                            else
                            {
                                cr = currentSuite.TestCaseRuns.First();
                            }

                            if (currentSuiteError.Contains("#Testfall fehlerhaft:"))
                                currentSuiteError = currentSuiteError.Replace("#Testfall fehlerhaft:", "").Trim();

                            TestError te = new TestError();
                            te.Message = currentSuiteError;
                            cr.TestErrors.Add(te);
                        }
                    }
                    else
                    {
                        currentSuiteError = currentSuiteError + line.Trim() + " ";
                    }
                }
                // AppRun
                else if (line.StartsWith("Anwendung: "))
                {
                    string appArea = line.Replace("Anwendung: ", "");
                    if (appArea.ToLower().Contains("unittest.exe"))
                        appArea = appArea.Remove(appArea.ToLower().IndexOf("unittest.exe"));
                    dummyRun.AppArea = appArea;
                }

                else if (line.StartsWith("Builddatum: "))
                    dummyRun.BuildDate = Convert.ToDateTime(line.Replace("Builddatum: ", ""));

                else if (line.StartsWith("Server: "))
                    dummyRun.ServerName = line.Replace("Server: ", "").ToLower();

                else if (line.StartsWith("Version: "))
                    dummyRun.Version = line.Replace("Version: ", "").ToLower();
                    
                else if (line.StartsWith("Start: "))
                    dummyRun.StartTime = Convert.ToDateTime(line.Replace("Start: ", ""));

                else if (line.StartsWith("Ende: "))
                    dummyRun.EndTime = Convert.ToDateTime(line.Replace("Ende: ", ""));
            }

            // apply rundata to all runs in this log file
            foreach (AppRun r in allRuns)
            {
                r.AppArea = dummyRun.AppArea;
                r.BuildDate = dummyRun.BuildDate;
                r.ServerName = dummyRun.ServerName;
                r.Version = dummyRun.Version;
                r.StartTime = dummyRun.StartTime;
                r.EndTime = dummyRun.EndTime;
            }

            return allRuns;
        }

        /// <summary>
        /// converts the given string to a duration as double
        /// </summary>
        /// <param name="duration">e.g [3,14 Sekunden], [2,68 Minuten]</param>
        /// <returns></returns>
        private double ConvertDurationString(string duration)
        {
            string[] parts = duration.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            double result = 0;
            switch (parts[1].ToLower())
            {
                case "sekunden":
                    result = Convert.ToDouble(parts[0]);
                    break;
                case "minuten":
                    result = Convert.ToDouble(parts[0]) * 60;
                    break;
                case "stunden":
                    result = Convert.ToDouble(parts[0]) * 60 * 60;
                    break;
            }
            return Math.Round(result, 2);
        }
    }
}