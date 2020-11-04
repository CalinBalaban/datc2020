using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Models;
using Newtonsoft.Json;

namespace L06
{
    class Program
	    {
	        private static CloudTable studentsTable;
	        private static CloudTableClient tableClient;
	        private static TableOperation tableOperation;
	        private static TableResult tableResult;
	        private static List<Studententity> students  = new List<Studententity>();
	        static void Main(string[] args)
	        {
	            Task.Run(async () => { await Initialize(); })
	                .GetAwaiter()
	                .GetResult();
	        }
	        static async Task Initialize()
	        {
	            string storageConnectionString = "DefaultEndpointsProtocol=https;"
	            + "AccountName=datc2020calin;"
	            + "AccountKey=YE+mCp/GKqlbCgMWIiKYJrUxbhbU7UamgwC2lqCJKxHx6lRYPv5pIV05bWpYs4os5vYAv4T9SrhzVyGCJ86y7g==;"
	            + "EndpointSuffix=core.windows.net";
	
	            var account = CloudStorageAccount.Parse(storageConnectionString);
	            tableClient = account.CreateCloudTableClient();
	
	            studentsTable = tableClient.GetTableReference("Students");
	
	            await studentsTable.CreateIfNotExistsAsync();
	            
	            int option = -1;
	            do
	            {
	                System.Console.WriteLine("1.Adaugare student.");
	                System.Console.WriteLine("2.Stergere student.");
	                System.Console.WriteLine("3.Afisare studenti.");
	                System.Console.WriteLine("4.Exit");
	                System.Console.WriteLine("Alegeti optiunea: ");
	                string opt = System.Console.ReadLine();
	                option =Int32.Parse(opt);
	                switch(option)
	                {
	                    case 1:
	                        await AdaugareStudent();
	                        break;
	                    case 2:
	                        await StergereStudent();
	                        break;
	                    case 3:
	                        await AfisareStudenti();
	                        break;
	                    case 4:
	                        System.Console.WriteLine("O zi buna!");
	                        break;
	                }
	            }while(option != 4);
	            
	        }
	        private static async Task<Studententity> RetrieveRecordAsync(CloudTable table,string partitionKey,string rowKey)
	        {
	            tableOperation = TableOperation.Retrieve<Studententity>(partitionKey, rowKey);
	            tableResult = await table.ExecuteAsync(tableOperation);
	            return tableResult.Result as Studententity;
	        }
	        private static async Task AdaugareStudent()
	        {
	            System.Console.WriteLine("Introduceti Universitatea:");
	            string university = Console.ReadLine();
	            System.Console.WriteLine("Introduceti CNP:");
	            string cnp = Console.ReadLine();
	            System.Console.WriteLine("Intruduceti nume:");
	            string nume = Console.ReadLine();
	            System.Console.WriteLine("Introduceti prenume:");
	            string prenume = Console.ReadLine();
	            System.Console.WriteLine("Introduceti facultatea:");
	            string facultate= Console.ReadLine();
	            System.Console.WriteLine("Introduceti anul de studiu:");
	            string an = Console.ReadLine();
	
	            Studententity stud = await RetrieveRecordAsync(studentsTable, university, cnp);
	            if(stud == null)
	            {
	                var student = new Studententity(university,cnp);
	                student.Nume = nume;
	                student.Prenume = prenume;
	                student.Facultate = facultate;
	                student.An = Convert.ToInt32(an);
	                //var insertOperation = TableOperation.Insert(student);
	                //await studentsTable.ExecuteAsync(insertOperation);
					
					var jsonStundet=JsonConvert.SerializeObject(student);
					var plainTextBytes=System.Text.Encoding.UTF8.GetBytes(jsonStundet);
					var base64String=System.Convert.ToBase64String(plainTextBytes);
					QueueClient queueClient= new QueueClient("DefaultEndpointsProtocol=https;AccountName=datc2020calin;AccountKey=YE+mCp/GKqlbCgMWIiKYJrUxbhbU7UamgwC2lqCJKxHx6lRYPv5pIV05bWpYs4os5vYAv4T9SrhzVyGCJ86y7g==;EndpointSuffix=core.windows.net","stundents-queue");
					await queueClient.SendMessageAsync(base64String);
					
	                System.Console.WriteLine("Studentul a fost introdus!");
	            }
	            else
	            {
	                System.Console.WriteLine("Exista deja acest student!");
	            }
	        }
	        
	        private static async Task StergereStudent()
	        {
	            System.Console.WriteLine("Introduceti Universitatea:");
	            string university = Console.ReadLine();
	            System.Console.WriteLine("Introduceti CNP:");
	            string cnp = Console.ReadLine();
	
	            Studententity stud = await RetrieveRecordAsync(studentsTable, university, cnp);
	            if(stud != null)
	            {
	                var student = new Studententity(university,cnp);
	                student.ETag = "*";
	                var deleteOperation = TableOperation.Delete(student);
	                await studentsTable.ExecuteAsync(deleteOperation);
	                System.Console.WriteLine("Studentul a fost sters!");
	            }
	            else
	            {
	                System.Console.WriteLine("Studentul nu exista!");
	            }
	        }
	        private static async Task<List<Studententity>> GetAllStudents()
	        {
	            TableQuery<Studententity> tableQuery = new TableQuery<Studententity>();
	            TableContinuationToken token = null;
	            do
	            {
	                TableQuerySegment<Studententity> result = await studentsTable.ExecuteQuerySegmentedAsync(tableQuery,token);
	                token = result.ContinuationToken;
	                students.AddRange(result.Results);
	            }while(token != null);
	            return students;
	        }
	        private static async Task AfisareStudenti()
	        {
	            await GetAllStudents();
	
	            foreach(Studententity std in students)
	            {
					Console.WriteLine("Nume : {0}", std.Nume);
					Console.WriteLine("Prenume : {0}", std.Prenume);
	                Console.WriteLine("Facultatea : {0}", std.Facultate);
	                Console.WriteLine("An : {0}", std.An);
	                Console.WriteLine("\n");
	            }
	            students.Clear();
	            
	        }
	    }
	}

