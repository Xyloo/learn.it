﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace learn.it.Models;

[Table("Permissions", Schema = "learnitdb")]
public partial class Permission
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PermissionId { get; set; }

    [Required]
    [StringLength(45)]
    public string Name { get; set; }

    [InverseProperty("Permissions")]
    [JsonIgnore]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}