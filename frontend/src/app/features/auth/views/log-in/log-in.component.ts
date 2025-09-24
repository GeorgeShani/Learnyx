import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { TokenService } from '@core/services/token.service';
import { AuthService } from '@features/auth/services/auth.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, CommonModule, RouterModule],
  templateUrl: './log-in.component.html',
  styleUrl: './log-in.component.scss',
})
export class LogInComponent {
  loginForm: FormGroup;
  showPassword = false;
  isLoading = false;
  returnUrl!: string;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private route: ActivatedRoute,
    private tokenService: TokenService,
    private router: Router
  ) {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: [
        '',
        [Validators.required, Validators.minLength(8), this.passwordValidator],
      ],
      rememberMe: [false],
    });

    this.route.queryParams.subscribe((params) => {
      this.returnUrl = params['returnUrl'] || '/';
    });
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      const { email, password } = this.loginForm.value;
      const formData = { email, password };

      console.log('Login attempt:', formData);

      this.isLoading = true;
      this.authService
        .logIn(formData)
        .pipe(finalize(() => (this.isLoading = false)))
        .subscribe({
          next: (response) => {
            this.tokenService.setToken((response as any).token);
            console.log('Login successful:', response);

            if (this.returnUrl) {
              this.router.navigate([this.returnUrl]);
            } else {
              this.router.navigate(['/']);
            }
          },
          error: (err) => {
            console.error('Login error:', err);
          },
        });
    } else {
      // Mark all fields as touched to show validation errors
      this.markFormGroupTouched();
    }
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  onSocialLogin(provider: 'google' | 'facebook'): void {
    console.log(`${provider} login clicked`);
    this.authService.provideOAuth(provider);
  }

  private markFormGroupTouched(): void {
    Object.keys(this.loginForm.controls).forEach((key) => {
      const control = this.loginForm.get(key);
      if (control) {
        control.markAsTouched();
      }
    });
  }

  private passwordValidator(control: AbstractControl): ValidationErrors | null {
    const value = control.value;
    if (!value) {
      return null;
    }

    const hasNumber = /[0-9]/.test(value);
    const hasUpper = /[A-Z]/.test(value);
    const hasLower = /[a-z]/.test(value);
    const hasSpecial = /[#?!@$%^&*-]/.test(value);

    const passwordValid = hasNumber && hasUpper && hasLower && hasSpecial;

    return !passwordValid
      ? {
          passwordStrength: {
            hasNumber,
            hasUpper,
            hasLower,
            hasSpecial,
          },
        }
      : null;
  }

  // Getter methods for easy access to form controls in template
  get email() {
    return this.loginForm.get('email');
  }

  get password() {
    return this.loginForm.get('password');
  }

  get rememberMe() {
    return this.loginForm.get('rememberMe');
  }

  // Helper methods for validation display
  hasEmailError(): boolean {
    const emailControl = this.email;
    return !!(emailControl && emailControl.invalid && emailControl.touched);
  }

  hasPasswordError(): boolean {
    const passwordControl = this.password;
    return !!(
      passwordControl &&
      passwordControl.invalid &&
      passwordControl.touched
    );
  }

  getEmailErrorMessage(): string {
    const emailControl = this.email;
    if (emailControl?.hasError('required')) {
      return 'Email is required';
    }
    if (emailControl?.hasError('email')) {
      return 'Please enter a valid email address';
    }
    return '';
  }

  getPasswordErrorMessage(): string {
    const passwordControl = this.password;
    if (passwordControl?.hasError('required')) {
      return 'Password is required';
    }
    if (passwordControl?.hasError('minlength')) {
      return 'Password must be at least 8 characters long';
    }
    if (passwordControl?.hasError('passwordStrength')) {
      const errors = passwordControl.errors?.['passwordStrength'];
      const missing = [];
      if (!errors.hasNumber) missing.push('a number');
      if (!errors.hasUpper) missing.push('an uppercase letter');
      if (!errors.hasLower) missing.push('a lowercase letter');
      if (!errors.hasSpecial) missing.push('a special character');
      return `Password must contain ${missing.join(', ')}`;
    }
    return '';
  }
}
