import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

export interface UserLoginDto {
  userNameOrEmail: string;
  password: string;
}

export interface UserRegisterDto {
  userName: string;
  email: string;
  phone: string;
  password: string;
}

export interface UserAuthResultDto {
  id: number;
  userName: string;
  email: string;
}

@Injectable({ providedIn: 'root' })
export class UserService {
  private api = inject(ApiService);

  /**
   * User registration
   */
  register(registerDto: UserRegisterDto): Observable<UserAuthResultDto> {
    return this.api.post<UserAuthResultDto>('userauth/register', registerDto);
  }

  /**
   * User login with email/username and password
   */
  login(loginDto: UserLoginDto): Observable<UserAuthResultDto> {
    return this.api.post<UserAuthResultDto>('userauth/login', loginDto);
  }
}
