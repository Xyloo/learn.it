import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable, catchError, map, throwError } from 'rxjs';
import { UserSetDto } from '../../models/user-sets.dto';
import { CreateStudySetDto } from '../../models/study-sets/create-study-set.dto';
import { StudySet } from '../../models/study-sets/study-set';
import { AccountService } from '../account.service';

@Injectable({
  providedIn: 'root'
})
export class StudySetsService {

  private userId: number;
  constructor(
    private http: HttpClient,
    private accountSerivce: AccountService
  ) { }
/*
const storedUser = JSON.parse(localStorage.getItem('userId')!);
this.userId = storedUser ? storedUser : null;*/


  getStudySets(): Observable<UserSetDto[]> {
    return this.http.get<any[]>(`${environment.apiUrl}/users/${this.accountSerivce.userValue?.userId}/studysets`).pipe(
      map(response => response.map(item => ({
        id: item.id,
        setName: item.name,
        description: item.description,
        username: item.creator.username
      })))
    );
  }

  deleteStudySet(studySetId: number): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/studysets/${studySetId}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  createStudySet(studySet: CreateStudySetDto): Observable<any> {
    console.log("StudySet: ", studySet);
    return this.http.post(`${environment.apiUrl}/studysets`, studySet);
  }

  getStudySet(setId: number): Observable<StudySet> {
    return this.http.get<StudySet>(`${environment.apiUrl}/studysets/${setId}`);
  }

  updateStudySet(setId: number, studySet: any): Observable<any> {
    return this.http.put(`${environment.apiUrl}/studysets/${setId}`, studySet);
  }
  findStudySets(query: string): Observable<StudySet[]> {
    return this.http.get<StudySet[]>(`${environment.apiUrl}/studysets/find/${query}`);
  }

  getUserRecommendedSets(): Observable<StudySet[]> {
    return this.http.get<StudySet[]>(`${environment.apiUrl}/studysets`).pipe(
      map(response => response.map(item => ({
        ...item, 
        creator: {
          ...item.creator,
          avatar: item.creator.avatar ? `/Avatars/${item.creator.avatar}` : '/assets/temp-avatar.png'
        }
      })))
    );
  }


  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'Unknown error occurred';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.error.message}`;
    } else {
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    return throwError(errorMessage);
  }


}
