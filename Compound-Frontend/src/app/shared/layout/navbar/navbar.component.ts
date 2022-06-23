import { Input, OnInit, Output } from '@angular/core';
import { Component, EventEmitter } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { LocalStorageService } from 'angular-2-local-storage';
import { FormCompoundComponent } from 'src/app/pages/compound/form-compound/form-compound.component';
import { ICompound } from 'src/app/services/api/admin/compounds.service';
import { CompoundsStoreService } from 'src/app/services/compounds-store.serives';
import { LanguageService } from 'src/app/services/language.serives';
import { NotificationDrawerService } from 'src/app/services/notification-drawer.service';
import { RightDrawer } from 'src/app/services/right-drawer.serives';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {
  @Input() isSmallScreen: boolean = false;
  @Input() showCompoundList = false;
  @Output() showSidebar = new EventEmitter<boolean>();
  isMenuOpened: boolean = this.isSmallScreen ? this.localStorageService.get('nav~showText') : false;
  currentLang: string = '';

  selectedCompound: ICompound | undefined;
  compounds: ICompound[] = [];
  urlLastSegment: string = '';


  constructor(
    private localStorageService: LocalStorageService,
    public languageService: LanguageService,
    public activatedRoute: ActivatedRoute,
    public rightDrawer: RightDrawer,
    public compoundsStoreService: CompoundsStoreService,
    public notificationDrawerService: NotificationDrawerService
  ) {
    if (this.languageService.lang == 'en') {
      this.currentLang = 'العربية';
    } else {
      this.currentLang = 'English';
    }
  }

  getUserCompounds() {
    this.compounds = this.compoundsStoreService.compounds;

    if (this.showCompoundList) {
      this.activatedRoute.pathFromRoot[1].url.subscribe(([_, url]) => {
        if (url) {
          this.urlLastSegment = url.path;
        } else {
          this.urlLastSegment = 'dashboard';
        }
      });
    }

    this.activatedRoute.params.subscribe(({ compoundId }) =>
      this.selectedCompound = this.compounds.find(i => i.compoundId == compoundId)
    )
  }
  ngOnInit(): void {
    this.getUserCompounds();
  }
  changeLang() {
    if (this.languageService.lang == 'en') {
      this.languageService.selectLang('ar');
    } else {
      this.languageService.selectLang('en');
    }
  }
  openMenu(): void {
    this.isMenuOpened = !this.isMenuOpened;
    this.showSidebar.emit(this.isMenuOpened);
  }
  openForm(data: any = null) {
    this.rightDrawer.open(FormCompoundComponent, data).subscribe((arg: any) => {
      if (arg != null) {
        this.getUserCompounds();
      }
    });
  }
}
