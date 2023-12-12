import {Injectable} from '@angular/core';
import {BehaviorSubject, Observable, tap} from 'rxjs';
import {HttpClient} from '@angular/common/http';
import {Router} from '@angular/router';
import {TokenStorage} from './jwt/token.service';
import {environment} from "../../shared/environment";
import {AuthenticationResponse} from './model/authentication-response.model';
import {User, UserRole} from './model/user.model';
import {Registration} from './model/registration.model';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  user$ = new BehaviorSubject<User>({id: 0, email: "", password: "", name: "", surname: "", city: "", country: "", phone: "", job: "", companyInfo: "", userRole: UserRole.Unauthenticated });

  constructor(private http: HttpClient,
    private tokenStorage: TokenStorage,
    private router: Router) { }

/*  login(login: Login): Observable<AuthenticationResponse> {
    return this.http
      .post<AuthenticationResponse>(environment.apiHost + 'users/login', login)
      .pipe(
        tap((authenticationResponse) => {
          this.tokenStorage.saveAccessToken(authenticationResponse.accessToken);
          this.setUser();
        })
      );
  }*/

  register(registration: Registration): Observable<AuthenticationResponse> {
    return this.http
    .post<AuthenticationResponse>(environment.apiHost + 'users/register', registration)
    .pipe(
      tap((authenticationResponse) => {
        this.tokenStorage.saveAccessToken(authenticationResponse.accessToken);
        this.setUser();
      })
    );
  }

/*  logout(): void {
    this.router.navigate(['/home']).then(_ => {
      this.tokenStorage.clear();
      this.user$.next({username: "", id: 0, role: "" });
      }
    );
  }*/

  checkIfUserExists(): void {
    const accessToken = this.tokenStorage.getAccessToken();
    if (accessToken == null) {
      return;
    }
    this.setUser();
  }

  private setUser(): void {
    const jwtHelperService = new JwtHelperService();
    const accessToken = this.tokenStorage.getAccessToken() || "";
    id: +jwtHelperService.decodeToken(accessToken).id;
    //this.user$.next(user);
  }
}