import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { departments } from '../../constants';
import { DoctorModel } from '../../models/doctor.model';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule, DatePipe } from '@angular/common';
import { DxSchedulerModule } from 'devextreme-angular';
import { HttpService } from '../../services/http.service';
import { AppointmentModel } from '../../models/appointment.model';
import { createAppointmentModel } from '../../models/create-appointment.model';
import { FormValidateDirective } from 'form-validate-angular';
import { PatientModel } from '../../models/patient.model';
import { SwalService } from '../../services/swal.service';
import { AuthService } from '../../services/auth.service';

declare const $:any;

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [FormsModule, CommonModule, DxSchedulerModule, FormValidateDirective ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  providers: [DatePipe]
})
export class HomeComponent implements OnInit {

  @ViewChild("addModalCloseBtn") addModalCloseBtn : ElementRef<HTMLButtonElement> | undefined;
  constructor(
    private http: HttpService,
    private date: DatePipe,
    private swal: SwalService,
    private auth: AuthService
  ){}
  ngOnInit(): void {
    console.log("Us: ",this.auth.tokenDecode.username);
    console.log(this.auth.tokenDecode.name);
  }

  departments = departments;
  doctors: DoctorModel[] = [];
  createModel: createAppointmentModel = new createAppointmentModel();


  selectedDepartmentValue: number = 0;
  selectedDoctorId: string = "";

  appointments: AppointmentModel[] = []


  getAllDoctor(){
    this.selectedDoctorId = "";
    if(this.selectedDepartmentValue > 0){
      this.http.post<DoctorModel[]>("Appointments/GetDoctorsByDepartment", {departmentValue: +this.selectedDepartmentValue}, (res) => {
        this.doctors = res.data;
      })
    }
  }

  getAllAppointments(){
    if(this.selectedDoctorId){
      this.http.post<AppointmentModel[]>("Appointments/GetAllByDoctorId", {doctorId: this.selectedDoctorId}, (res) => {
        this.appointments = res.data;
      });
   }
  }

  onAppointmentFormOpening(e:any){
    e.cancel = true;
    this.createModel.startDate = this.date.transform(e.appointmentData.startDate, "dd.MM.yyyy HH:mm") ?? "";
    this.createModel.endDate = this.date.transform(e.appointmentData.endDate, "dd.MM.yyyy HH:mm") ?? "";
    this.createModel.doctorId = this.selectedDoctorId;

    $("#addModal").modal("show");
  }

  getPatientByIdentityNumber(){
    this.http.post<PatientModel>("Appointments/GetPatientByIdentityNumber" , {identityNumber: this.createModel.identityNumber}, res => {
      if(res.data === null){
        this.createModel.patientId = null;
        this.createModel.firstName = "";
        this.createModel.lastName = "";
        this.createModel.city = "";
        this.createModel.town = "";
        this.createModel.fullAddress = "";
        return;  
      }

      this.createModel.patientId = res.data.id;
      this.createModel.firstName = res.data.firstName;
      this.createModel.lastName = res.data.lastName;
      this.createModel.city = res.data.city;
      this.createModel.town = res.data.town;
      this.createModel.fullAddress = res.data.fullAddress;

    })
  }
  Create(form: NgForm) {
    if (form.valid) {
      this.http.post<string>("Appointments/Create", this.createModel, res => {
        this.swal.callToast(res.data);
        this.addModalCloseBtn?.nativeElement.click();
        this.createModel = new createAppointmentModel();
        this.getAllAppointments();
      })
    }
  }

  onAppointmentDeleted(e:any){
    e.cancel = true;
    
  }
  onAppointmentDeleting(e:any){
    e.cancel = true;

    this.swal.callSwal("Delete appointment" , `You want to delete ${e.appointmentData.patient.fullName} appointment?` , ()=> {
      this.http.post<string>("Appointments/DeleteById", {id: e.appointmentData.id}, res => {
        this.swal.callToast(res.data, "info");
        this.getAllAppointments();
      })
    })
  }
  onAppointmentUpdating(e: any) {
    e.cancel = true;

    const data = {
      id: e.oldData.id,
      startDate: e.newData.startDate,
      endDate: e.newData.endDate,
    };

    this.http.post<string>("Appointments/Update", data, res => {
      this.swal.callToast(res.data);
      this.getAllAppointments();
    });
  }
}