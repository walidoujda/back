import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http'; // ✅ Ajouté HttpClientModule
import { AppComponent } from './app.component';
import { TokenInterceptor } from './auth/token.interceptor';

@NgModule({
  declarations: [
    AppComponent
    // ...autres composants
  ],
  imports: [
    BrowserModule,
    HttpClientModule // ✅ Nécessaire pour que les intercepteurs HTTP fonctionnent
    // ...autres modules
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true }
  ],
  bootstrap: [AppComponent] // ✅ N'oublie pas le bootstrap dans AppModule
})
export class AppModule {}
