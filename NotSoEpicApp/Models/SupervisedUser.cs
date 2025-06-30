using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotSoEpicApp
{
    public class SupervisedUser
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public bool AllowViewCharts { get; set; }
        public bool AllowViewTransactions { get; set; }
        public bool AllowViewSupervisors { get; set; }
        public bool AllowAddTransactions { get; set; }
        public bool AllowAddSupervisors { get; set; }
        public bool AllowControlSupervised { get; set; }
    }

}
