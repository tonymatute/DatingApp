import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.css'],
})
export class ConfirmEmailComponent implements OnInit {
  emailConfirmed: boolean = false;
  urlParams: any = {};

  constructor(
    private route: ActivatedRoute,
    private accountService: AccountService,
    private toastrService: ToastrService
  ) {}

  ngOnInit(): void {
    this.urlParams.token = this.route.snapshot.queryParamMap.get('token');
    this.urlParams.userid = this.route.snapshot.queryParamMap.get('userid');
    this.confirmEmail();
  }

  confirmEmail() {
    this.accountService.confirmEmail(this.urlParams).subscribe(() => {
      this.toastrService.success("Email Confirmed succesful");
      this.emailConfirmed = true;
    }, (error) => {
        this.toastrService.error("Unable to confirm your email!");
        this.emailConfirmed = false;
    })
  }
}
