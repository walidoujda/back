import { Component, OnInit, inject, signal } from "@angular/core";
import { Product } from "../../data-access/product.model";
import { ProductsService } from "../../data-access/products.service";
import { ProductFormComponent } from "../../ui/product-form/product-form.component";
import { ButtonModule } from "primeng/button";
import { CardModule } from "primeng/card";
import { DataViewModule } from 'primeng/dataview';
import { DialogModule } from 'primeng/dialog';
import { CommonModule } from '@angular/common';
import { AuthService } from "../../../auth/auth.service";
import { LoginComponent } from "../../../auth/login.component";
import { FormsModule } from "@angular/forms"; // Ajoute cette ligne

const emptyProduct: Product = {
  id: 0,
  code: "",
  name: "",
  description: "",
  image: "",
  category: "",
  price: 0,
  quantity: 0,
  internalReference: "",
  shellId: 0,
  inventoryStatus: "INSTOCK",
  rating: 0,
  createdAt: 0,
  updatedAt: 0,
};

@Component({
  selector: "app-product-list",
  templateUrl: "./product-list.component.html",
  styleUrls: ["./product-list.component.scss"],
  standalone: true,
  imports: [
    CommonModule,
    DataViewModule,
    CardModule,
    ButtonModule,
    DialogModule,
    ProductFormComponent,
    LoginComponent,
    FormsModule // Ajoute FormsModule ici
  ],
})
export class ProductListComponent implements OnInit {
  private readonly productsService = inject(ProductsService);
  private readonly authService = inject(AuthService);

  public readonly products = this.productsService.products;
  public isDialogVisible = false;
  public isCreation = false;
  public readonly editedProduct = signal<Product>(emptyProduct);

  public readonly isLoggedIn = this.authService.isLoggedIn;
  public readonly userEmail = this.authService.email;

  public quantities: { [productId: number]: number } = {};

  ngOnInit() {
    if (this.isLoggedIn()) {
      this.productsService.get().subscribe();
    }
  }

  public onCreate() {
    this.isCreation = true;
    this.isDialogVisible = true;
    this.editedProduct.set(emptyProduct);
  }

  public onUpdate(product: Product) {
    this.isCreation = false;
    this.isDialogVisible = true;
    this.editedProduct.set(product);
  }

  public onDelete(product: Product) {
    this.productsService.delete(product.id).subscribe();
  }

  public onSave(product: Product) {
    if (this.isCreation) {
      this.productsService.create(product).subscribe();
    } else {
      this.productsService.update(product).subscribe();
    }
    this.closeDialog();
  }

  public onCancel() {
    this.closeDialog();
  }

  private closeDialog() {
    this.isDialogVisible = false;
  }

  public addToCart(productId: number) {
    const quantity = this.quantities[productId] || 1;
    this.productsService.addToCart(productId, quantity).subscribe({
      next: () => alert('Produit ajouté au panier !'),
      error: () => alert('Erreur lors de l\'ajout au panier')
    });
  }
}
