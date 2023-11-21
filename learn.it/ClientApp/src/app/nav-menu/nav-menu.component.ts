import { Component, ElementRef, HostListener, ViewChild } from '@angular/core';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {

  @ViewChild('searchInput') searchInput: ElementRef;

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

  @HostListener('document:click', ['$event'])
  clickOutside(event: { target: { closest: (arg0: string) => any; }; }) {
    if (!event.target.closest('.dropdown')) {
      this.closeDropdown();
    }
  }
}
