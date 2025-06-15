import { Component } from "@angular/core";
import { AuthService } from "./auth.service";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms"; // Ajout ici

@Component({
  selector: "app-login",
  standalone: true,
  imports: [CommonModule, FormsModule], // Ajout ici
  template: `
    <div class="login-form">
      <h2>Login</h2>
      <form (ngSubmit)="onLogin()">
        <input [(ngModel)]="username" name="username" placeholder="Username" required />
        <input [(ngModel)]="password" name="password" type="password" placeholder="Password" required />
        <button type="submit">Login</button>
        <div *ngIf="error" style="color:red;">Login incorrect</div>
      </form>
    </div>
  `,
  styles: [`.login-form { max-width: 300px; margin: 100px auto; }`]
})
export class LoginComponent {
  username = '';
  password = '';
  error = false;

  constructor(private auth: AuthService) {}

  async onLogin() {
    const success = await this.auth.login(this.username, this.password);
    this.error = !success;
  }
}