﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace learn.it.Models;

[PrimaryKey("UserId", "FlashcardId")]
[Table("flashcard_user_progress", Schema = "learnitdb")]
[Index("FlashcardId", Name = "fk_flashcard_user_progress_flashcards1_idx")]
public partial class FlashcardUserProgress
{
    [Key]
    [Column("user_id")]
    public int UserId { get; private set; }

    [Key]
    [Column("flashcard_id")]
    public int FlashcardId { get; private set; }

    [Column("consecutive_correct_answers")]
    public int ConsecutiveCorrectAnswers { get; set; }

    [Column("is_mastered")]
    public short IsMastered { get; set; }

    [Column("mastered_timestamp")]
    [Precision(0)]
    public DateTime? MasteredTimestamp { get; set; }

    [Column("needs_more_repetitions")]
    public short NeedsMoreRepetitions { get; set; }

    [ForeignKey("FlashcardId")]
    [InverseProperty("FlashcardUserProgress")]
    public virtual Flashcard Flashcard { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("FlashcardUserProgress")]
    public virtual User User { get; set; }
}