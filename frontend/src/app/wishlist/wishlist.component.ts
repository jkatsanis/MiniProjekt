import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { WishlistService, Wishlist, WishlistItem } from '../wishlist.service';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { WishlistItemComponent } from '../wishlist-item/wishlist-item.component';

@Component({
  selector: 'app-wishlist',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatCheckboxModule, MatButtonModule, WishlistItemComponent],
  templateUrl: './wishlist.component.html',
  styleUrl: './wishlist.component.scss'
})
export class WishlistComponent implements OnInit {
  wishlist: Wishlist | null = null;
  loading = true;
  error = '';

  constructor(private route: ActivatedRoute, private wishlistService: WishlistService, private router: Router) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.wishlistService.getWishlist(id).subscribe({
      next: (res) => {
        this.wishlist = res;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Fehler beim Laden der Wishlist';
        this.loading = false;
      }
    });
  }

  goBack() {
    this.router.navigate(['/']);
  }
}
