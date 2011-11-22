using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfWebApp.Models
{
    public class TaskList
    {
        public int Id { get; set; }
        public User User { get; set; }
        public List<Item> Items { get; set; }
    }
}