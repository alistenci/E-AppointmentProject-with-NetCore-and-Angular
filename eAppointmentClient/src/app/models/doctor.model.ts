export class DoctorModel{
    id: string = "";
    firstName: string = "";
    lastName: string = "";
    fullName: string = "";
    department: DepartmentModel = new DepartmentModel();
    departmentValue: number = 0;
}

export class DepartmentModel{  // Departman bilgileri obje olarak tutulduğu için ayrı bir model oluşturduk.
    name: string = "";
    value: number = 0;
}