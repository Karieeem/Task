using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Task.ViewModels
{
    public class CustomerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CheckBoxViewModel> Products { get; set; }

    }
}