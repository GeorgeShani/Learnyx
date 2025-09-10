import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

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
  cardType = '';

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

  constructor(
    private formBuilder: FormBuilder,
    private sanitizer: DomSanitizer
  ) {
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

  detectCardType(cardNumber: string): string {
    const number = cardNumber.replace(/\s/g, '');

    if (/^4/.test(number)) return 'visa';
    if (/^5[1-5]/.test(number)) return 'mastercard';
    if (/^3[47]/.test(number)) return 'amex';
    if (/^6(?:011|5)/.test(number)) return 'discover';

    return '';
  }

  getCardBrandIcon(): SafeHtml {
    switch (this.cardType) {
      case 'visa':
        return this.sanitizer.bypassSecurityTrustHtml(
          `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 640 640"><path d="M502.1 295.3C502.1 295.3 509.7 332.5 511.4 340.3L478 340.3C481.3 331.4 494 296.8 494 296.8C493.8 297.1 497.3 287.7 499.3 281.9L502.1 295.3zM608 144L608 496C608 522.5 586.5 544 560 544L80 544C53.5 544 32 522.5 32 496L32 144C32 117.5 53.5 96 80 96L560 96C586.5 96 608 117.5 608 144zM184.5 395.2L247.7 240L205.2 240L165.9 346L161.6 324.5L147.6 253.1C145.3 243.2 138.2 240.4 129.4 240L64.7 240L64 243.1C79.8 247.1 93.9 252.9 106.2 260.2L142 395.2L184.5 395.2zM278.9 395.4L304.1 240L263.9 240L238.8 395.4L278.9 395.4zM418.8 344.6C419 326.9 408.2 313.4 385.1 302.3C371 295.2 362.4 290.4 362.4 283.1C362.6 276.5 369.7 269.7 385.5 269.7C398.6 269.4 408.2 272.5 415.4 275.6L419 277.3L424.5 243.7C416.6 240.6 404 237.1 388.5 237.1C348.8 237.1 320.9 258.3 320.7 288.5C320.4 310.8 340.7 323.2 355.9 330.7C371.4 338.3 376.7 343.3 376.7 350C376.5 360.4 364.1 365.2 352.6 365.2C336.6 365.2 328 362.7 314.9 356.9L309.6 354.4L304 389.3C313.4 393.6 330.8 397.4 348.8 397.6C391 397.7 418.5 376.8 418.8 344.6zM560 395.4L527.6 240L496.5 240C486.9 240 479.6 242.8 475.5 252.9L415.8 395.4L458 395.4C458 395.4 464.9 376.2 466.4 372.1L518 372.1C519.2 377.6 522.8 395.4 522.8 395.4L560 395.4z"/></svg>`
        );
      case 'mastercard':
        return this.sanitizer.bypassSecurityTrustHtml(
          `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 640 640"><path d="M514.9 474.3C514.9 481.1 510.3 486 503.7 486C496.9 486 492.5 480.8 492.5 474.3C492.5 467.8 496.9 462.6 503.7 462.6C510.3 462.6 514.9 467.8 514.9 474.3zM204.1 462.6C197 462.6 192.9 467.8 192.9 474.3C192.9 480.8 197 486 204.1 486C210.6 486 215 481.1 215 474.3C214.9 467.8 210.6 462.6 204.1 462.6zM321.6 462.3C316.2 462.3 312.9 465.8 312.1 471L331.2 471C330.3 465.3 326.8 462.3 321.6 462.3zM429.4 462.6C422.6 462.6 418.5 467.8 418.5 474.3C418.5 480.8 422.6 486 429.4 486C436.2 486 440.6 481.1 440.6 474.3C440.6 467.8 436.2 462.6 429.4 462.6zM535.3 488.7C535.3 489 535.6 489.2 535.6 489.8C535.6 490.1 535.3 490.3 535.3 490.9C535 491.2 535 491.4 534.8 491.7C534.5 492 534.3 492.2 533.7 492.2C533.4 492.5 533.2 492.5 532.6 492.5C532.3 492.5 532.1 492.5 531.5 492.2C531.2 492.2 531 491.9 530.7 491.7C530.4 491.4 530.2 491.2 530.2 490.9C529.9 490.4 529.9 490.1 529.9 489.8C529.9 489.3 529.9 489 530.2 488.7C530.2 488.2 530.5 487.9 530.7 487.6C531 487.3 531.2 487.3 531.5 487.1C532 486.8 532.3 486.8 532.6 486.8C533.1 486.8 533.4 486.8 533.7 487.1C534.2 487.4 534.5 487.4 534.8 487.6C535.1 487.8 535 488.2 535.3 488.7zM533.1 490.1C533.6 490.1 533.6 489.8 533.9 489.8C534.2 489.5 534.2 489.3 534.2 489C534.2 488.7 534.2 488.5 533.9 488.2C533.6 488.2 533.4 487.9 532.8 487.9L531.2 487.9L531.2 491.4L532 491.4L532 490L532.3 490L533.4 491.4L534.2 491.4L533.1 490.1zM608 145L608 497C608 523.5 586.5 545 560 545L80 545C53.5 545 32 523.5 32 497L32 145C32 118.5 53.5 97 80 97L560 97C586.5 97 608 118.5 608 145zM96 284.6C96 361.1 158.1 423.1 234.5 423.1C261.7 423.1 288.4 414.9 311 400C238.1 340.7 238.6 228.8 311 169.5C288.4 154.5 261.7 146.4 234.5 146.4C158.1 146.3 96 208.4 96 284.6zM320 393.4C390.5 338.4 390.2 231.2 320 175.9C249.8 231.2 249.5 338.5 320 393.4zM177.7 469.7C177.7 461 172 455.3 163 455C158.4 455 153.5 456.4 150.2 461.5C147.8 457.4 143.7 455 138 455C134.2 455 130.4 456.4 127.4 460.4L127.4 456L119.2 456L119.2 492.7L127.4 492.7C127.4 473.8 124.9 462.5 136.4 462.5C146.6 462.5 144.6 472.7 144.6 492.7L152.5 492.7C152.5 474.4 150 462.5 161.5 462.5C171.7 462.5 169.7 472.5 169.7 492.7L177.9 492.7L177.9 469.7L177.7 469.7zM222.6 456L214.7 456L214.7 460.4C212 457.1 208.2 455 203 455C192.7 455 184.8 463.2 184.8 474.3C184.8 485.5 192.7 493.6 203 493.6C208.2 493.6 212 491.7 214.7 488.2L214.7 492.8L222.6 492.8L222.6 456zM263.1 481.6C263.1 466.6 240.2 473.4 240.2 466.4C240.2 460.7 252.1 461.6 258.7 465.3L262 458.8C252.6 452.7 231.8 452.8 231.8 467C231.8 481.3 254.7 475.3 254.7 482C254.7 488.3 241.2 487.8 234 482.8L230.5 489.1C241.7 496.7 263.1 495.1 263.1 481.6zM298.5 490.9L296.3 484.1C292.5 486.2 284.1 488.5 284.1 480L284.1 463.4L297.2 463.4L297.2 456L284.1 456L284.1 444.8L275.9 444.8L275.9 456L268.3 456L268.3 463.3L275.9 463.3L275.9 480C275.9 497.6 293.2 494.4 298.5 490.9zM311.8 477.5L339.3 477.5C339.3 461.3 331.9 454.9 321.9 454.9C311.3 454.9 303.7 462.8 303.7 474.2C303.7 494.7 326.3 498.1 337.5 488.4L333.7 482.4C325.9 488.8 314.1 488.2 311.8 477.5zM370.9 456C366.3 454 359.3 454.2 355.7 460.4L355.7 456L347.5 456L347.5 492.7L355.7 492.7L355.7 472C355.7 460.4 365.2 461.9 368.5 463.6L370.9 456zM381.5 474.3C381.5 462.9 393.1 459.2 402.2 465.9L406 459.4C394.4 450.3 373.3 455.3 373.3 474.4C373.3 494.2 395.7 498.2 406 489.4L402.2 482.9C393 489.4 381.5 485.5 381.5 474.3zM448.2 456L440 456L440 460.4C431.7 449.4 410.1 455.6 410.1 474.3C410.1 493.5 432.5 499 440 488.2L440 492.8L448.2 492.8L448.2 456zM481.9 456C479.5 454.8 470.9 453.1 466.7 460.4L466.7 456L458.8 456L458.8 492.7L466.7 492.7L466.7 472C466.7 461 475.7 461.7 479.5 463.6L481.9 456zM522.2 441.1L514.3 441.1L514.3 460.4C506.1 449.5 484.4 455.3 484.4 474.3C484.4 493.7 506.9 498.9 514.3 488.2L514.3 492.8L522.2 492.8L522.2 441.1zM529.8 366L529.8 370.6L530.6 370.6L530.6 366L532.5 366L532.5 365.2L527.9 365.2L527.9 366L529.8 366zM536.4 489.8C536.4 489.3 536.4 488.7 536.1 488.2C535.8 487.9 535.6 487.4 535.3 487.1C535 486.8 534.5 486.6 534.2 486.3C533.7 486.3 533.1 486 532.6 486C532.3 486 531.8 486.3 531.2 486.3C530.7 486.6 530.4 486.8 530.1 487.1C529.6 487.4 529.3 487.9 529.3 488.2C529 488.7 529 489.3 529 489.8C529 490.1 529 490.6 529.3 491.2C529.3 491.5 529.6 492 530.1 492.3C530.4 492.6 530.6 492.8 531.2 493.1C531.7 493.4 532.3 493.4 532.6 493.4C533.1 493.4 533.7 493.4 534.2 493.1C534.5 492.8 535 492.6 535.3 492.3C535.6 492 535.8 491.5 536.1 491.2C536.4 490.6 536.4 490.1 536.4 489.8zM539.6 365.1L538.2 365.1L536.6 368.6L535 365.1L533.6 365.1L533.6 370.5L534.4 370.5L534.4 366.4L536 369.9L537.1 369.9L538.5 366.4L538.5 370.5L539.6 370.5L539.6 365.1zM544 284.6C544 208.4 481.9 146.3 405.5 146.3C378.3 146.3 351.6 154.5 329 169.4C401.1 228.7 402.2 340.9 329 399.9C351.6 414.9 378.5 423 405.5 423C481.9 423.1 544 361.1 544 284.6z"/></svg>`
        );
      case 'amex':
        return this.sanitizer.bypassSecurityTrustHtml(
          `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 640 640"><path d="M32 496C32 522.5 53.5 544 80 544L560 544C586.5 544 608 522.5 608 496L608 494.9L546.3 494.9L514.4 459.8L482.5 494.9L278.8 494.9L278.8 331.1L213 331.1L294.7 146.4L373.3 146.4L401.4 209.6L401.4 146.4L498.6 146.4L515.5 194L532.5 146.4L608 146.4L608 144C608 117.5 586.5 96 560 96L80 96C53.5 96 32 117.5 32 144L32 496zM472.4 474.3L514.6 428L556.6 474.3L608 474.3L540 402.2L608 330.1L557.4 330.1L515.4 376.8L473.9 330.1L422.5 330.1L490 402.6L422.6 474.2L422.6 441.1L339.6 441.1L339.6 418.9L420.5 418.9L420.5 386.6L339.6 386.6L339.6 364.2L422.6 364.2L422.6 331.1L300.6 331.1L300.6 474.3L472.4 474.3zM568.7 402.3L608 444.2L608 360.9L568.7 402.3zM532.4 310.3L569.3 209.7L569.3 310.3L608 310.3L608 167L547.8 167L515.6 256.3L483.7 167L422.5 167L422.5 310.1L359.3 167L308.1 167L245.7 310.3L288.7 310.3L300.6 281.6L366.5 281.6L378.5 310.3L461.2 310.3L461.2 210L498 310.3L532.4 310.3zM314 249.4L333.5 202.5L352.9 249.4L314 249.4z"/></svg>`
        );
      default:
        return this.sanitizer.bypassSecurityTrustHtml(
          `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 640 640"><path d="M64 192L64 224L576 224L576 192C576 156.7 547.3 128 512 128L128 128C92.7 128 64 156.7 64 192zM64 272L64 448C64 483.3 92.7 512 128 512L512 512C547.3 512 576 483.3 576 448L576 272L64 272zM128 424C128 410.7 138.7 400 152 400L200 400C213.3 400 224 410.7 224 424C224 437.3 213.3 448 200 448L152 448C138.7 448 128 437.3 128 424zM272 424C272 410.7 282.7 400 296 400L360 400C373.3 400 384 410.7 384 424C384 437.3 373.3 448 360 448L296 448C282.7 448 272 437.3 272 424z"/></svg>`
        );
    }
  }

  formatCardNumber(event: any): void {
    const value = event.target.value.replace(/\s/g, '').replace(/[^0-9]/gi, '');
    const formattedValue = value.match(/.{1,4}/g)?.join(' ') || value;

    this.cardType = this.detectCardType(value);

    event.target.value = formattedValue;
    this.cardForm.patchValue({ number: formattedValue });
  }

  formatExpiryDate(event: any): void {
    let value = event.target.value.replace(/\D/g, '');

    if (value.length >= 2) {
      value = value.substring(0, 2) + '/' + value.substring(2, 4);
    }

    event.target.value = value;
    this.cardForm.patchValue({ expiry: value });
  }

  formatCVV(event: any): void {
    let value = event.target.value.replace(/\D/g, '');
    const maxLength = this.cardType === 'amex' ? 4 : 3;

    if (value.length > maxLength) {
      value = value.substring(0, maxLength);
    }

    event.target.value = value;
    this.cardForm.patchValue({ cvv: value });
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
