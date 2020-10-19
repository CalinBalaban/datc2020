using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Models;


namespace L04
{
    class Program
	    {
	        private static CloudTable studentsTable;
	        private static CloudTableClient tableClient;
	        private static TableOperation tableOperation;
	        private static TableResult tableResult;
	        private static List<StudentEntity> studenti  = new List<StudentEntity>();
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
	
	            studentsTable = tableClient.GetTableReference("Studenti");
	
	            await studentsTable.CreateIfNotExistsAsync();
	            
	            int option = -1;
	            do
	            {
	                System.Console.WriteLine("1.Adaugare");
	                System.Console.WriteLine("2.Stergere");
	                System.Console.WriteLine("3.Afisare");
	                System.Console.WriteLine("4.Iesire");
	                System.Console.WriteLine("Alegeti optiunea: ");
	                string opt = System.Console.ReadLine();
	                option =Int32.Parse(opt);
	                switch(option)
	                {
	                    case 1:
	                        await Adaugare();
	                        break;
	                    case 2:
	                        await Stergere();
	                        break;
	                    case 3:
	                        await Afisare();
	                        break;
	                    case 4:
	                        break;
	                }
	            }while(option != 4);
	            
	        }
	        private static async Task<StudentEntity> RetrieveRecordAsync(CloudTable table,string partitionKey,string rowKey)
	        {
	            tableOperation = TableOperation.Retrieve<StudentEntity>(partitionKey, rowKey);
	            tableResult = await table.ExecuteAsync(tableOperation);
	            return tableResult.Result as StudentEntity;
	        }
	        private static async Task Adaugare()
	        {
	            System.Console.WriteLine("Universitatea:");
	            string universitate = Console.ReadLine();
	            System.Console.WriteLine("Cnp:");
	            string cnp = Console.ReadLine();
	            System.Console.WriteLine("Nume:");
	            string nume = Console.ReadLine();
	            System.Console.WriteLine("Prenume:");
	            string prenume = Console.ReadLine();
	            System.Console.WriteLine("Facultatea:");
	            string facultate= Console.ReadLine();
	            System.Console.WriteLine("Anul:");
	            string an = Console.ReadLine();
	
	            StudentEntity student = await RetrieveRecordAsync(studentsTable, universitate, cnp);
	            if(student == null)
	            {
	                var s = new StudentEntity(universitate,cnp);
	                s.Nume = nume;
	                s.Prenume = prenume;
	                s.Facultate = facultate;
	                s.An = Convert.ToInt32(an);
	                var insertOperation = TableOperation.Insert(s);
	                await studentsTable.ExecuteAsync(insertOperation);
	                System.Console.WriteLine("Studentul a fost introdus");
	            }
	            else
	            {
	                System.Console.WriteLine("Studentul exista deja");
	            }
	        }
	        
	        private static async Task Stergere()
	        {
	            System.Console.WriteLine("Universitatea:");
	            string universitate = Console.ReadLine();
	            System.Console.WriteLine("CNP:");
	            string cnp = Console.ReadLine();
	
	            StudentEntity student = await RetrieveRecordAsync(studentsTable, universitate, cnp);
	            if(student != null)
	            {
	                var s = new StudentEntity(universitate,cnp);
	                s.ETag = "*";
	                var deleteOperation = TableOperation.Delete(s);
	                await studentsTable.ExecuteAsync(deleteOperation);
	                System.Console.WriteLine("Studentul a fost sters!");
	            }
	            else
	            {
	                System.Console.WriteLine("Studentul nu exista!");
	            }
	        }
	        private static async Task<List<StudentEntity>> GetAllStudents()
	        {
	            TableQuery<StudentEntity> tableQuery = new TableQuery<StudentEntity>();
	            TableContinuationToken token = null;
	            do
	            {
	                TableQuerySegment<StudentEntity> result = await studentsTable.ExecuteQuerySegmentedAsync(tableQuery,token);
	                token = result.ContinuationToken;
	                studenti.AddRange(result.Results);
	            }while(token != null);
	            return studenti;
	        }
	        private static async Task Afisare()
	        {
	            await GetAllStudents();
	
	            foreach(StudentEntity s in studenti)
	            {
					Console.WriteLine("Nume : {0}", s.Nume);
					Console.WriteLine("Prenume : {0}", s.Prenume);
	                Console.WriteLine("Facultatea : {0}", s.Facultate);
	                Console.WriteLine("An : {0}", s.An);
	                Console.WriteLine("\n");
	            }
	            studenti.Clear();
	            
	        }
	    }
	}

