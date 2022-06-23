import { CollectionViewer, SelectionChange } from "@angular/cdk/collections";
import { DataSource } from "@angular/cdk/table";
import { FlatTreeControl } from "@angular/cdk/tree";
import { BehaviorSubject, Observable, merge } from "rxjs";
import { map } from "rxjs/operators";
import { IGroup } from "src/app/services/api/admin/compound-groups.service";
import { TreeDataMapService, TreeNodeType } from "./tree-data-map.service";

export class DynamicTreeDataSource implements DataSource<IGroup> {
  dataChange = new BehaviorSubject<IGroup[]>([]);

  constructor(
    private _treeControl: FlatTreeControl<IGroup>,
    private _database: TreeDataMapService
  ) { }

  get data(): IGroup[] { return this.dataChange.value; }
  set data(value: IGroup[]) {
    this._treeControl.dataNodes = value;
    this.dataChange.next(value);
  }

  connect(collectionViewer: CollectionViewer): Observable<IGroup[]> {
    this._treeControl.expansionModel.changed.subscribe(change => {
      if ((change as SelectionChange<IGroup>).added ||
        (change as SelectionChange<IGroup>).removed) {
        this.handleTreeControl(change as SelectionChange<IGroup>);
      }
    });
    return merge(collectionViewer.viewChange, this.dataChange).pipe(map(() => this.data));
  }

  disconnect(collectionViewer: CollectionViewer): void { }

  handleTreeControl(change: SelectionChange<IGroup>) {
    if (change.added) {
      change.added.forEach(node => this.toggleNode(node, true));
    }
    if (change.removed) {
      // change.removed.slice().reverse().forEach(node => this.toggleNode(node, false));
      change.removed.slice().forEach(node => this.toggleNode(node, false));
    }

  }

  toggleNode(node: IGroup, expand: boolean) {
    const index = this.data.findIndex(i => {
      switch (node.treeNodeType) {
        case TreeNodeType.group:
          return i.compoundGroupId == node.compoundGroupId;
        case TreeNodeType.unit:
          return i.compoundUnitId == node.compoundUnitId;
          case TreeNodeType.owner:
            return i.compoundOwnerId == node.compoundOwnerId;
        default:
          return i.compoundGroupId == node.compoundGroupId;
      }
    });
    console.log(index, { node });

    if (expand) {
      node.isLoading = true;
      this._database.getChildren(node).subscribe(child => {
        this.data.splice(index + 1, 0, ...child as any[]);
        this.dataChange.next(this.data);
        node.isLoading = false;
      });
    } else {
      let count = 0;
      for (let i = index + 1; i < this.data.length && this.data[i].level > node.level; i++, count++) { }
      this.data.splice(index + 1, count);
      this.dataChange.next(this.data);
    }
  }

  getParentsNode(node: IGroup): (IGroup | null | undefined) {
    const currentLevel = this._treeControl.getLevel(node);
    if (currentLevel < 1) return null;
    let startIndex: number = 0;
    switch (node.treeNodeType) {
      case TreeNodeType.group:
        startIndex = this.data.findIndex(i => i.compoundGroupId == node.compoundGroupId) - 1;
        break;
      case TreeNodeType.unit:
        startIndex = this.data.findIndex(i => i.compoundUnitId == node.compoundUnitId) - 1;
        break;
      case TreeNodeType.owner:
        startIndex = this.data.findIndex(i => i.ownerRegistrationId == node.ownerRegistrationId) - 1;
        break;
      case TreeNodeType.subowner:
        startIndex = this.data.findIndex(i => i.compoundOwnerId == node.compoundOwnerId) - 1;
        break;
    }
    console.log({ node });

    for (let i = startIndex; i >= 0; i--) {
      const currentNode = this.data[i];
      if (this._treeControl.getLevel(currentNode) < currentLevel) {
        return currentNode;
      }
    }
    return undefined;
  }
}
