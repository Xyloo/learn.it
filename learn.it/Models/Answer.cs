﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace learn.it.Models;

[Table("Answers", Schema = "learnitdb")]
[Index("FlashcardId", Name = "fk_answers_flashcards1_idx")]
[Index("UserId", Name = "fk_answers_users1_idx")]
public partial class Answer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AnswerId { get; private set; }

    public bool IsCorrect { get; set; }

    public int AnswerTime { get; set; }

    public DateTime AnswerTimestamp { get; set; }

    [ForeignKey("FlashcardId")]
    [InverseProperty("Answers")]
    public virtual Flashcard Flashcard { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Answers")]
    public virtual User User { get; set; }
}