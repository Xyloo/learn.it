﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

    [Column("sets_completed")]
    public int SetsCompleted { get; set; }

    [Column("total_login_days")]
    public int TotalLoginDays { get; set; }

    [Column("total_flashcards_mastered")]
    public int TotalFlashcardsMastered { get; set; }

    [Column("consecutive_login_days")]
    public int ConsecutiveLoginDays { get; set; }

    [Column("sets_added")]
    public int SetsAdded { get; set; }

    [Column("flashcards_added")]
    public int FlashcardsAdded { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserStats")]
    public virtual User User { get; set; }
}