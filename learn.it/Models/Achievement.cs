﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace learn.it.Models;

[Table("achievements", Schema = "learnitdb")]
public partial class Achievement
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("achievement_id")]
    public int AchievementId { get; private set; }

    [Required]
    [Column("name")]
    [StringLength(150)]
    public string Name { get; set; }

    [Required]
    [Column("image_url")]
    [StringLength(100)]
    public string ImageUrl { get; set; }

    [Required]
    [Column("description")]
    [StringLength(200)]
    public string Description { get; set; }

    [InverseProperty("Achievement")]
    [JsonIgnore]
    public virtual ICollection<UserAchievements> UserAchievements { get; set; } = new List<UserAchievements>();
}