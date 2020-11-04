export interface Pagination {
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
}

export class PaginateResult<T> {
    result: T;
    pagination: Pagination;
}