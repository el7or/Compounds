import { IGroup } from '../../../services/api/admin/compound-groups.service';
import { Injectable } from '@angular/core';
import { CompoundGroupsService } from 'src/app/services/api/admin/compound-groups.service';
import { CompoundsService } from 'src/app/services/api/admin/compounds.service';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { LoadingService } from 'src/app/services/loading.serives';
import { LanguageService } from 'src/app/services/language.serives';
import { UnitsService } from 'src/app/services/api/admin/units.service';
import { SubOwnersService } from 'src/app/services/api/admin/sub-owners.service';
import { HttpParams } from '@angular/common/http';


@Injectable({ providedIn: 'root' })
export class TreeDataMapService {

  constructor(
    private compoundsService: CompoundsService,
    private compoundGroupsService: CompoundGroupsService,
    private unitsService: UnitsService,
    private subOwnersService: SubOwnersService,
    private loadingService: LoadingService,
    private languageService: LanguageService
  ) { }

  initialData(compoundId: string): Observable<IGroup[]> {
    this.loadingService.itIsLoading = true;
    return this.compoundsService.getParentGroupsByCompoundId(compoundId).pipe(
      map(({ result }) => {
        this.loadingService.itIsLoading = false;
        return result.map((group: IGroup) =>
          ({ ...group, level: 0, name: this.languageService.getName(group), treeNodeType: TreeNodeType.group }))
      })
    )
  }

  getChildren(node: IGroup): Observable<IGroup[]> {
    let service: any;
    this.loadingService.itIsLoading = true;
    if (node.subGroupsCount > 0) {
      service = this.compoundGroupsService.getSubGroupsCompoundGroups(node.compoundGroupId).pipe(
        map(({ result }) => {
          this.loadingService.itIsLoading = false;
          return result.map((group: IGroup) => ({ ...group, level: node.level + 1, name: this.languageService.getName(group), treeNodeType: TreeNodeType.group }));
        })
      )
    }
    if (node.unitsCount > 0) {
      service = this.compoundGroupsService.getUnitsCompoundGroups(node.compoundGroupId).pipe(
        map(({ result }) => {
          this.loadingService.itIsLoading = false;
          return result.map((group: IGroup) => ({ ...group, level: node.level + 1, treeNodeType: TreeNodeType.unit }));
        })
      )
    }
    if (node.ownersCount > 0) {
      service = this.unitsService.getOwnersByUnitId(node.compoundUnitId).pipe(
        map(({ result }) => {
          this.loadingService.itIsLoading = false;
          return result.map((group: IGroup) => ({ ...group, level: node.level + 1, treeNodeType: TreeNodeType.owner }));
        })
      )
    }
    if (node.subOwners > 0) {
      service = this.subOwnersService.getSubOwners(new HttpParams().set('mainRegistrationId', node.ownerRegistrationId)).pipe(
        map(({ result }) => {
          this.loadingService.itIsLoading = false;
          return result.map((group: IGroup) => ({ ...group, level: node.level + 1, treeNodeType: TreeNodeType.subowner }));
        })
      )
    }
    return service;
  }

  isExpandable(node: IGroup): boolean {
    return node.subGroupsCount > 0 || node.unitsCount > 0 || node.ownersCount > 0 || node.subOwners > 0;
    // return node.subGroupsCount + node.unitsCount > 0;
  }
}

export enum TreeNodeType { group, unit, owner, subowner }

