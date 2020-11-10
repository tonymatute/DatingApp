import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { PaginateResult } from '../_models/pagination';

 export function getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();

    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());

    return params;
  }

 export function getPaginationResult<T>(url: string, params: HttpParams, http: HttpClient) {
    const paginateResult: PaginateResult<T> = new PaginateResult<T>();

    return http
      .get<T>(url, { observe: 'response', params })
      .pipe(
        map((response) => {
          paginateResult.result = response.body;
          if (response.headers.get('Pagination') !== null) {
            paginateResult.pagination = JSON.parse(
              response.headers.get('Pagination')
            );
          }
          return paginateResult;
        })
      );
  }