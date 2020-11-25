import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css'],
})
export class ForgotPasswordComponent implements OnInit {
  forgotPasswordForm: FormGroup;
  email: FormControl;
  validationErrors: string[] = [];

  constructor(
    private accountService: AccountService,
    private fb: FormBuilder,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.email = new FormControl('', [
      Validators.required,
      Validators.pattern('^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$'),
      Validators.email,
    ]);
    this.forgotPasswordForm = this.fb.group({
      email: this.email,
    });
  }

  cancel() {
    this.router.navigateByUrl('/');
  }

  resetPassword() {
    let emailInfo = this.forgotPasswordForm.value;
    this.accountService.sendForgotPasswordEmail(emailInfo).subscribe(
      (respond) => {
        this.router.navigateByUrl('/confirm-password-sent');
      },
      (error) => {
        this.validationErrors = error;
      }
    );
  }
}
