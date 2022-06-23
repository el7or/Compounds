import { SignalRService } from './../../../services/signal-r.service';
import { IGroup } from '../../../services/api/admin/compound-groups.service';
import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { CompoundGroupsService } from 'src/app/services/api/admin/compound-groups.service';
import { ICompound } from 'src/app/services/api/admin/compounds.service';
import { LanguageService } from 'src/app/services/language.serives';
import { RightDrawer } from 'src/app/services/right-drawer.serives';
import { FlatTreeControl } from '@angular/cdk/tree';
import { MatMenuTrigger } from '@angular/material/menu';
import { FormUnitComponent } from '../form-unit/form-unit.component';
import { FormGroupComponent } from '../form-group/form-group.component';
import { FormOwnerComponent } from '../form-owner/form-owner.component';
import { TreeDataMapService, TreeNodeType } from './tree-data-map.service';
import { DynamicTreeDataSource } from './dynamic-tree-data-source.class';
import { CompoundsStoreService } from 'src/app/services/compounds-store.serives';
import { MatDialog } from '@angular/material/dialog';
import { DialogLayoutComponent } from 'src/app/shared/dialog-layout/dialog-layout.component';
import { IAddUnitsOwners, IUnitOwner, UnitsService } from 'src/app/services/api/admin/units.service';
import { CompoundOwnersService } from 'src/app/services/api/admin/compound-owners.service';
import { OwnerFormComponent } from '../../owners/owner-form/owner-form.component';
import { OwnerType } from 'src/app/services/api/admin/sub-owners.service';
import * as XLSX from 'xlsx';
import Swal from 'sweetalert2';
import { LoadingService } from 'src/app/services/loading.serives';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-compound-tree',
  templateUrl: './compound-tree.component.html',
  styleUrls: ['./compound-tree.component.scss']
})
export class CompoundTreeComponent implements OnInit, OnDestroy {
  TreeNodeType = TreeNodeType;
  selectedCompound: ICompound | undefined;
  selectedNode: IGroup | undefined;
  selectedCompoundId!: string;

  treeControl: FlatTreeControl<IGroup>;
  dataSource: DynamicTreeDataSource;

  @ViewChild(MatMenuTrigger)
  contextMenu: MatMenuTrigger | undefined;
  contextMenuPosition = { x: '0px', y: '0px' };

  progressBarValue = 0;
  dataExcelCount = 0;
  uploadedDataExcelCount = 0;

  constructor(
    public languageService: LanguageService,
    public activatedRoute: ActivatedRoute,
    public rightDrawer: RightDrawer,
    public treeDataMapService: TreeDataMapService,
    private compoundsStoreService: CompoundsStoreService,
    private matDialog: MatDialog,
    private snackBar: MatSnackBar,
    private compoundGroupsService: CompoundGroupsService,
    private unitsService: UnitsService,
    private loadingService: LoadingService,
    private translateService: TranslateService,
    private compoundOwnersService: CompoundOwnersService,
    private signalRService: SignalRService
  ) {
    this.treeControl = new FlatTreeControl<IGroup>((node: IGroup) => node.level, this.treeDataMapService.isExpandable);
    this.dataSource = new DynamicTreeDataSource(this.treeControl, treeDataMapService);

    this.signalRService.addedExcelOwnersCount.subscribe((count: number) => {
      this.uploadedDataExcelCount = count;
      this.progressBarValue = count / this.dataExcelCount * 100;
    });
  }

  hasChild = (_: number, _nodeData: IGroup) => this.treeControl.isExpandable(_nodeData);

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(({ compoundId }) => {
      this.selectedCompoundId = compoundId;
      this.selectedCompound = this.compoundsStoreService.compounds.find(i => i.compoundId == this.selectedCompoundId);
      this.initialTreeData();
    });
  }

  ngOnDestroy() {
    //this.signalRService.stopConnection();
  }

  initialTreeData() {
    this.treeDataMapService.initialData(this.selectedCompoundId).subscribe(data => {
      this.dataSource.data = data;
    })
  }

  onContextMenu(event: MouseEvent, item: IGroup) {
    if (this.selectedNode!.treeNodeType != TreeNodeType.subowner) {
      event.preventDefault();
      this.contextMenuPosition.x = event.clientX + 'px';
      this.contextMenuPosition.y = event.clientY + 'px';
      if (this.contextMenu) {
        this.contextMenu.menuData = { 'item': item };
        this.contextMenu.menu.focusFirstItem('mouse');
        this.contextMenu.openMenu();
      }
    }
  }

  selectNode(node: IGroup) {
    if (node.treeNodeType == 3 && !isNaN(+node.userType)) {
      node.userType = OwnerType[node.userType];
      this.selectedNode = node;
    } else {
      this.selectedNode = node;
    }
  }

  treeDataChange() {
    let dataSource = [...this.dataSource.data]
    this.dataSource.data = [];
    this.dataSource.data = dataSource;
  }

  openGroupForm(node: IGroup | any, forEdit: boolean, forRoot: boolean = false) {
    node.forEdit = forEdit;
    this.rightDrawer.open(FormGroupComponent, node).subscribe((arg: any) => {
      if (arg != null) {
        if (forRoot) {
          if (this.selectedCompound)
            this.treeDataMapService.initialData(this.selectedCompound.compoundId).subscribe(data => {
              this.dataSource.data = data;
            })
        } else {
          if (node.forEdit) {
            node.name = this.languageService.getName(arg);
            node.nameAr = arg.nameAr;
            node.nameEn = arg.nameEn;
          } else {
            node.subGroupsCount = node.subGroupsCount + 1;
            this.treeControl.collapse(node);
            this.treeDataChange();
            this.treeControl.expand(node);
          }
        }
      }
    });
  }

  deleteGroup(node: IGroup) {
    this.matDialog.open(DialogLayoutComponent, { data: { massage: 'tree.deleteGroup' } })
      .afterClosed().subscribe((confirm) => {
        if (confirm) {
          this.compoundGroupsService.DeleteCompoundGroups(node.compoundGroupId).subscribe(({ ok, result, message }) => {
            if (ok) {
              this.snackBar.open(result, 'OK', {
                duration: 3000,
                horizontalPosition: 'start',
                panelClass: ['bg-primary']
              });
              if (!node.parentGroupId) {
                if (this.selectedCompound)
                  this.treeDataMapService.initialData(this.selectedCompound.compoundId).subscribe(data => {
                    this.dataSource.data = data;
                  });
              } else {
                let parentNode = this.dataSource.getParentsNode(node);
                if (parentNode) {
                  parentNode.subGroupsCount = parentNode.subGroupsCount - 1;
                  if (!parentNode.subGroupsCount) {
                    this.treeControl.collapse(parentNode);
                    this.treeDataChange();
                  } else {
                    this.treeControl.expand(parentNode);
                    this.treeControl.collapse(parentNode);
                  }
                }
              }
            } else {
              this.snackBar.open(message, 'OK', {
                duration: 3000,
                horizontalPosition: 'start',
                panelClass: ['bg-danger']
              });
            }
          })
        }
      });
  }

  openUnitForm(node: IGroup, forEdit: boolean) {
    node.forEdit = forEdit;
    this.rightDrawer.open(FormUnitComponent, node).subscribe((arg: any) => {
      if (arg != null) {
        if (node.forEdit) {
          node.name = arg.name;
          node.compoundUnitTypeId = arg.compoundUnitTypeId;
        } else {
          node.unitsCount = node.unitsCount + 1;
          this.treeControl.collapse(node);
          this.treeDataChange();
          this.treeControl.expand(node);
        }
      }
    });
  }

  deleteUnit(node: IGroup) {
    this.matDialog.open(DialogLayoutComponent, { data: { massage: 'tree.deleteUnit' } })
      .afterClosed().subscribe((confirm) => {
        if (confirm) {
          this.unitsService.DeleteUnits(node.compoundUnitId).subscribe(({ ok, result, message }) => {
            if (ok) {
              this.snackBar.open(result, 'OK', {
                duration: 3000,
                horizontalPosition: 'start',
                panelClass: ['bg-primary']
              });
              let parentNode = this.dataSource.getParentsNode(node);
              if (parentNode) {
                parentNode.unitsCount = parentNode.unitsCount - 1;
                if (!parentNode.unitsCount) {
                  this.treeControl.collapse(parentNode);
                  this.treeDataChange();
                } else {
                  this.treeControl.collapse(parentNode);
                  this.treeControl.expand(parentNode);
                }
              }
            } else {
              this.snackBar.open(message, 'OK', {
                duration: 3000,
                horizontalPosition: 'start',
                panelClass: ['bg-danger']
              });
            }
          })
        }
      });
  }

  openOwnerForm(node: IGroup, forEdit: boolean) {
    node.forEdit = forEdit;
    if (forEdit) {
      this.compoundOwnersService.getCompoundOwnerById(node.compoundOwnerId).subscribe(({ ok, result, message }) => {
        if (ok) {
          result.fromTree = true;
          this.rightDrawer.open(OwnerFormComponent, result).subscribe((arg: any) => {
            //this.ngOnInit();
            this.treeControl.collapse(node);
            this.treeDataChange();
            this.treeControl.expand(node);
          })
        } else {
          this.snackBar.open(message, 'ERROR', {
            duration: 3000,
            horizontalPosition: 'start',
            panelClass: ['bg-danger']
          })
        }
      })
    }
    else {
      this.rightDrawer.open(FormOwnerComponent, node).subscribe((arg: any) => {
        if (arg != null) {
          node.ownersCount = node.ownersCount + 1;
          this.treeControl.collapse(node);
          this.treeDataChange();
          this.treeControl.expand(node);
        }
      });
    }
  }
  //todo
  deleteOwner(node: IGroup) {
    this.matDialog.open(DialogLayoutComponent, { data: { massage: 'tree.deleteUnit' } })
      .afterClosed().subscribe((confirm) => {
        if (confirm) {
          this.compoundOwnersService.DeleteCompoundOwner(node.compoundUnitId).subscribe(({ ok, result, message }) => {
            if (ok) {
              this.snackBar.open(result, 'OK', {
                duration: 3000,
                horizontalPosition: 'start',
                panelClass: ['bg-primary']
              });
              let parentNode = this.dataSource.getParentsNode(node);
              if (parentNode) {
                parentNode.unitsCount = parentNode.unitsCount - 1;
                if (!parentNode.unitsCount) {
                  this.treeControl.collapse(parentNode);
                  this.treeDataChange();
                } else {
                  this.treeControl.collapse(parentNode);
                  this.treeControl.expand(parentNode);
                }
              }
            } else {
              this.snackBar.open(message, 'OK', {
                duration: 3000,
                horizontalPosition: 'start',
                panelClass: ['bg-danger']
              });
            }
          })
        }
      });
  }

  getNodeIcon(node: IGroup): string {
    let iconName: string = ''
    switch (node.treeNodeType) {
      case TreeNodeType.group:
        iconName = 'apartment';
        break;
      case TreeNodeType.unit:
        iconName = 'house';
        break;
      case TreeNodeType.owner:
        iconName = 'face';
        break;
      case TreeNodeType.subowner:
        iconName = 'supervised_user_circle';
        break;
    }
    return iconName;
  }

  onImportExcel(event: any) {
    const target: DataTransfer = <DataTransfer>(event.target);
    if (target.files.length !== 1) {
      throw new Error('Cannot use multiple files');
    }
    const reader: FileReader = new FileReader();
    reader.readAsBinaryString(target.files[0]);
    reader.onload = (e: any) => {
      const binarystr: string = e.target.result;
      const wb: XLSX.WorkBook = XLSX.read(binarystr, { type: 'binary' });
      const wsname: string = wb.SheetNames[0];
      const ws: XLSX.WorkSheet = wb.Sheets[wsname];
      const data: IUnitOwner[] = XLSX.utils.sheet_to_json(ws);
      this.dataExcelCount = data.length;
      if (data.some(d => d.group == null || d.unit == null || d.ownerName == null || d.mobile == null || d.unitType == null)) {
        Swal.fire({
          title: this.translateService.instant('tree.invalidData'),
          text: this.translateService.instant('tree.invalidDataText'),
          icon: 'error',
          backdrop: false,
          confirmButtonText: this.translateService.instant('tree.cancel'),
        });
        event.target.value = null;
      } else if (data.some(d => isNaN(+d.unitType))) {
        Swal.fire({
          title: this.translateService.instant('tree.invalidData'),
          text: this.translateService.instant('tree.invalidUnitType'),
          icon: 'error',
          backdrop: false,
          confirmButtonText: this.translateService.instant('tree.cancel'),
        });
        event.target.value = null;
      } else {
        Swal.fire({
          title: this.translateService.instant('tree.confirm'),
          text: this.translateService.instant('tree.confirmText') + this.dataExcelCount,
          icon: 'info',
          showCancelButton: true,
          backdrop: false,
          confirmButtonColor: '#1d6f42',
          confirmButtonText: this.translateService.instant('tree.continue'),
          cancelButtonText: this.translateService.instant('tree.cancel'),
        }).then((result: any) => {
          if (result.value) {
            this.loadingService.itIsLoading = true;
            const unitsOwners: IAddUnitsOwners = {
              compoundId: this.selectedCompoundId,
              unitsOwners: data,
              connectionId: this.signalRService.connectionId!
            };
            this.unitsService.importUnitsWithOwners(unitsOwners).subscribe(({ ok, result, message }) => {
              if (ok) {
                this.snackBar.open(this.translateService.instant('tree.done'), 'OK', {
                  duration: 3000,
                  horizontalPosition: 'start',
                  panelClass: ['bg-success']
                });
              } else {
                this.snackBar.open(message, 'OK', {
                  duration: 3000,
                  horizontalPosition: 'start',
                  panelClass: ['bg-danger']
                });
              }
              event.target.value = null;
              this.initialTreeData();
              this.loadingService.itIsLoading = false;
            });
          }
        });
      }
    };
  }
}
