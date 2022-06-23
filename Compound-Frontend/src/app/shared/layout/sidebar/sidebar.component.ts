import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CompoundsService } from 'src/app/services/api/admin/compounds.service';
import { CurrentUserService } from 'src/app/services/current-user.serives';
import { LanguageService } from 'src/app/services/language.serives';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { SystemPageActions } from 'src/app/services/system-page-actions.enum';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent implements OnInit {
  compoundId: string | undefined;
  constructor(
    public rightDrawer: RightDrawer,
    private compoundsService: CompoundsService,
    public currentUserService: CurrentUserService,
    private activatedRoute: ActivatedRoute,
    public languageService: LanguageService) {

  }
  ngOnInit(): void {
    this.activatedRoute.params.subscribe(({ compoundId }) => this.compoundId = compoundId);
  }


  @Input() showText: boolean = false;
  public routes: {
    name: string;
    path?: string,
    icon?: string,
    type: RouteType,
    roleAction?: SystemPageActions,
    child?: {
      name: string;
      path?: string,
      icon?: string,
      type: RouteType
    }[];
  }[] = [
      {
        name: 'menu.dashboard',
        path: '/dashboard',
        icon: 'bar_chart',
        type: RouteType.LINK,
        roleAction: SystemPageActions.UnitsDisplayandFullManagment
      },
      {
        name: 'menu.units',
        path: '/units',
        icon: 'maps_home_work',
        type: RouteType.HEDER,
        roleAction: SystemPageActions.UnitsDisplayandFullManagment
        // child:[
        //   {
        //     name: 'Unit',
        //     path: '/home',
        //     icon:'door_sliding',
        //     type: RouteType.LINK
        //   },
        //   {
        //     name: 'Unit2',
        //     path: '/home',
        //     icon:'door_sliding',
        //     type: RouteType.LINK
        //   }
        // ]
      },
      {
        name: 'menu.gates',
        path: '/gates',
        icon: 'door_sliding',
        type: RouteType.LINK,
        roleAction: SystemPageActions.GatesDisplay
      },
      {
        name: 'menu.services',
        path: '/services',
        icon: 'miscellaneous_services',
        type: RouteType.LINK,
        roleAction: SystemPageActions.ServicesDisplay
      },
      {
        name: 'menu.issues',
        path: '/issues',
        icon: 'report_problem',
        type: RouteType.LINK,
        roleAction: SystemPageActions.IssuesDisplay
      },
      {
        name: 'menu.visits',
        path: '/visits',
        icon: 'meeting_room',
        type: RouteType.LINK,
        roleAction: SystemPageActions.VisitDisplay
      },
      {
        name: 'menu.ads',
        path: '/ads',
        icon: 'featured_video',
        type: RouteType.LINK,
        roleAction: SystemPageActions.AdsDisplay
      },
      {
        name: 'menu.owners',
        path: '/owners',
        icon: 'person',
        type: RouteType.LINK,
        roleAction: SystemPageActions.OwnersDisplay
      },
      {
        name: 'menu.notification',
        path: '/notifications',
        icon: 'notifications',
        type: RouteType.LINK,
        roleAction: SystemPageActions.NotificationsDisplay
      },
      {
        name: 'menu.news',
        path: '/news',
        icon: 'article',
        type: RouteType.LINK,
        roleAction: SystemPageActions.NewsDisplay
      },
      {
        name: 'menu.stores',
        path: '/home',
        icon: 'local_mall',
        type: RouteType.LINK,
        roleAction: SystemPageActions.StoresDisplay
      }
    ];
  public RouteType = RouteType;

}
export enum RouteType { HEDER, LINK, DROPDOWN }
