import { Component, OnInit } from '@angular/core';
import { AccountService } from '../services/account.service';
import { first } from 'rxjs';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit{

  form!: FormGroup;
  errorMessage: string | null = null;
  constructor(
    private accountService: AccountService,
    private formBuilder: FormBuilder,
    private router: Router,
  ) { }

  ngOnInit() {
    this.form = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }
  get f() { return this.form.controls; }

  onSubmit() {
    if (this.form.invalid) 
      return;    
    this.accountService.login(this.f.username.value, this.f.password.value)
      .pipe(first())
      .subscribe({
        next: () => {
          this.router.navigateByUrl('/');
        },
        error: error => {
          if (error.status === 400) 
            this.errorMessage = 'Niepoprawna nazwa użytkownika lub hasło.';
          else 
            this.errorMessage = 'Wystąpił błąd podczas logowania.';
        }
      });
  }

}
