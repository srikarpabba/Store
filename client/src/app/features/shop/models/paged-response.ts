export interface PagedResponse<T> {

    items: T[];

    pageIndex: number;

    pageSize: number;

    totalCount: number;

    totalPages: number;

    hasPrevious: boolean;

    hasNext: boolean;
}