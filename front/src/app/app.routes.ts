import { Routes } from "@angular/router";
import { HomeComponent } from "./shared/features/home/home.component";
import { RecapComponent } from "./cart/recap.component";

export const APP_ROUTES: Routes = [
  {
    path: "home",
    component: HomeComponent,
  },
  {
    path: "products",
    loadChildren: () =>
      import("./products/products.routes").then((m) => m.PRODUCTS_ROUTES),
  },
  { path: "", redirectTo: "home", pathMatch: "full" },
  { path: "recap", component: RecapComponent },
];
