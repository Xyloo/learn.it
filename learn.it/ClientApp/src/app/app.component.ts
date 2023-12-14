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
      const learningComponentRoutes = ['/learn'];

      const isLearningRoute = learningComponentRoutes.some(route => event.urlAfterRedirects.startsWith(route));
      const shouldHideNavMenu = hideNavMenuOnRoutes.some(route => event.urlAfterRedirects.startsWith(route));

      this.showNavLearn = isLearningRoute;
      this.showNavMenu = !shouldHideNavMenu && !isLearningRoute;
    });

  }
}
