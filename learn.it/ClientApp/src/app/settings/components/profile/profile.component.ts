import { HttpClient } from '@angular/common/http';
import { Component, ViewChild } from '@angular/core';
import { UserService } from '../../../services/user/user.service';
import { SnackbarService } from '../../../services/snackbar.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent {
  @ViewChild('avatarInput') avatarInput: any;
  userAvatarUrl: string = '/assets/temp-avatar.png';
  selectedAvatarFile: File | null = null;

  constructor(
    private userService: UserService,
    private snackBarService: SnackbarService
  ) { }

  onAvatarSelected(event: Event) {
    const inputElement = event.target as HTMLInputElement;
    const file = inputElement.files ? inputElement.files[0] : null;

    if (file) {
      this.selectedAvatarFile = file;
      this.userAvatarUrl = URL.createObjectURL(file);
    } else {
      this.selectedAvatarFile = null;
      this.userAvatarUrl = '/assets/temp-avatar.png';
    }
  }

  removeAvatar() {
    this.userService.removeAvatar().subscribe({
      next: () => this.snackBarService.openSnackBar("Pomyślnie usunięto awatar."),
      error: () => this.snackBarService.openSnackBar("Wystąpił błąd podczas usuwania awatara.")
    });
    this.selectedAvatarFile = null;
    this.userAvatarUrl = '/assets/temp-avatar.png';
  }

  saveChanges() {
    if (this.selectedAvatarFile) {
      const formData = new FormData();
      formData.append('avatar', this.selectedAvatarFile);
      this.userService.uploadAvatar(formData).subscribe({
        next: () => this.snackBarService.openSnackBar("Pomyślnie dodano awatar."),
        error: () => this.snackBarService.openSnackBar("Wystąpił błąd podczas dodawania awatara.")
      });
    }
  }
}
