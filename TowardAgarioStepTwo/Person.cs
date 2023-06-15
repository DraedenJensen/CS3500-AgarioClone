using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowardAgarioStepTwo
{
    internal class Person
    {
        public int ID { get; private set; }
        public float GPA { get; private set; }
        public string Name { get; private set; }

        public Person(string name, float gpa)
        {
            ID = 1;
            GPA = gpa;
            Name = name;
        }
    }
}
