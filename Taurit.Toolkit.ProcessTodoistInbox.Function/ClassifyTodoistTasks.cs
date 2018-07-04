using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Taurit.Toolkit.ProcessTodoistInbox.Function
{
    public static class ClassifyTodoistTasks
    {
        [FunctionName("ClassifyTodoistTasks")]
        public static void Run([TimerTrigger("0 0 */2 * * *", RunOnStartup = true)]
            TimerInfo myTimer, TraceWriter log, ExecutionContext executionContext)
        {
            try
            {
                Debug.Assert(log != null);

                log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
                log.Info($"yo world");


                String configFileContents = File.ReadAllText(executionContext.FunctionAppDirectory + "\\configuration.private.json");
                log.Info(configFileContents);
            }
            catch (Exception e)
            {
                log.Error("Unhandled top-level exception", e);
                throw e;
            }
        }
    }
}