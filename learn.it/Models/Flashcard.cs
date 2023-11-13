﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace learn.it.Models;

[Table("flashcards", Schema = "learnitdb")]
[Index("StudySetId", Name = "fk_flashcards_study_sets1_idx")]
public partial class Flashcard
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("flashcard_id")]
    public int FlashcardId { get; private set; }

    [Required(ErrorMessage = "Flashcard's term cannot be blank.")]
    [Column("term")]
    [StringLength(500, ErrorMessage = "Flashcard's term cannot be longer than 500 characters.")]
    public string Term { get; set; }

    [Required(ErrorMessage = "Flashcard's definition cannot be blank.")]
    [Column("definition")]
    [StringLength(500, ErrorMessage = "Flashcard's defintion cannot be longer than 500 characters.")]
    public string Definition { get; set; }

    [Column("is_term_text")]
    public short IsTermText { get; set; }

    [InverseProperty("Flashcard")]
    [JsonIgnore]
    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    [InverseProperty("Flashcard")]
    [JsonIgnore]
    public virtual ICollection<FlashcardUserProgress> FlashcardUserProgress { get; set; } = new List<FlashcardUserProgress>();

    [ForeignKey("StudySetId")]
    [InverseProperty("Flashcards")]
    public virtual StudySet StudySet { get; set; }
}