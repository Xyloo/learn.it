﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace learn.it.Models;

[Table("groups", Schema = "learnitdb")]
[Index("OwnerId", Name = "fk_groups_users1_idx")]
public partial class Groups
{
    [Key]
    [Column("group_id")]
    public int GroupId { get; set; }

    [Required(ErrorMessage = "Group's name cannot be blank.")]
    [Column("name")]
    [StringLength(150, ErrorMessage = "Group's name cannot be shorter than 5 and longer than 150 characters.", MinimumLength = 5)]
    public string Name { get; set; }

    [Column("owner_id")]
    public int OwnerId { get; set; }

    [ForeignKey("OwnerId")]
    [InverseProperty("Groups")]
    public virtual Users Owner { get; set; }

    [InverseProperty("Group")]
    public virtual ICollection<StudySets> StudySets { get; set; } = new List<StudySets>();

    [ForeignKey("GroupId")]
    [InverseProperty("Group")]
    public virtual ICollection<Users> User { get; set; } = new List<Users>();
}