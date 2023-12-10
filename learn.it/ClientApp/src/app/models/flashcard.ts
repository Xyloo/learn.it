export interface Flashcard {
  id: number; 
  term: string;
  definition: string;
  isTermText: boolean;
  studySetId?: number; 
}
