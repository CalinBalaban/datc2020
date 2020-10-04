using System.Collections.Generic;
namespace ApiStudent
{
    public static class StudentRepo
    {
        public static List<Student> Student = new List<Student>()
        {
            new Student()
            {
                Id = 1,
                Nume = "Balaban",
                Prenume = "Calin",
                Facultate = "Automatica si Calculatoare",
                An = 4

            },
            
            new Student()
            {
                Id = 2,
                Nume = "Blajovan",
                Prenume = "Andrei",
                Facultate = "Constructii",
                An = 4
            }

        };
    }
}