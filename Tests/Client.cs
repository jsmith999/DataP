using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataPath.Tests {
    class DalClient {
        public static Dictionary<int, DalClient> Data = new Dictionary<int, DalClient>();
        
        private int id = -1;
        [Key]
        public int Id {
            get { return id; }
            set { if (id <= 0) Data.Add(id = value, this); }
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
        
        // FK        
        private int contactKey;
        private int ContactKey { 
            get { return contactKey;}
            set { contact = DataPath.Tests.DalEmployee.Data.ContainsKey(value) ? DataPath.Tests.DalEmployee.Data[contactKey = value] : null; }
        }
        private DataPath.Tests.DalEmployee contact;
        public DataPath.Tests.DalEmployee Contact {
            get { return contact; }
            set { contactKey = (contact = value) == null ? 0 : value.Id; } 
        }
    }
}
