import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MatCarouselModule } from '@ngmodule/material-carousel';
import { FormLayoutModule } from 'src/app/shared/form-layout/form-layout.module';
import { LayoutModule } from 'src/app/shared/layout/layout.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { FormIssueComponent } from '../issues/form-issue/form-issue.component';
import { IssueRequestComponent } from '../issues/issue-request/issue-request.component';
import { IssueComponent } from '../issues/issue/issue.component';

@NgModule({
  declarations: [IssueComponent, FormIssueComponent, IssueRequestComponent],
  imports: [
    SharedModule,
    FormLayoutModule,
    LayoutModule,
    MatCarouselModule.forRoot(),
    RouterModule.forChild([
      { path: '', component: IssueComponent },
      { path: 'form-issue', component: FormIssueComponent },
      { path: '**', pathMatch: 'full', redirectTo: "issue" }
    ]),
  ]

})
export class IssuesModule { }
