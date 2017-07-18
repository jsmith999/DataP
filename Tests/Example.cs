using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DataPath.Tests {
    class Example {
        public static void Run() {
            var emp1 = new DalEmployee {
                City = "Montreal",
                Country = "CA",
                DOB = new DateTime(2000, 1, 1),
                HireDate = new DateTime(2015, 1, 1),
                Id = 1,
                Name = "Alpha",
                PostalCode = "H0H0H1",
                Province = "QC",
                Salary = 12345,
                Street1 = "Westmore",
                Street2 = "Av.",
                StreetNo = "5365",
            };
            var emp2 = new DalEmployee {
                City = "Montreal",
                Country = "CA",
                DOB = new DateTime(1990, 1, 1),
                HireDate = new DateTime(2015, 1, 1),
                Id = 2,
                Name = "Bravo",
                PostalCode = "H0H0H2",
                Province = "QC",
                Salary = 23456,
                Street1 = "Westmore",
                Street2 = "Av.",
                StreetNo = "5367",
            };
            var client1 = new DalClient {
                City = "Montreal",
                Contact = emp1,
                Country = "CA",
                DOB = new DateTime(2000, 1, 1),
                Id = 1,
                Name = "client one",
                PostalCode = "H0H1H1",
                Province = "QC",
                Street1 = "Westmore",
                Street2 = "Av.",
                StreetNo = "5365",
            };
            emp1.StreetNo = "x";
            Debug.Assert(client1.Contact.StreetNo == emp1.StreetNo);
        }
    }
}
