import { Component, HostListener } from '@angular/core';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {

  showDropdown: boolean = false;
  toggleDropdown() {
    this.showDropdown = !this.showDropdown;
  }

  closeDropdown() {
    this.showDropdown = false;
  }

  @HostListener('document:click', ['$event'])
  clickOutside(event: { target: { closest: (arg0: string) => any; }; }) {
    if (!event.target.closest('.dropdown')) {
      this.closeDropdown();
    }
  }
}
