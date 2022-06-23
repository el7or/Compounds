import { NgModule } from '@angular/core';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { MatMenuModule } from '@angular/material/menu';
import { MatSelectModule } from '@angular/material/select';
import { FormsModule } from '@angular/forms';
import { MatSidenavModule } from '@angular/material/sidenav';

import { SidebarComponent } from './sidebar/sidebar.component';
import { FooterComponent } from './footer/footer.component';
import { LayoutComponent } from './layout.component';
import { MatToolbarModule } from '@angular/material/toolbar';
import { NavbarComponent } from './navbar/navbar.component';
import { TranslateModule } from '@ngx-translate/core';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatRippleModule } from '@angular/material/core';
import { PortalModule } from '@angular/cdk/portal';
import { LayoutModule as MaterialLayoutMoudle } from '@angular/cdk/layout';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { NotificationDrawerComponent } from './notification-drawer/notification-drawer.component';
import { MatCardModule } from '@angular/material/card';
import { MatBadgeModule } from '@angular/material/badge';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@NgModule({
  declarations: [
    NavbarComponent,
    SidebarComponent,
    FooterComponent,
    LayoutComponent,
    NotificationDrawerComponent
  ],
  imports: [
    TranslateModule,
    MatListModule,
    MatIconModule,
    RouterModule,
    MatButtonModule,
    CommonModule,
    MatMenuModule,
    MatSelectModule,
    FormsModule,
    MatSidenavModule,
    MatToolbarModule,
    MatTooltipModule,
    MatRippleModule,
    PortalModule,
    MaterialLayoutMoudle,
    MatProgressBarModule,
    MatCardModule,
    MatBadgeModule,
    MatProgressSpinnerModule,
  ],
  exports: [
    NavbarComponent,
    SidebarComponent,
    FooterComponent,
    LayoutComponent,
  ]
})
export class LayoutModule { }
