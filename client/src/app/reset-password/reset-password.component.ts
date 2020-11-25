import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {
  resetPasswordForm: FormGroup;
  validationErrors: string[] = [];
  passwordchanged: boolean = false;
  model: any = {};  

  constructor(private accountService: AccountService,
    private toastrService: ToastrService,
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder) { }

  ngOnInit(): void {
    this.model.token = this.route.snapshot.queryParamMap.get('token');
    this.model.userid = this.route.snapshot.queryParamMap.get('userid');
    this.initializeForm();
  }

  resetPassword() {   
    this.model.password = this.resetPasswordForm.value.password;
    this.accountService.resetPassword(this.model).subscribe(() => {
      this.toastrService.success("Reset Password Sent");
      this.passwordchanged = true;
      this.router.navigateByUrl('/reset-password-complete');
    }, (error) => {
        this.toastrService.error("Unable to reset your password!");
        this.passwordchanged = false;
    })
  }

  initializeForm() {
    this.resetPasswordForm = this.fb.group({
      password: [
        '',
        [Validators.required, Validators.minLength(8), Validators.maxLength(20)],
      ],
      confirmPassword: [
        '',
        [Validators.required, this.matchPassword('password')],
      ]     
    });
  }

  matchPassword(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value
        ? null
        : { isMatching: true };
    };
  }


}
