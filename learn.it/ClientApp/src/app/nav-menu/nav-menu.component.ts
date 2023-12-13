import { Component, ElementRef, HostListener, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { UserService } from '../services/user/user.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit, OnDestroy {

  @ViewChild('searchInput') searchInput: ElementRef;
  searchQuery: string = '';
  public username: string = '';
  public userAvatarName: string = '/assets/temp-avatar.png'
  private userSubscription: Subscription;

  constructor(
    private router: Router,
    private accountService: AccountService
  ) { }

  ngOnDestroy(): void {
    this.userSubscription.unsubscribe();
  }


  ngOnInit(): void {
    this.userSubscription = this.accountService.user.subscribe(user => {
      this.username = user?.uniqueName || '';
      this.userAvatarName = user?.avatarName || '/assets/temp-avatar.png';
    });

  }

  showDropdown: boolean = false;
  toggleDropdown() {
    this.showDropdown = !this.showDropdown;
  }

  onSearch(): void {
    if (this.searchQuery.trim()) {
      this.router.navigate(['/search'], { queryParams: { query: this.searchQuery } });
      console.log("searching for " + this.searchQuery)
      this.closeDropdown();
    }
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
