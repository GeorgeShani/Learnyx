import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

interface Plan {
  id: string;
  name: string;
  price: number;
  description: string;
  features: string[];
}

@Component({
  selector: 'app-billing',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './billing.component.html',
  styleUrl: './billing.component.scss',
})
export class BillingComponent {
  selectedPlan = 'premium';
  paymentMethod = '';
  cardForm: FormGroup;

  plans: Plan[] = [
    {
      id: 'basic',
      name: 'Basic',
      price: 9.99,
      description: 'Perfect for individual learners',
      features: ['5 courses', 'Basic support', 'Mobile access'],
    },
    {
      id: 'premium',
      name: 'Premium',
      price: 19.99,
      description: 'Most popular choice',
      features: [
        'Unlimited courses',
        'Priority support',
        'Downloadable resources',
        'Certificates',
      ],
    },
    {
      id: 'enterprise',
      name: 'Enterprise',
      price: 49.99,
      description: 'For teams and organizations',
      features: [
        'Everything in Premium',
        'Team management',
        'Analytics dashboard',
        'Custom branding',
      ],
    },
  ];

  constructor(private formBuilder: FormBuilder) {
    this.cardForm = this.formBuilder.group({
      name: ['', Validators.required],
      number: [
        '',
        [
          Validators.required,
          Validators.pattern(/^\d{4}\s?\d{4}\s?\d{4}\s?\d{4}$/),
        ],
      ],
      expiry: [
        '',
        [Validators.required, Validators.pattern(/^(0[1-9]|1[0-2])\/\d{2}$/)],
      ],
      cvv: ['', [Validators.required, Validators.pattern(/^\d{3,4}$/)]],
    });
  }

  ngOnInit(): void {}

  get selectedPlanData(): Plan | undefined {
    return this.plans.find((p) => p.id === this.selectedPlan);
  }

  selectPlan(planId: string): void {
    this.selectedPlan = planId;
  }

  selectPaymentMethod(method: string): void {
    this.paymentMethod = method;

    // Mock payment processing
    if (method !== 'manual') {
      this.showToast(`${method} selected! (Mock implementation)`, 'success');
    }
  }

  processManualPayment(): void {
    if (this.cardForm.invalid) {
      this.showToast('Please fill in all card details correctly', 'error');
      return;
    }

    this.showToast(
      'Payment processed successfully! (Mock implementation)',
      'success'
    );
  }

  private showToast(message: string, type: 'success' | 'error'): void {
    // Mock toast implementation - in a real app, you'd use a toast service
    console.log(`[${type.toUpperCase()}] ${message}`);

    // Create a simple toast notification
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.textContent = message;
    toast.style.cssText = `
      position: fixed;
      top: 20px;
      right: 20px;
      padding: 12px 24px;
      border-radius: 8px;
      color: white;
      font-weight: 500;
      z-index: 1000;
      animation: slideIn 0.3s ease;
      background: ${
        type === 'success' ? 'hsl(142, 76%, 36%)' : 'hsl(0, 84%, 60%)'
      };
    `;

    document.body.appendChild(toast);

    setTimeout(() => {
      toast.remove();
    }, 3000);
  }
}
