import { Component, OnInit } from '@angular/core';
import { WishlistService, Wishlist } from '../wishlist.service';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AddWishlistDialogComponent } from '../add-wishlist-dialog/add-wishlist-dialog.component';

@Component({
  selector: 'app-wishlist-list',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatDialogModule, RouterModule],
  templateUrl: './wishlist-list.component.html',
  styleUrl: './wishlist-list.component.scss'
})
export class WishlistListComponent implements OnInit {
  wishlists: Wishlist[] = [];
  loading = true;
  error = '';

  constructor(private wishlistService: WishlistService, private dialog: MatDialog) {}

  ngOnInit(): void {
    this.loadWishlists();
  }

  loadWishlists() {
    this.loading = true;
    this.wishlistService.getAllWishlists().subscribe({
      next: (res) => {
        this.wishlists = res.wishlists;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Fehler beim Laden der Wishlists';
        this.loading = false;
      }
    });
  }

  openAddDialog() {
    const dialogRef = this.dialog.open(AddWishlistDialogComponent, {
      width: '500px',
    });
    dialogRef.afterClosed().subscribe((items) => {
      if (items && items.length > 0) {
        this.wishlistService.addWishlist(items).subscribe({
          next: () => this.loadWishlists(),
        });
      }
    });
  }
}
