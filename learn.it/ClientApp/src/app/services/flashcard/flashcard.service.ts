import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin } from 'rxjs';
import { Flashcard } from '../../models/flashcard';
import { FlashcardAnswer } from '../../models/flashcards/answer.dto';

@Injectable({
  providedIn: 'root'
})
export class FlashcardService {

  constructor(private http: HttpClient) { }

  createFlashcards(flashcards: any[]): Observable<any[]> {
    const requests = flashcards.map(flashcard =>
      this.http.post(`${environment.apiUrl}/flashcards`, flashcard)
    );
    return forkJoin(requests);
  }

  deleteFlashcard(flashcardId: number): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/flashcards/${flashcardId}`);
  }

  updateFlashcard(flashcardId: number, flashcard: any): Observable<any> {
    return this.http.put(`${environment.apiUrl}/flashcards/${flashcardId}`, flashcard);
  }

  generateAnswer(answer: FlashcardAnswer): Observable<any> {
    return this.http.post(`${environment.apiUrl}/answers`, answer);
  }
}
