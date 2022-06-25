import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';

@Injectable({ providedIn: 'root' })
export class ShortcutService {


  constructor(private Cookie: CookieService, private http: HttpClient) { }

  SetCookie(name: string, value: string, date: Date | number, path?: string) {
    if (path == undefined) {
      path = '/';
    }
    this
      .Cookie
      .set(name, value, date, path, undefined);
    // .set(name, value, date, path, null, true);
  }

  DeleteCookie(name: string, path?: string) {
    if (path == undefined) {
      path = '/';
    }
    this
      .Cookie
      .delete(name, path);
  }

  getcookie(name: string) {
    return this
      .Cookie
      .get(name);
  }

  checkcookie(name: string) {
    return this
      .Cookie
      .check(name);
  }

  replaceChar(value: any) {
    if (!value) return value;
    if (typeof value == 'string') value = value.trim();
    return value.replace(/[۰-۹]/g, (d: string) => '۰۱۲۳۴۵۶۷۸۹'.indexOf(d)).replace(/[٠-٩]/g, (d: string) => '٠١٢٣٤٥٦٧٨٩'.indexOf(d))
      .replace(/ﮎ/g, 'ک')
      .replace(/ﮏ/g, 'ک')
      .replace(/ﮐ/g, 'ک')
      .replace(/ﮑ/g, 'ک')
      .replace(/ك/g, 'ک')
      .replace(/ي/g, 'ی')
      .replace(/ /g, ' ')
      .replace(/‌/g, ' ')
      .replace(/ھ/g, 'ه')
      .replace(/ئ/g, 'ی');
  }


  getDataRowId(data: { rowData: { [x: string]: any; }; }) {
    for (const key in data.rowData) {
      if (key.endsWith('Id')) {
        return data.rowData[key];
      }
    }
  }

  groupBy(list: any[], key: any) {
    return list.reduce((rv, x) => {
      (rv[x[key]] = rv[x[key]] || []).push(x);
      return rv;
    }, {});
  };


  // DistinctArray(array: any[], valueKey: string): any[] {
  //   return [...new Map(array.map(item => [item[valueKey], item])).values()];
  // }


  checkUrlInList(list: string[], url: string) {
    return list.map(c => url.toLowerCase().includes(c.toLowerCase())).includes(true)
  }

  downloadFile(response: Response | HttpResponse<Blob>, fileName: string|null = null): void {
    if (!fileName) {
      fileName = 'file';
      fileName = response.headers.get("filename") ?? fileName;
    }
    let dataType = (response as HttpResponse<Blob>).body?.type;
    let binaryData:BlobPart[] = [];
    binaryData.push(response.body as any);
    let downloadLink = document.createElement('a');
    downloadLink.href = window.URL.createObjectURL(new Blob(binaryData, { type: dataType }));
    downloadLink.setAttribute('download', fileName);
    document.body.appendChild(downloadLink);
    downloadLink.click();
  }



  downloadFileByUrl(url: string, fileName: string = null) {
    return this.http.get(url, { responseType: 'blob', observe: 'response' }).subscribe(res => {
      this.downloadFile(res, fileName);
    })
  }
}

