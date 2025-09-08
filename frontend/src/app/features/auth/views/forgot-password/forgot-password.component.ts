import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';

interface ToastMessage {
  title: string;
  description: string;
  variant?: 'default' | 'destructive';
}

@Component({
  selector: 'app-forgot-password',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.scss',
})
export class ForgotPasswordComponent {
  currentStep = 1;
  isLoading = false;
  showPassword = false;
  showConfirmPassword = false;

  emailForm: FormGroup;
  verificationForm: FormGroup;
  passwordForm: FormGroup;

  verificationCode = '';
  otpDigits: string[] = ['', '', '', '', '', ''];
  private isUpdating = false; // Prevent circular updates

  constructor(private formBuilder: FormBuilder, private router: Router) {
    this.emailForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
    });

    this.verificationForm = this.formBuilder.group({
      code: ['', [Validators.required, Validators.minLength(6)]],
    });

    this.passwordForm = this.formBuilder.group(
      {
        password: ['', [Validators.required, Validators.minLength(8)]],
        confirmPassword: ['', [Validators.required]],
      },
      { validators: this.passwordMatchValidator }
    );
  }

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');

    if (
      password &&
      confirmPassword &&
      password.value !== confirmPassword.value
    ) {
      confirmPassword.setErrors({ mismatch: true });
      return { mismatch: true };
    }

    return null;
  }

  getStepTitle(): string {
    switch (this.currentStep) {
      case 1:
        return 'Reset Your Password';
      case 2:
        return 'Enter Verification Code';
      case 3:
        return 'Create New Password';
      default:
        return '';
    }
  }

  getStepDescription(): string {
    switch (this.currentStep) {
      case 1:
        return "Enter your email address and we'll send you a verification code";
      case 2:
        return 'Enter the 6-digit code we sent to your email';
      case 3:
        return 'Create a new secure password for your account';
      default:
        return '';
    }
  }

  getIconClass(): string {
    return 'icon-container';
  }

  async handleEmailSubmit(): Promise<void> {
    if (this.emailForm.invalid) {
      this.showToast({
        title: 'Email Required',
        description: 'Please enter your email address.',
        variant: 'destructive',
      });
      return;
    }

    this.isLoading = true;

    // Simulate API call
    await new Promise((resolve) => setTimeout(resolve, 1000));

    this.isLoading = false;

    this.showToast({
      title: 'Verification Code Sent',
      description: "We've sent a 6-digit verification code to your email.",
    });

    this.currentStep = 2;
  }

  async handleVerificationSubmit(): Promise<void> {
    if (this.verificationCode.length !== 6) {
      this.showToast({
        title: 'Invalid Code',
        description: 'Please enter the complete 6-digit verification code.',
        variant: 'destructive',
      });
      return;
    }

    // Static validation - check if code is "123456"
    if (this.verificationCode !== '123456') {
      this.showToast({
        title: 'Invalid Code',
        description: 'The verification code is incorrect. Please try again.',
        variant: 'destructive',
      });
      return;
    }

    this.isLoading = true;
    await new Promise((resolve) => setTimeout(resolve, 500));
    this.isLoading = false;

    this.showToast({
      title: 'Code Verified',
      description: 'Please create your new password.',
    });

    this.currentStep = 3;
  }

  async handlePasswordReset(): Promise<void> {
    if (this.passwordForm.invalid) {
      const passwordControl = this.passwordForm.get('password');
      const confirmPasswordControl = this.passwordForm.get('confirmPassword');

      if (passwordControl?.errors?.['minlength']) {
        this.showToast({
          title: 'Password Too Short',
          description: 'Password must be at least 8 characters long.',
          variant: 'destructive',
        });
        return;
      }

      if (confirmPasswordControl?.errors?.['mismatch']) {
        this.showToast({
          title: "Passwords Don't Match",
          description: 'Please make sure both passwords match.',
          variant: 'destructive',
        });
        return;
      }
    }

    this.isLoading = true;
    await new Promise((resolve) => setTimeout(resolve, 1000));
    this.isLoading = false;

    this.showToast({
      title: 'Password Reset Successfully',
      description: 'Your password has been updated. You can now log in.',
    });

    this.router.navigate(['/auth/login']);
  }

  // OTP Input Methods
  onOtpInput(event: Event, index: number): void {
    if (this.isUpdating) return;

    const input = event.target as HTMLInputElement;
    let value = input.value;

    // Handle paste or multiple chars
    if (value.length > 1) {
      this.handleOtpPaste(value, index);
      return;
    }

    // Validate single character (numbers only)
    if (value && !/^\d$/.test(value)) {
      this.setOtpInputValue(index, this.otpDigits[index] || '');
      return;
    }

    // Update our state
    this.otpDigits[index] = value;
    this.updateVerificationCode();

    // Auto advance to next input
    if (value && index < 5) {
      setTimeout(() => {
        const nextInput = document.getElementById(
          `otp-${index + 1}`
        ) as HTMLInputElement;
        if (nextInput) {
          nextInput.focus();
        }
      }, 0);
    }
  }

  onOtpPaste(event: ClipboardEvent, index: number): void {
    event.preventDefault();
    const paste = event.clipboardData?.getData('text') || '';
    this.handleOtpPaste(paste, index);
  }

  onOtpKeydown(event: KeyboardEvent, index: number): void {
    if (event.key === 'Backspace') {
      event.preventDefault();

      if (this.otpDigits[index]) {
        // Clear current
        this.otpDigits[index] = '';
        this.setOtpInputValue(index, '');
        this.updateVerificationCode();
      } else if (index > 0) {
        // Go back and clear
        const prevIndex = index - 1;
        this.otpDigits[prevIndex] = '';
        this.setOtpInputValue(prevIndex, '');
        setTimeout(() => {
          const prevInput = document.getElementById(
            `otp-${prevIndex}`
          ) as HTMLInputElement;
          if (prevInput) {
            prevInput.focus();
          }
        }, 0);
        this.updateVerificationCode();
      }
    } else if (event.key === 'Delete') {
      event.preventDefault();
      this.otpDigits[index] = '';
      this.setOtpInputValue(index, '');
      this.updateVerificationCode();
    } else if (event.key === 'ArrowLeft' && index > 0) {
      event.preventDefault();
      const prevInput = document.getElementById(
        `otp-${index - 1}`
      ) as HTMLInputElement;
      if (prevInput) {
        prevInput.focus();
      }
    } else if (event.key === 'ArrowRight' && index < 5) {
      event.preventDefault();
      const nextInput = document.getElementById(
        `otp-${index + 1}`
      ) as HTMLInputElement;
      if (nextInput) {
        nextInput.focus();
      }
    } else if (event.key === 'Home') {
      event.preventDefault();
      const firstInput = document.getElementById('otp-0') as HTMLInputElement;
      if (firstInput) {
        firstInput.focus();
      }
    } else if (event.key === 'End') {
      event.preventDefault();
      const lastInput = document.getElementById('otp-5') as HTMLInputElement;
      if (lastInput) {
        lastInput.focus();
      }
    }
    // Block invalid keys
    else if (
      !/[\d]/.test(event.key) &&
      !['Tab', 'Shift', 'Control', 'Alt', 'Meta'].includes(event.key) &&
      !event.ctrlKey &&
      !event.metaKey
    ) {
      event.preventDefault();
    }
  }

  onOtpFocus(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input && input.value) {
      input.select();
    }
  }

  // Private OTP Helper Methods
  private setOtpInputValue(index: number, value: string): void {
    const input = document.getElementById(`otp-${index}`) as HTMLInputElement;
    if (input) {
      this.isUpdating = true;
      input.value = value;
      this.isUpdating = false;
    }
  }

  private updateVerificationCode(): void {
    if (this.isUpdating) return;
    this.verificationCode = this.otpDigits.join('');
  }

  private handleOtpPaste(value: string, startIndex: number): void {
    const chars = value.replace(/\D/g, '').split('');

    // Update digits array
    for (let i = 0; i < chars.length && startIndex + i < 6; i++) {
      this.otpDigits[startIndex + i] = chars[i];
    }

    // Update all inputs at once
    this.isUpdating = true;
    for (let i = 0; i < 6; i++) {
      const input = document.getElementById(`otp-${i}`) as HTMLInputElement;
      if (input) {
        input.value = this.otpDigits[i] || '';
      }
    }
    this.isUpdating = false;

    this.updateVerificationCode();

    // Focus appropriately
    const lastFilledIndex = Math.min(startIndex + chars.length - 1, 5);
    const nextIndex = Math.min(lastFilledIndex + 1, 5);
    setTimeout(() => {
      const focusInput = document.getElementById(
        `otp-${nextIndex}`
      ) as HTMLInputElement;
      if (focusInput) {
        focusInput.focus();
      }
    }, 0);
  }

  // Password visibility toggle
  togglePasswordVisibility(field: 'password' | 'confirm'): void {
    if (field === 'password') {
      this.showPassword = !this.showPassword;
    } else {
      this.showConfirmPassword = !this.showConfirmPassword;
    }
  }

  // Step navigation
  goToStep(step: number): void {
    this.currentStep = step;
  }

  // Toast notification
  private showToast(message: ToastMessage): void {
    // In a real Angular app, you would use a toast service
    // For demo purposes, we'll use console.log
    console.log(`Toast: ${message.title} - ${message.description}`);

    // You could also use Angular Material Snackbar or similar
    // this.snackBar.open(`${message.title}: ${message.description}`, 'Close', {
    //   duration: 5000,
    //   panelClass: message.variant === 'destructive' ? 'error-snackbar' : 'success-snackbar'
    // });
  }
}
