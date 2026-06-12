using System;
using System.Collections.Generic;
using System.Text;

namespace SportivaModels.Models
{
    internal class Tourment
    {
        public int id {  get; set; }   
        public int organizerId { get; set; }
        public string name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string status { get; set; }
        public DateTime createdAt { get; set; }
    }
}
