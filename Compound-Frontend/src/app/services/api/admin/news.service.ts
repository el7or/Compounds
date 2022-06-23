import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Iimage, PagedInput, WebApiService } from '../../webApi.service';

@Injectable({
  providedIn: 'root'
})
export class NewsService {

  private controller: string = 'CompoundNews';
  private serviceSubDomain: string = environment.serviceSubDomain_admin;

  constructor(private webApi: WebApiService) { }

  getFilteredNews(newsFilter: NewsFilter) {
    return this.webApi.getWithFilter(this.serviceSubDomain, `${this.controller}`, newsFilter);
  }
  getNews(newsId: string) {
    return this.webApi.get(this.serviceSubDomain, `${this.controller}/${newsId}`);
  }
  postNews(body: News) {
    return this.webApi.post(this.serviceSubDomain, `${this.controller}`, body);
  }
  putNews(body: News) {
    return this.webApi.put(this.serviceSubDomain, `${this.controller}`, body);
  }
  deleteNews(newsId: string) {
    return this.webApi.delete(this.serviceSubDomain, `${this.controller}/${newsId}`);
  }
}

export class NewsFilter extends PagedInput {
  compoundId?: string;
  companyId?: string;
  publishDateFrom?: Date;
  publishDateTo?: Date;
  isActive?:boolean;
  searchText?:string;
  public constructor(init?: Partial<NewsFilter>) {
    super(init);
    Object.assign(this, init);
  }
}

export class News {
  compoundNewsId!: string;
  compoundId!: string;
  companyId!: string;
  englishTitle!: string;
  arabicTitle!: string;
  englishSummary!: string;
  arabicSummary!: string;
  englishDetails!: string;
  arabicDetails!: string;
  publishDate!: Date | string;
  foregroundTillDate?: Date | string;
  isActive!: boolean;
  images?: Iimage[];
}
