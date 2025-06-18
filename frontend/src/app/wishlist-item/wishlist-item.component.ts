import { Component, Input } from '@angular/core';
import { WishlistItem, WishlistService } from '../wishlist.service';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-wishlist-item',
  standalone: true,
  imports: [CommonModule, MatCheckboxModule],
  templateUrl: './wishlist-item.component.html',
  styleUrl: './wishlist-item.component.scss'
})
export class WishlistItemComponent {
  @Input() item!: WishlistItem;
  @Input() wishlistId!: number;

  constructor(private wishlistService: WishlistService) {}

  toggleDone() {
    const newStatus = !this.item.isDone;
    // Optimistic UI
    this.item.isDone = newStatus;
    this.wishlistService.updateItem(
      this.wishlistId,
      this.item.id,
      this.item.name,
      this.item.description ?? '',
      newStatus
    ).subscribe({
      error: () => {
        // Rollback bei Fehler
        this.item.isDone = !newStatus;
      }
    });
  }
}
