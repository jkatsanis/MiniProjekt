import { Routes } from '@angular/router';
import { WishlistListComponent } from './wishlist-list/wishlist-list.component';
import { WishlistComponent } from './wishlist/wishlist.component';

export const routes: Routes = [
  { path: '', component: WishlistListComponent },
  { path: 'wishlist/:id', component: WishlistComponent },
];
