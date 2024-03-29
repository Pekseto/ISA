import { Component } from '@angular/core';
import { AuthService } from "../auth.service";
import { Router } from "@angular/router";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { Login } from "../model/login.model";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  constructor(private authService: AuthService, private router: Router) {
  }
  loginForm = new FormGroup({
    username: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required])
  });

  login(): void {
    const login: Login = {
      username: this.loginForm.value.username || '',
      password: this.loginForm.value.password || ''
    };

    if (this.loginForm.valid) {
      this.authService.login(login).subscribe({
        next: () => {
          const user = this.authService.user$.getValue();
          console.log(user)
          if ((user.userRole?.toString() == 'Company_Admin' || user.userRole?.toString() == 'System_Admin') && user.isFirstLogin == 'True') {
            this.router.navigate(['/change-password']);
          } else {
            // TODO: fix this
            // Ovo u sustini reloaduje sve da bi se navbar updateovao
            // Treba ispraviti da se koristi BehaviorSubject i da se kroz click navbara to se updatuje. 
            location.reload();
            alert("Welcome " + user.name + " " + user.surname + "!");
            //this.router.navigate(['/home']);
          }
        },
      });
    }
  }
}
