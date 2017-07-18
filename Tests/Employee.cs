using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataPath.Tests {
    class DalEmployee {
        public static Dictionary<int, DalEmployee> Data = new Dictionary<int, DalEmployee>();
        
        private int id = -1;
        [Key]
        public int Id {
            get { return id; }
            set { if (id < 0) Data.Add(id = value, this); }
        }

        public string Name { get; set; }
        public string StreetNo { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public DateTime DOB { get; set; }
        public float Salary { get; set; }
        public DateTime HireDate { get; set; }
    }
}
