﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSUIR.ManagerQueue.Data.Model
{
    [Table("Position")]
    public class Position : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string JobTitle { get; set; }

        public int? Priority { get; set; }

        public virtual IEnumerable<Employee> Employees { get; set; }
    }
}
