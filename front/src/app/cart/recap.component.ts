import { Component } from "@angular/core";
import { CommonModule } from "@angular/common";

@Component({
  selector: "app-recap",
  standalone: true,
  imports: [CommonModule],
  template: `
    <h1 class="text-2xl font-bold text-center my-8">Récapitulatif du panier</h1>
    <div class="text-center text-gray-500">Votre panier est vide (à compléter selon votre logique panier).</div>
  `
})
export class RecapComponent {}