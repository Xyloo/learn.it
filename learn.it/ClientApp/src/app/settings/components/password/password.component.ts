import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { mustMatch } from '../../../utils/validators';
import { AccountService } from '../../../services/account.service';
import { UserService } from '../../../services/user/user.service';
import { SnackbarService } from '../../../services/snackbar.service';
@Component({
  selector: 'app-password',
  templateUrl: './password.component.html',
  styleUrls: ['./password.component.css']
})
export class PasswordComponent {
  changePasswordForm: FormGroup;

  constructor(
    private formBuilder: FormBuilder,
    private userService: UserService,
    private snackBarService: SnackbarService

  ) { }

  ngOnInit(): void {
    this.changePasswordForm = this.formBuilder.group({
      currentPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.pattern("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,}$")]],
      confirmPassword: ['', Validators.required]
    }, {
      validators: mustMatch('newPassword', 'confirmPassword')
    });
  }

  get f() { return this.changePasswordForm.controls; }


  onSubmit() {
    if (this.changePasswordForm.valid) {
      this.validateAndChangePassword();
    }
  }

  validateAndChangePassword() {
    this.userService.validatePasswordChange(this.f.newPassword.value).subscribe(
      () => this.changePassword(),
      error => this.displayErrorMessage(error.error.detail)
    );
  }

  changePassword() {
    this.userService.changeUserPassword(this.f.newPassword.value).subscribe(
      () => this.handlePasswordChangeSuccess(),
      error => this.handlePasswordChangeError()
    );
  }

  handlePasswordChangeSuccess() {
    this.snackBarService.openSnackBar('Pomyślnie zmieniono hasło.', "Zamknij", 3000);
    this.changePasswordForm.reset();
  }

  handlePasswordChangeError() {
    this.snackBarService.openSnackBar('Wystąpił błąd podczas zmiany hasła.', "Zamknij", 3000);
  }

  displayErrorMessage(message: string) {
    this.snackBarService.openSnackBar(message, "Zamknij", 3000);
  }
}
