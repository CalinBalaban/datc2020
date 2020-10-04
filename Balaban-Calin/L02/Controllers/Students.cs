using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
namespace ApiStudent.Controllers
{
   [ApiController]
   [Route("[controller]")]
   public class Students : ControllerBase
   {
       private readonly ILogger<Students> _logger;

        public Students(ILogger<Students> logger)
        {
            _logger = logger;
        }

       [HttpGet]
       public IEnumerable<Student> Get()
       {
           return StudentRepo.Student;
       }

       [HttpGet("{Id}")]
        public Student Get(int id)
       {
           foreach(Student s in StudentRepo.Student)
           {
               if(s.Id == id)
               {
                   return s;
               }
           }
           return null;
       }

       [HttpPost]
       public List<Student> Post([FromBody] Student s)
       {
           StudentRepo.Student.Add(s);
           return StudentRepo.Student;
       }

       [HttpDelete("{Id}")]
       public List<Student> Delete(int id)
       {
           foreach(Student s in StudentRepo.Student)
           {
               if(s.Id == id)
               {
                   StudentRepo.Student.Remove(s);

               }
            
           }
           return StudentRepo.Student;

       }

       [HttpPut("{Id}")]
       public List<Student> Put(int id, [FromBody] Student s)
       {
           foreach(Student st in StudentRepo.Student)
           {
               if(st.Id == id)
               {
                   st.Id = s.Id;
                   st.Nume = s.Nume;
                   st.Prenume = s.Prenume;
                   st.Facultate = s.Facultate;
                   st.An = s.An;
               }
           }
           return StudentRepo.Student;
       }

       
   }
}