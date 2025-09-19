import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { TokenService } from '@core/services/token.service';
import { AuthService } from '@features/auth/services/auth.service';

@Component({
  selector: 'app-sign-up',
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './sign-up.component.html',
  styleUrl: './sign-up.component.scss',
})
export class SignUpComponent {
  registerForm: FormGroup;
  showPassword = false;
  showConfirmPassword = false;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private tokenService: TokenService,
    private router: Router
  ) {
    this.registerForm = this.formBuilder.group(
      {
        firstName: ['', [Validators.required, Validators.minLength(2)]],
        lastName: ['', [Validators.required, Validators.minLength(2)]],
        email: ['', [Validators.required, Validators.email]],
        role: ['Select your role', [Validators.required]],
        password: [
          '',
          [
            Validators.required,
            Validators.minLength(8),
            this.passwordValidator,
          ],
        ],
        confirmPassword: ['', [Validators.required]],
        agreeToTerms: [false, [Validators.requiredTrue]],
        subscribeNewsletter: [false],
      },
      {
        validators: this.passwordMatchValidator,
      }
    );
  }

  onSubmit(): void {
    if (this.registerForm.valid) {
      const { firstName, lastName, email, password, role } = this.registerForm.value;
      const formData = { firstName, lastName, email, password, role };

      console.log("Registration attempted:", formData);

      this.authService.signUp(formData).subscribe({
        next: (response) => {
          console.log('Registration successful:', response);
          this.tokenService.setToken((response as any).token);
          this.router.navigate(['/']);
        },
        error: (err) => {
          console.error('Registration error:', err);
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

  toggleConfirmPassword(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  onSocialLogin(provider: 'google' | 'facebook'): void {
    console.log(`${provider} registration clicked`);
    this.authService.provideOAuth(provider);
  }

  // Custom Validators
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

  private passwordMatchValidator(
    control: AbstractControl
  ): ValidationErrors | null {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');

    if (!password || !confirmPassword) {
      return null;
    }

    return password.value !== confirmPassword.value
      ? { passwordMismatch: true }
      : null;
  }

  private markFormGroupTouched(): void {
    Object.keys(this.registerForm.controls).forEach((key) => {
      const control = this.registerForm.get(key);
      if (control) {
        control.markAsTouched();
      }
    });
  }

  // Getter methods for easy access to form controls in template
  get firstName() {
    return this.registerForm.get('firstName');
  }

  get lastName() {
    return this.registerForm.get('lastName');
  }

  get email() {
    return this.registerForm.get('email');
  }

  get role() {
    return this.registerForm.get('role');
  }

  get password() {
    return this.registerForm.get('password');
  }

  get confirmPassword() {
    return this.registerForm.get('confirmPassword');
  }

  get agreeToTerms() {
    return this.registerForm.get('agreeToTerms');
  }

  get subscribeNewsletter() {
    return this.registerForm.get('subscribeNewsletter');
  }

  // Helper methods for validation display
  hasFirstNameError(): boolean {
    const firstNameControl = this.firstName;
    return !!(
      firstNameControl &&
      firstNameControl.invalid &&
      firstNameControl.touched
    );
  }

  hasLastNameError(): boolean {
    const lastNameControl = this.lastName;
    return !!(
      lastNameControl &&
      lastNameControl.invalid &&
      lastNameControl.touched
    );
  }

  hasEmailError(): boolean {
    const emailControl = this.email;
    return !!(emailControl && emailControl.invalid && emailControl.touched);
  }

  hasRoleError(): boolean {
    const roleControl = this.role;
    return !!(roleControl && roleControl.invalid && roleControl.touched);
  }

  hasPasswordError(): boolean {
    const passwordControl = this.password;
    return !!(
      passwordControl &&
      passwordControl.invalid &&
      passwordControl.touched
    );
  }

  hasConfirmPasswordError(): boolean {
    const confirmPasswordControl = this.confirmPassword;
    const formErrors = this.registerForm.errors;
    return !!(
      confirmPasswordControl &&
      confirmPasswordControl.touched &&
      (confirmPasswordControl.invalid || formErrors?.['passwordMismatch'])
    );
  }

  // Error message methods
  getFirstNameErrorMessage(): string {
    const firstNameControl = this.firstName;
    if (firstNameControl?.hasError('required')) {
      return 'First name is required';
    }
    if (firstNameControl?.hasError('minlength')) {
      return 'First name must be at least 2 characters long';
    }
    return '';
  }

  getLastNameErrorMessage(): string {
    const lastNameControl = this.lastName;
    if (lastNameControl?.hasError('required')) {
      return 'Last name is required';
    }
    if (lastNameControl?.hasError('minlength')) {
      return 'Last name must be at least 2 characters long';
    }
    return '';
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

  getRoleErrorMessage(): string {
    const roleControl = this.role;
    if (roleControl?.hasError('required')) {
      return 'Please select your role';
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

  getConfirmPasswordErrorMessage(): string {
    const confirmPasswordControl = this.confirmPassword;
    if (confirmPasswordControl?.hasError('required')) {
      return 'Please confirm your password';
    }
    if (this.registerForm.errors?.['passwordMismatch']) {
      return 'Passwords do not match';
    }
    return '';
  }
}
