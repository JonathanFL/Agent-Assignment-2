using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace AgentAssignment2
{
    public class Agent
    {
        public Agent()
        {
        }

        public Agent(string aId, string aName, string specialty, string aAssignment)
        {
            ID = aId;
            CodeName = aName;
            Specialty = specialty;
            Assignment = aAssignment;
        }

        public string ID { get; set; }

        public string CodeName { get; set; }

        public string Specialty { get; set; }

        public string Assignment { get; set; }
    }
}
