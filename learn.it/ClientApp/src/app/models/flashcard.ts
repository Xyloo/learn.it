export interface Flashcard {
  flashcardId: number; 
  term: string;
  definition: string;
  isTermText: boolean;
  studySetId?: number;
}
