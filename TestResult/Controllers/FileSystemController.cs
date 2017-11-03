using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            // dont save apprun in db if already exists
            var runAlreadyInDb = from st in context.AppRuns where st.StartTime == appRun.StartTime select st;

            // if test wasnt completed, no apparea is provided. Dont save this to db
            if (runAlreadyInDb.Count() > 0 || String.IsNullOrWhiteSpace(appRun.AppArea))
                return;

            context.AppRuns.Add(appRun);
            context.SaveChanges();
        }

        /// <summary>
        /// checks directories given in settings
        /// </summary>
        public void checkDirectories()
        {
            String[] paths = new string[] { @"\\vmwrewetcdev\d$\CGM\CGM_REWE\Cobra\Test" };
            foreach (string path in paths)
            {
                string cleanPath = path.EndsWith(@"\") ? path.Remove(path.Length - 1) : path;
                // ignore if directory is not found
                if (!Directory.Exists(cleanPath))
                    continue;

                foreach (string filename in Directory.GetFiles(cleanPath, "*.log", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        AppRun r = ProcessFile(filename);
                        SaveToDatabase(r);

                        // move successfully processed file to backup dir
                        string backupDir = cleanPath + @"\Backup\";
                        if (!Directory.Exists(backupDir))
                            Directory.CreateDirectory(backupDir);
                        File.Move(filename, backupDir + Path.GetFileName(filename));

                        // write log
                        writeLog(cleanPath, "successfully processed file [" + filename + "]", true);
                    }
                    catch (Exception ex)
                    {
                        // log exception
                        writeLog(cleanPath, "an error occured while processing file [" + filename + "]:", true);
                        writeLog(cleanPath, ex.Message + nl + ex.StackTrace, false);

                        // log inner exception if available
                        if (ex.InnerException != null)
                            writeLog(cleanPath, "inner exception: " + nl + ex.InnerException.Message + nl + ex.InnerException.StackTrace, false);
                    }
                }
            }
        }

        /// <summary>
        /// writes message to logfile in given directory
        /// </summary>
        /// <param name="cleanPath"></param>
        /// <param name="message"></param>
        /// <param name="writeDateTime"></param>
        private void writeLog(string cleanPath, string message, bool writeDateTime)
        {
            string content = writeDateTime ? DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " " + message : message;
            using (StreamWriter file = new StreamWriter(cleanPath + @"\ServiceLog.txt", true))
            {
                file.WriteLine(content);
            }
        }

        /// <summary>
        /// processes the given unittest logfile
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private AppRun ProcessFile(string filename)
        {
            AppRun currentRun = new AppRun();
            TestSuiteRun currentSuite = new TestSuiteRun();
            TestCaseRun currentCase = new TestCaseRun();
            string currentErrorLine = "";

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
                        currentErrorLine = "";
                    }
                    // end of testcase
                    else if (line.Contains(" beendet [Dauer: "))
                    {
                        int strStart = line.IndexOf("[Dauer: ") + 8;
                        string duration = line.Substring(strStart, line.IndexOf("]") - strStart).Trim();
                        currentCase.Duration = ConvertDurationString(duration);
                        currentErrorLine = currentErrorLine.Trim();
                        currentErrorLine = currentErrorLine.Replace("#Testfall fehlgeschlagen: ", "");
                        currentErrorLine = currentErrorLine.Replace("#Testfall fehlerhaft: ", "");

                        if (!String.IsNullOrWhiteSpace(currentErrorLine))
                        {
                            TestError err = new TestError();
                            err.Message = currentErrorLine;
                            currentCase.TestErrors.Add(err);
                        }
                        currentErrorLine = "";
                    }
                    // testerror
                    else
                    {
                        currentErrorLine += line.Trim() + " ";
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
                        currentSuite.DbType = line.Substring(line.IndexOf("[") + 1, line.IndexOf("]") - line.IndexOf("[") - 1).Trim();
                        currentSuite.Alias = line.Substring(strStart + 22, line.IndexOf("[") - strStart - 24).Trim();
                        currentRun.TestSuiteRuns.Add(currentSuite);
                    }
                    else if (line.Contains(" beendet [Dauer: "))
                    {
                        int strStart = line.IndexOf("[Dauer: ") + 8;
                        //TC_BsGui_BusinessObjectMassInsertHelper_01 beendet [Dauer: 1,48 Sekunden]
                        string duration = line.Substring(strStart, line.IndexOf("]") - strStart).Trim();
                        currentSuite.Duration = ConvertDurationString(duration);
                    }
                }
                // AppRun
                else if (line.StartsWith("Anwendung: "))
                {
                    string appArea = line.Replace("Anwendung: ", "");
                    if (appArea.ToLower().Contains("unittest.exe"))
                        appArea = appArea.Remove(appArea.ToLower().IndexOf("unittest.exe"));
                    currentRun.AppArea = appArea;
                }

                else if (line.StartsWith("Builddatum: "))
                    currentRun.BuildDate = Convert.ToDateTime(line.Replace("Builddatum: ", ""));

                else if (line.StartsWith("Server: "))
                    currentRun.ServerName = line.Replace("Server: ", "").ToLower();

                else if (line.StartsWith("Start: "))
                    currentRun.StartTime = Convert.ToDateTime(line.Replace("Start: ", ""));

                else if (line.StartsWith("Ende: "))
                    currentRun.EndTime = Convert.ToDateTime(line.Replace("Ende: ", ""));
            }

            return currentRun;
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