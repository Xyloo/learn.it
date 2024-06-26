﻿using System.Text.Json.Serialization;

namespace learn.it.Models.Dtos.Response
{
    public class BasicStudySetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public Visibility Visibility { get; set; }
        public AnonymousUserResponseDto Creator { get; set; }
        public int FlashcardsCount { get; set; }
        public BasicGroupDto? Group { get; set; }

        public BasicStudySetDto(StudySet studySet)
        {
            Id = studySet.StudySetId;
            Name = studySet.Name;
            Description = studySet.Description;
            Visibility = studySet.Visibility;
            Creator = new AnonymousUserResponseDto(studySet.Creator);
            Group = studySet.Group is null ? null : new BasicGroupDto(studySet.Group);
            FlashcardsCount = studySet.Flashcards.Count;
        }

        [JsonConstructor]
        public BasicStudySetDto()
        {
        }
    }
}
