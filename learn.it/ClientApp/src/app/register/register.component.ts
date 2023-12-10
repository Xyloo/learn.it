import { Component } from '@angular/core';
import { AccountService } from '../services/account.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { first } from 'rxjs';
import { mustMatch } from '../utils/validators';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  form!: FormGroup;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  submitted = false;
  passwordRegexErrorMessage = 'Hasło musi zawierać co najmniej 8 znaków, w tym jedną dużą literę, jedną małą literę i cyfrę.';

  constructor(
    private accountService: AccountService,
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
  ) { }

  ngOnInit() {
    this.form = this.formBuilder.group({
      username: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.pattern("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,}$")]],
      confirmPassword: ['', Validators.required]
    },
      {
        validators: mustMatch('password', 'confirmPassword')
      }
    );
  }

  get f() { return this.form.controls; }

  setCustomError(controlName: string, errorMessage: string) {
    const control = this.form.get(controlName);
    control?.setErrors({ customError: errorMessage })
  }

  onSubmit() {

    this.submitted = true;
    
    if (!this.isFormValid()) {
      return;
    }

    this.accountService.register(this.f.username.value, this.f.email.value, this.f.password.value)
      .pipe(first())
      .subscribe({
        next: () => {
          this.successMessage = 'Rejestracja zakończona sukcesem. Przekierowanie...';
          setTimeout(() => {
            this.router.navigateByUrl('/login');
          }, 1200);
        },
        error: error => {
          if (error.error && error.error.detail) {
            if (error.status === 409) {
              const detail = error.error.detail;
              if (typeof detail === 'string') {
                if (detail.toLowerCase().includes('email')) {
                  this.setCustomError('email', 'Podany adres email jest już zajęty.');
                } else if (detail.toLowerCase().includes('użytkownika')) {
                  this.setCustomError('username', 'Podana nazwa użytkownika jest już zajęta.');
                }
              }
            }
          } else {
            this.errorMessage = 'Wystąpił błąd podczas logowania.';
            setTimeout(() => this.errorMessage = null, 2000);
          }
        }
      });
  }

  isFormValid(): boolean {
    if (this.form.invalid) {
      Object.keys(this.form.controls).forEach(key => {
        const controlErrors = this.form.get(key)?.errors;
        if (controlErrors) {
          console.log(`Validation error in ${key}:`, controlErrors);
        }
      });
      return false;
    }
    return true;
  }


}
