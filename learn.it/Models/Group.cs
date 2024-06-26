﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using learn.it.Models.Dtos.Response;
using Microsoft.EntityFrameworkCore;

namespace learn.it.Models;

[Table("Groups", Schema = "learnitdb")]
[Index("CreatorId", Name = "fk_groups_users1_idx")]
public partial class Group
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int GroupId { get; set; }

    public string Name { get; set; }

    [ForeignKey("CreatorId")]
    [InverseProperty("GroupCreator")]
    public virtual User Creator { get; set; }

    [InverseProperty("Group")]
    [JsonIgnore]
    public virtual ICollection<StudySet> StudySets { get; set; } = new List<StudySet>();

    [ForeignKey("GroupId")]
    [InverseProperty("Groups")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();

    [InverseProperty("Group")]
    [JsonIgnore]
    public virtual ICollection<GroupJoinRequest> GroupJoinRequests { get; set; } = new List<GroupJoinRequest>();

    public BasicGroupDto ToBasicGroupDto()
    {
        return new BasicGroupDto(this);
    }

    public GroupDto ToGroupDto()
    {
        return new GroupDto(this);
    }
}