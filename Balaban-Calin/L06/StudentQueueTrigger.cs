using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Models;
using Newtonsoft.Json;

namespace DATC.TEMA
{
    public static class StudentQueueTrigger
    {
        [FunctionName("StudentQueueTrigger")]
        public static Studententity Run([QueueTrigger("stundets-queue", Connection = "storageaccountdatc29220_STORAGE")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            var student=JsonConvert.DeserializeObject<Studententity>(myQueueItem);
            return student;
        }
    }
}
