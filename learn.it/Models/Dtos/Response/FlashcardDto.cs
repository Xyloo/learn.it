﻿using System.Text.Json.Serialization;

namespace learn.it.Models.Dtos.Response
{
    public class FlashcardDto
    {
        public int FlashcardId { get; set; }

        public string Term { get; set; }

        public string Definition { get; set; }
        public bool IsTermText { get; set; }

        public FlashcardDto(Flashcard flashcard)
        {
            FlashcardId = flashcard.FlashcardId;
            Term = flashcard.Term;
            Definition = flashcard.Definition;
            IsTermText = flashcard.IsTermText;
        }

        [JsonConstructor]
        public FlashcardDto()
        { }
    }
}
