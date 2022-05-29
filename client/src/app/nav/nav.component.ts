import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  @ViewChild('loginForm') loginForm: NgForm;
  model: any = {};
  isCollapsed = true;
  
  constructor(
    public accountService: AccountService,
    private router: Router) { }

  ngOnInit(): void {
  }

  login() {
    this.accountService.login(this.model).subscribe(resp => {
      this.loginForm.resetForm();
      this.router.navigateByUrl('/members');
      this.collapse();
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
    this.collapse();
  }

  collapse() {
    this.isCollapsed = !this.isCollapsed;
  }

}
