import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CsvExportServiceService {

  constructor() { }
  static exportToCsv(filename: string, rows: string[]) {
    if (!rows || !rows.length) {
      return;
    }

    const csvContent =
      rows.map(row => {
        return row;
      }).join('\n');

    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    if (navigator.msSaveBlob) { // IE 10+
      navigator.msSaveBlob(blob, filename);
    } else {
      const link = document.createElement('a');
      if (link.download !== undefined) {
        // Browsers that support HTML5 download attribute
        const url = URL.createObjectURL(blob);
        link.setAttribute('href', url);
        link.setAttribute('download', filename);
        link.style.visibility = 'hidden';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
      }
    }
  }
}
