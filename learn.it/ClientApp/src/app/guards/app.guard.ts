import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AccountService } from '../services/account.service';

@Injectable({
  providedIn: 'root'
})
export class AppGuard implements CanActivate {
  constructor(private router: Router, private AccountService: AccountService) { }

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const isLoggedIn = this.AccountService.userValue?.token !== undefined;
    const isPublicRoute = ['search', 'login', 'register'].some(path => route.routeConfig?.path?.startsWith(path));

    if (!isLoggedIn && !isPublicRoute) {
      this.router.navigate(['/login']);
      return false;
    }
    return true;
  }

}
