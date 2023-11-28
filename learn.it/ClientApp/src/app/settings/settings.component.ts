import { Component, ComponentFactory, ComponentFactoryResolver, ViewChild } from '@angular/core';
import { DynamicHostDirective } from '../dynamic-host.directive';
import { Location } from '@angular/common';
import { ProfileComponent } from './components/profile/profile.component';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})
export class SettingsComponent {

  @ViewChild(DynamicHostDirective, { static: true }) dynamicHost: DynamicHostDirective;
  constructor(private componentFactoryResolver: ComponentFactoryResolver, private location: Location) { }

  selectedTab = 'profile'; 

  ngOnInit() {
    this.loadComponent(this.selectedTab); 
  }

  loadComponent(componentName: string) {
    const viewContainerRef = this.dynamicHost.viewContainerRef;
    viewContainerRef.clear();

    let componentFactory: ComponentFactory<any>;
    switch (componentName) {
      case 'profile':
          componentFactory = this.componentFactoryResolver.resolveComponentFactory(ProfileComponent);
        break;
      default:
        componentFactory = this.componentFactoryResolver.resolveComponentFactory(ProfileComponent);
        break;
    }
    if(componentFactory)
      viewContainerRef.createComponent(componentFactory);
  }

  goBack() {
    this.location.back();
  }

}
