import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Iimage, PagedInput, WebApiService } from '../../webApi.service';

@Injectable({
  providedIn: 'root'
})
export class AdsService {
  private controller: string = 'CompoundAds';
  private serviceSubDomain: string = environment.serviceSubDomain_ads;

  constructor(private webApi: WebApiService) { }

  getFilteredAds(adsFilter: AdsFilter) {
    return this.webApi.getWithFilter(this.serviceSubDomain, `${this.controller}`, adsFilter);
  }
  getAdById(adId: string) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/${adId}`);
  }
  postAd(body: Ad) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}`, body);
  }
  putAd(body: Ad) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}`, body);
  }
  deleteAd(adId: string) {
    return this.webApi.delete(this.serviceSubDomain, `${this.controller}/${adId}`);
  }
}

export class AdsFilter extends PagedInput {
  compoundId?: string;
  searchText?:string;
  public constructor(init?: Partial<AdsFilter>) {
    super(init);
    Object.assign(this, init);
  }
}

export class Ad {
  compoundAdId!: string;
  compoundId!: string;
  startDate!: Date | string;
  endDate!: Date | string;
  isUrl!: string;
  adUrl!: string;
  englishTitle!: string;
  arabicTitle!: string;
  englishDescription!: string;
  arabicDescription!: string;
  showsCount!:number;
  clicksCount!:number;
  uniqueShowsCount!:number;
  uniqueClicksCount!:number;
  isActive!: boolean;
  images?: Iimage[];
}
