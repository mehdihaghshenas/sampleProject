export interface CompanyInput {
    id: string;
    name: string;
}

export interface FilterRes<T> {
    data: T[];
    totalCount: number;
    pageNumber: number;
    pageSize: number;
}

export interface ComapnyOutput {
    id: number;
    name: string;
}

export interface FilterModel {
    whereConditionText: string;
    sortText: string;
    pageSize: number;
    pageNumber: number;
    disablePaging: boolean;
}



export const emptyFilter :FilterModel= {
    "whereConditionText": "",
    "sortText": "",
    "pageSize": 10,
    "pageNumber": 1,
    "disablePaging": false
}