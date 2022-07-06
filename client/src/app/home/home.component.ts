import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { LearnMoreComponent } from '../modals/learn-more/learn-more.component';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;
  bsModalRef: BsModalRef;

  constructor(private modalService: BsModalService) { }

  ngOnInit(): void {
  }

  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  cancelRegisterMode(event: boolean){
    this.registerMode = event;
  }

  openLearnMoreModal() {
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
      }
    }
    this.bsModalRef = this.modalService.show(LearnMoreComponent, config);
  }

}