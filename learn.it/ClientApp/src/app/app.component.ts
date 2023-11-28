import { Component } from '@angular/core';
import { Router, NavigationEnd, Event as RouterEvent } from '@angular/router';
import { filter, map } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  showNavMenu: boolean = true;
  showNavLearn: boolean = false;

  constructor(private router: Router) {
    this.router.events.pipe(
      filter((event: RouterEvent): event is NavigationEnd => event instanceof NavigationEnd),
    ).subscribe((event: NavigationEnd) => {
      const hideNavMenuOnRoutes = ['/login', '/register'];
      const learningComponentRoutes = ['/learn']; //add more
      if (learningComponentRoutes.includes(event.urlAfterRedirects)) {
        this.showNavMenu = false;
        this.showNavLearn = true;
      }
      else {
        this.showNavMenu = !hideNavMenuOnRoutes.includes(event.urlAfterRedirects);
      }
      
    });
  }
}
