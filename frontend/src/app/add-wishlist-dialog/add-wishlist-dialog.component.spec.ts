import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddWishlistDialogComponent } from './add-wishlist-dialog.component';

describe('AddWishlistDialogComponent', () => {
  let component: AddWishlistDialogComponent;
  let fixture: ComponentFixture<AddWishlistDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddWishlistDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddWishlistDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
