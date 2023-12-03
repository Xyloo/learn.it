import { Component, ElementRef, HostListener, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {

  @ViewChild('searchInput') searchInput: ElementRef;

  constructor(private router: Router, private accountService: AccountService) { }

  showDropdown: boolean = false;
  toggleDropdown() {
    this.showDropdown = !this.showDropdown;
  }

  closeDropdown() {
    this.showDropdown = false;
  }

  focusOnSearchInput(): void {
    if (this.searchInput) {
      this.searchInput.nativeElement.focus();
      this.closeDropdown();
    }
  }
  redirectTo(url: string) {
    this.router.navigateByUrl(url);
    this.closeDropdown();
  }

  logout() {
    this.accountService.logout();
  }


  @HostListener('document:click', ['$event'])
  clickOutside(event: { target: { closest: (arg0: string) => any; }; }) {
    if (!event.target.closest('.dropdown')) {
      this.closeDropdown();
    }
  }
}
