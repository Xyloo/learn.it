import { Flashcard } from "../flashcard";

export interface StudySet {
  id: number;
  name: string;
  description: string;
  visibility: number;
  creator: { username: string; avatar: string | null };
  flashcards: Flashcard[];
  flashcardsCount: number;
  group?: number; 
}
