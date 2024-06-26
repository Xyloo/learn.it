import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { LearningSetManagerComponent } from './learning-set-manager/learningSetManager.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatTooltipModule } from '@angular/material/tooltip';
import { NavLearnComponent } from './nav-learn/nav-learn.component';
import { LearningModuleComponent } from './learning-module/learning-module.component';
import { SearchComponent } from './search/search.component';
import { MultipleChoiceComponent } from './multiple-choice/multiple-choice.component';
import { FlashcardComponent } from './flashcard/flashcard.component';
import { InputQuizComponent } from './input-quiz/input-quiz.component';
import { UserSetsComponent } from './user-sets/user-sets.component';
import { SettingsComponent } from './settings/settings.component';
import { DynamicHostDirective } from './dynamic-host.directive';
import { ProfileComponent } from './settings/components/profile/profile.component';
import { PasswordComponent } from './settings/components/password/password.component';
import { GroupsComponent } from './settings/components/groups/groups.component';
import { ChooseGroupDialogComponent } from './choose-group-dialog/choose-group-dialog.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { AuthInterceptor } from './auth.interceptor';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { UserGroupsComponent } from './user-groups/user-groups.component';
import { GroupDetailsComponent } from './group-details/group-details.component';
import { CreateGroupDialogComponent } from './create-group-dialog/create-group-dialog.component';
import { InviteUserDialogComponent } from './invite-user-dialog/invite-user-dialog.component';
import { ChooseMethodDialogComponent } from '../choose-method-dialog/choose-method-dialog.component';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { AppGuard } from './guards/app.guard';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    LoginComponent,
    RegisterComponent,
    LearningSetManagerComponent,
    NavLearnComponent,
    LearningModuleComponent,
    SearchComponent,
    MultipleChoiceComponent,
    FlashcardComponent,
    InputQuizComponent,
    UserSetsComponent,
    SettingsComponent,
    DynamicHostDirective,
    ProfileComponent,
    PasswordComponent,
    GroupsComponent,
    ChooseGroupDialogComponent,
    UserGroupsComponent,
    GroupDetailsComponent,
    CreateGroupDialogComponent,
    InviteUserDialogComponent,
    ChooseMethodDialogComponent,

  ],
  imports: [
    MatTooltipModule,
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    MatSnackBarModule,
    MatSelectModule,
    MatButtonModule,
    MatSlideToggleModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full', canActivate: [AppGuard] },
      { path: 'counter', component: CounterComponent, canActivate: [AppGuard] },
      { path: 'login', component: LoginComponent, canActivate: [AppGuard] },
      { path: 'register', component: RegisterComponent, canActivate: [AppGuard] },
      { path: 'set', component: LearningSetManagerComponent, canActivate: [AppGuard] },
      { path: 'set/:id', component: LearningSetManagerComponent, canActivate: [AppGuard] },
      { path: 'learn/:id', component: LearningModuleComponent, canActivate: [AppGuard] },
      { path: 'search', component: SearchComponent, canActivate: [AppGuard] },
      { path: 'sets', component: UserSetsComponent, canActivate: [AppGuard] },
      { path: 'settings', component: SettingsComponent, canActivate: [AppGuard] },
      { path: 'groups', component: UserGroupsComponent, canActivate: [AppGuard] },
      { path: 'groups/:id', component: GroupDetailsComponent, canActivate: [AppGuard] },
    ]),
    BrowserAnimationsModule,
    ReactiveFormsModule
  ],
  providers: [{ provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }, AppGuard],
  bootstrap: [AppComponent]
})
export class AppModule { }
