﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace learn.it.Models;

[Table("user_stats", Schema = "learnitdb")]
[Index("UserId", Name = "fk_user_stats_users1_idx")]
public partial class UserStats
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("user_id")]
    public int UserId { get; private set; }

    [Column("total_sets_mastered")]
    public int TotalSetsMastered { get; set; }

    [Column("total_login_days")]
    public int TotalLoginDays { get; set; }

    [Column("total_flashcards_mastered")]
    public int TotalFlashcardsMastered { get; set; }

    [Column("consecutive_login_days")]
    public int ConsecutiveLoginDays { get; set; }

    [Column("total_sets_added")]
    public int TotalSetsAdded { get; set; }

    [Column("total_flashcards_added")]
    public int TotalFlashcardsAdded { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserStats")]
    [JsonIgnore]
    public virtual User User { get; set; }
}