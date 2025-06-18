import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface WishlistItem {
  id: number;
  name: string;
  description?: string;
  isDone: boolean;
  wishlistId?: number;
}

export interface Wishlist {
  id: number;
  items: WishlistItem[];
}

@Injectable({
  providedIn: 'root',
})
export class WishlistService {
  private apiUrl = 'http://localhost:5200/api/wishlists';

  constructor(private http: HttpClient) {}

  getAllWishlists(): Observable<{ wishlists: Wishlist[] }> {
    return this.http.get<{ wishlists: Wishlist[] }>(this.apiUrl);
  }

  getWishlist(id: number): Observable<Wishlist> {
    return this.http.get<Wishlist>(`${this.apiUrl}/${id}`);
  }

  addWishlist(items: { name: string; description?: string }[]): Observable<Wishlist> {
    return this.http.post<Wishlist>(this.apiUrl, { items });
  }

  deleteWishlist(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  addItem(wishlistId: number, name: string, description?: string): Observable<WishlistItem> {
    return this.http.post<WishlistItem>(`${this.apiUrl}/${wishlistId}/items`, { name, description });
  }

  updateItem(wishlistId: number, itemId: number, name: string, description?: string, isDone?: boolean): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${wishlistId}/items/${itemId}`, { name, description, isDone });
  }

  deleteItem(wishlistId: number, itemId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${wishlistId}/items/${itemId}`);
  }

  setItemDone(wishlistId: number, itemId: number, isDone: boolean): Observable<void> {
    // Annahme: PATCH-Endpunkt existiert, sonst per updateItem lösen
    return this.http.patch<void>(`${this.apiUrl}/${wishlistId}/items/${itemId}`, { isDone });
  }
}
