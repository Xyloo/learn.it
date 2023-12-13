import { Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from '../services/account.service';

@Component({
    selector: 'app-nav-menu',
    templateUrl: './nav-menu.component.html',
    styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit {

    @ViewChild('searchInput') searchInput: ElementRef;
    searchQuery: string = '';
  public username: string = '';
  img = 'Avatars/cb528457-71c0-4793-bb02-519989040eda.webp';
    constructor(private router: Router, private accountService: AccountService) { }
    ngOnInit(): void {
        this.username = this.accountService.userValue?.uniqueName || '';
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
