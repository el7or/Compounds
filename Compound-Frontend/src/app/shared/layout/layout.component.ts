import { Subscription } from 'rxjs';
import { ChangeDetectorRef, Component, ElementRef, Input, ViewChild, AfterViewInit, OnDestroy, OnInit, AfterContentChecked } from '@angular/core';
import { MediaMatcher } from '@angular/cdk/layout';
import { MatDrawer, MatSidenav } from '@angular/material/sidenav';
import { LocalStorageService } from 'angular-2-local-storage';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { Portal } from '@angular/cdk/portal';
import { BreakpointObserver } from '@angular/cdk/layout';
import { LoadingService } from 'src/app/services/loading.serives';
import { ActivatedRoute, Router } from '@angular/router';
import { CurrentUserService } from 'src/app/services/current-user.serives';
import { SignalRService } from 'src/app/services/signal-r.service';
import Swal from 'sweetalert2';
import { TranslateService } from '@ngx-translate/core';
import { NotificationDrawerService } from 'src/app/services/notification-drawer.service';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent implements AfterViewInit, AfterContentChecked, OnDestroy {
  @ViewChild('sidenav') sidenav!: MatSidenav;
  @ViewChild('rightPanel') rightPanel!: MatSidenav;
  @ViewChild('notificationDrawer') notificationDrawer!: MatDrawer;
  @ViewChild('domPortalContent') domPortalContent!: ElementRef<HTMLElement>;
  @Input() showCompoundList: boolean = false;

  showText: boolean = this.localStorageService.get('nav~showText');
  selectedPortal!: Portal<any>;
  isSmallScreen: boolean;
  isItLoading: boolean = false;
  compoundId: string = '';
  isDrawerOpen?: boolean;

  subs = new Subscription();

  constructor(
    media: MediaMatcher,
    public loadingService: LoadingService,
    private localStorageService: LocalStorageService,
    public rightDrawer: RightDrawer,
    breakpointObserver: BreakpointObserver,
    private activatedRoute: ActivatedRoute,
    private signalRService: SignalRService,
    private currentUserService: CurrentUserService,
    private translateService: TranslateService,
    private router: Router,
    private notificationDrawerService: NotificationDrawerService,
    private cdr: ChangeDetectorRef
  ) {
    this.isSmallScreen = breakpointObserver.isMatched('(max-width: 599px)');
    breakpointObserver.observe('(max-width: 599px)').subscribe(result => this.isSmallScreen = result.matches);
    this.rightDrawer.panelPortal.subscribe(component => this.selectedPortal = component);
    loadingService.isItLoading.subscribe((isItLoading: boolean) => this.isItLoading = isItLoading);
    this.activatedRoute.params.subscribe(({ compoundId }) => this.compoundId = compoundId);
    this.subs.add(
      this.signalRService.forceLogoutUser.subscribe((userId: string) => {
        if (userId == currentUserService.currentUser.id) {
          Swal.fire({
            title: 'تنبيه هام !',
            text: 'تم تعديل الصلاحيات الخاصة بحسابكم من قبل الأدمن، مما يستلزم إعادة تسجيل الدخول.',
            icon: 'info',
            confirmButtonText: 'حسناً',
            confirmButtonColor: '#ffc529',
          }).then((result: any) => {
            if (result.value) {
              this.currentUserService.logout();
              this.router.navigate(['auth', 'login']);
            }
          });
        }
      })
    );
  }

  ngAfterViewInit() {
    setTimeout(() => {
      this.rightDrawer.panel = this.rightPanel;
      this.notificationDrawerService.drawerEl = this.notificationDrawer;
    })
  }

  ngAfterContentChecked() {
    this.cdr.detectChanges();
  }

  public showSidebar() {
    if (this.isSmallScreen) {
      this.showText = true;
      this.localStorageService.set('nav~showText', this.showText);
      this.sidenav.toggle();
    } else {
      this.sidenav.close();
      if (this.showCompoundList) {
        setTimeout(() => {
          this.showText = !this.showText;
          this.localStorageService.set('nav~showText', this.showText);
          this.sidenav.open();
        }, 550);
      }
    }
  }

  onOpenedChange(isOpen: boolean) {
    this.isDrawerOpen = isOpen;
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }
}
