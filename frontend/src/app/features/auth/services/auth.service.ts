import { Injectable } from '@angular/core';
import { ApiService } from '@core/services/api.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private googleAuthUrl = 'https://accounts.google.com/o/oauth2/v2/auth';
  private facebookAuthUrl = 'https://www.facebook.com/v18.0/dialog/oauth';

  private clientIdGoogle = '348733949068-hvcqp682kgdr6499lka5jsujvmb1f1ud.apps.googleusercontent.com';
  private clientIdFacebook = '1406683273763679';
  private googleRedirectUri = 'https://localhost:7188/api/auth/google/callback';
  private facebookRedirectUri = 'https://localhost:7188/api/auth/facebook/callback';

  constructor(private apiService: ApiService) {}

  signUp(body: any) {
    return this.apiService.post('/api/auth/signup', body);
  }

  logIn(body: any) {
    return this.apiService.post('/api/auth/login', body);
  }

  provideOAuth(oauthType: 'google' | 'facebook') {
    let oauthUrl = '';

    if (oauthType === 'google') {
      oauthUrl =
        this.googleAuthUrl +
        '?' +
        new URLSearchParams({
          client_id: this.clientIdGoogle,
          redirect_uri: this.googleRedirectUri,
          response_type: 'code',
          scope: 'openid email profile',
          prompt: 'select_account',
        });
    } else if (oauthType === 'facebook') {
      oauthUrl =
        this.facebookAuthUrl +
        '?' +
        new URLSearchParams({
          client_id: this.clientIdFacebook,
          redirect_uri: this.facebookRedirectUri,
          response_type: 'code',
          scope: 'email,public_profile',
          auth_type: 'rerequest',
        });
    }
      
    console.log(oauthUrl);
    // Simply redirect the whole window
    window.location.href = oauthUrl;
  }
}
