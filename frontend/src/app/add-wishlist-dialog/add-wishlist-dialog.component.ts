import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { FormArray, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-add-wishlist-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './add-wishlist-dialog.component.html',
  styleUrl: './add-wishlist-dialog.component.scss'
})
export class AddWishlistDialogComponent {
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<AddWishlistDialogComponent>
  ) {
    this.form = this.fb.group({
      items: this.fb.array([
        this.fb.group({ name: ['', Validators.required], description: [''] })
      ])
    });
  }

  get items() {
    return this.form.get('items') as FormArray;
  }

  addItem() {
    this.items.push(this.fb.group({ name: ['', Validators.required], description: [''] }));
  }

  removeItem(index: number) {
    if (this.items.length > 1) {
      this.items.removeAt(index);
    }
  }

  submit() {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value.items);
    }
  }

  cancel() {
    this.dialogRef.close();
  }
}
