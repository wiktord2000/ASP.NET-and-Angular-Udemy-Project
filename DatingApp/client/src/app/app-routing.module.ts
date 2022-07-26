import { PreventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { TextErrorsComponent } from './errors/text-errors/text-errors.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ListsComponent } from './lists/lists.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { AuthGuard } from './_guards/auth.guard';


const routes: Routes = [
  {path: '' , component: HomeComponent},
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [

      {path: 'members' , component: MemberListComponent},
      {path: 'members/:username' , component: MemberDetailComponent},
      // {path: 'members/edit' , component: MemberDetailComponent, pathMatch: 'full'}, -> If we want to have fath members/edit instead member/edit
      {path: 'member/edit' , component: MemberEditComponent, canDeactivate: [PreventUnsavedChangesGuard]},
      {path: 'lists' , component: ListsComponent},
      {path: 'messages' , component: MessagesComponent},
    ]

  },
  {path: 'errors' , component: TextErrorsComponent},
  {path: 'not-found', component: NotFoundComponent},
  {path: 'server-error', component: ServerErrorComponent},
  // When route doean't exist at all - redirect
  {path: '**' , component: NotFoundComponent, pathMatch: 'full'},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
